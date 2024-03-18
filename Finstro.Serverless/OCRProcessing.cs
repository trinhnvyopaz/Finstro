﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System;

namespace Finstro.Serverless.API
{
    public class OCRProcessing
    {
        public async Task<Receipt> ProcessReceiptImage(byte[] image)
        {
            try
            {
                OCRVisionResponse ocrResponse = await CallComputerVisionOCRAsync(image);

                Receipt processedReceipt = ExtractReceiptLines(ocrResponse);

                processedReceipt = ExtractDataOfInterest(processedReceipt);

                return processedReceipt;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Receipt();

            }
        }
        async Task<OCRVisionResponse> CallComputerVisionOCRAsync(byte[] image)
        {
            using (var client = new HttpClient())
            {
                string apikey = Secrets.apikey;
                string apiendpoint_ocr = Secrets.apiendpoint_ocr;

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apikey);

                // Request parameters. Set languague to "unknown" and detect orientation true
                string requestParameters = "language=unk&detectOrientation=true";

                // Construct the API URI
                string uri = apiendpoint_ocr + "?" + requestParameters;

                // attach image content passed in via post                
                var content = new ByteArrayContent(content: image);
                content.Headers.ContentType = new MediaTypeHeaderValue(mediaType: "application/octet-stream");

                // send to Computer Vision OCR
                var result = await client.PostAsync(requestUri: uri, content: content);
                result.EnsureSuccessStatusCode();

                // fetch string result and return
                var jsonstring = await result.Content.ReadAsStringAsync();

                // Deserialize OCR response to our model and return
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OCRVisionResponse>(json);
            }
        }

        Receipt ExtractReceiptLines(OCRVisionResponse ocr)
        {
            // create Receipt object and populate from OCR response
            Receipt receipt = new Receipt();
            receipt.lines = new List<ReceiptLine>();
            receipt.language = ocr.language;
            receipt.orientation = ocr.orientation;
            receipt.textAngle = ocr.textAngle;

            decimal docminx = 100000;  // set high and reduce
            decimal docminy = 100000;  // set high and reduce
            decimal docwidth = 0;
            decimal docheight = 0;
            double avglineheight = 0;

            // reduce to region/lines
            foreach (Region region in ocr.regions)
            {
                foreach (Line line in region.lines)
                {
                    string strline = "";
                    foreach (Word word in line.words)
                    {
                        strline += word.text + " ";

                    }
                    strline = strline.TrimEnd();

                    ReceiptLine newReceiptLine = new ReceiptLine();
                    newReceiptLine.boundingBox = line.boundingBox;

                    string[] bounds = line.boundingBox.Split(',');
                    int.TryParse(bounds[0], out newReceiptLine.x);
                    int.TryParse(bounds[1], out newReceiptLine.y);
                    int.TryParse(bounds[2], out newReceiptLine.width);
                    int.TryParse(bounds[3], out newReceiptLine.height);

                    newReceiptLine.text = strline;
                    receipt.lines.Add(newReceiptLine);
                    docheight = Math.Max(docheight, newReceiptLine.height + newReceiptLine.y);
                    docwidth = Math.Max(docwidth, newReceiptLine.width + newReceiptLine.x);
                    docminx = Math.Min(docminx, newReceiptLine.x);
                    docminy = Math.Min(docminy, newReceiptLine.y);
                }
            }

            // sort the receipt class by the vertical, then horizontal position
            receipt.lines.Sort((a, b) => a.y == b.y ? a.x.CompareTo(b.x) : (a.y.CompareTo(b.y)));

            // calculate the average line height
            avglineheight = receipt.lines.Average(a => a.height);

            Receipt sortedreceipt = new Receipt();
            sortedreceipt.lines = new List<ReceiptLine>();
            sortedreceipt.language = receipt.language;
            sortedreceipt.orientation = sortedreceipt.orientation;
            sortedreceipt.textAngle = sortedreceipt.textAngle;

            int currenty = 0;
            int currentx = 0;
            int currentheight = 0;
            int currentwidth = 0;

            foreach (ReceiptLine line in receipt.lines)
            {
                // figure out if we're on a new line, based on some line height difference calc (may need tweaking)
                currentheight = line.height;
                if (line.y > (currenty + currentheight - (avglineheight / 3)))
                {
                    currenty = line.y;
                }

                ReceiptLine newline = new ReceiptLine();
                newline.height = currentheight;
                newline.text = line.text;
                newline.y = currenty;
                newline.x = line.x;
                newline.width = line.width;
                newline.boundingBox = String.Format("{0},{1},{2},{3}", newline.x, newline.y, newline.width, newline.height);
                sortedreceipt.lines.Add(newline);
            }

            // re-sort the receipt class by the vertical, then horizontal position
            sortedreceipt.lines.Sort((a, b) => a.y == b.y ? a.x.CompareTo(b.x) : (a.y.CompareTo(b.y)));

            Receipt processedreceipt = new Receipt();
            processedreceipt.lines = new List<ReceiptLine>();
            processedreceipt.language = sortedreceipt.language;
            processedreceipt.orientation = sortedreceipt.orientation;
            processedreceipt.textAngle = sortedreceipt.textAngle;

            // concatenate into single lines
            currenty = 0;
            currentx = 0;
            currentheight = 0;
            currentwidth = 0;
            ReceiptLine processedLine = new ReceiptLine();
            foreach (ReceiptLine line in sortedreceipt.lines)
            {
                if (currenty != line.y)
                {
                    processedLine = new ReceiptLine();
                    processedreceipt.lines.Add(processedLine);
                    currenty = line.y;
                    currentx = line.x;
                    currentwidth = line.width;
                    currentheight = line.height;
                    processedLine.y = currenty;
                    processedLine.x = Math.Min(currentx, line.x);
                    processedLine.width = currentwidth;
                    processedLine.height = currentheight;
                    processedLine.boundingBox = String.Format("{0},{1},{2},{3}", processedLine.x, processedLine.y, processedLine.width, processedLine.height);
                    processedLine.text += line.text + " ";
                }
                else
                {
                    currentx = line.x;
                    processedLine.width = line.width + currentwidth;
                    processedLine.height = Math.Max(currentheight, line.height);
                    processedLine.x = Math.Min(currentx, line.x);
                    processedLine.boundingBox = String.Format("{0},{1},{2},{3}", processedLine.x, processedLine.y, processedLine.width, processedLine.height);
                    processedLine.text += line.text + " ";

                }
            }

            return processedreceipt;
        }

        Receipt ExtractDataOfInterest(Receipt receipt)
        {

            string currentlinetext = "";
            string abn = "";
            decimal receipttotal = 0.00M;
            DateTime receiptdate = new DateTime(1900, 01, 01);
            List<decimal> amounts = new List<decimal>();


            // scan processed receipt lines for ABN, Date, Total
            foreach (ReceiptLine line in receipt.lines)
            {
                currentlinetext = line.text;

                // ABN : Australian Business Number
                if (currentlinetext.ToUpper().Contains("ABN") || currentlinetext.ToUpper().Contains("A.B.N"))
                {
                    // try and extract an ABN from the line
                    var extractedabn = ExtractABN(currentlinetext);

                    // if the extracted ABN passes validation, lets accept and set it
                    if (ValidateABN(extractedabn)) { abn = extractedabn; }
                }


                // Bpay Info 
                if (currentlinetext.ToUpper().Contains("BILLER CODE"))
                {
                    try
                    {
                        var extractedBpay = ExtractBpay(line, receipt.lines);
                        var text = extractedBpay.ToUpper().Split(new string[] { "BILLER" }, StringSplitOptions.None);

                        receipt.biller = new String(text[1].Where(Char.IsDigit).ToArray());
                        receipt.billerRef = new String(text[0].Where(Char.IsDigit).ToArray());

                    }
                    catch
                    {
                    }

                }


                // Receipt Date
                var extractedDateString = ExtractDate(currentlinetext);
                DateTime extractedDate = new DateTime(1900, 01, 01);
                if (extractedDateString != "")
                {
                    DateTime.TryParse(extractedDateString, System.Globalization.CultureInfo.GetCultureInfo("en-AU"), System.Globalization.DateTimeStyles.None, out extractedDate);

                }
                if (extractedDate > receiptdate && extractedDate <= DateTime.Today)
                {
                    // we're going to keep the highest date we find (for now)
                    receiptdate = extractedDate;
                }

                // Receipt Total
                if (currentlinetext.ToUpper().Contains("TOTAL") ||
                    currentlinetext.ToUpper().Contains("SUBTOTAL") ||
                    currentlinetext.ToUpper().Contains("SALE AMOUNT") ||
                    currentlinetext.ToUpper().Contains("CASH") ||
                    currentlinetext.ToUpper().Contains("CREDIT") ||
                    currentlinetext.ToUpper().Contains("PAID") ||
                    currentlinetext.ToUpper().Contains("EFT") ||
                    currentlinetext.ToUpper().Contains("EFTPOS") ||
                    currentlinetext.ToUpper().Contains("AMOUNT DUE")
                    )
                {
                    var extractedMoneyString = ExtractMoney(currentlinetext);
                    var extractedMoney = 0.00M;
                    if (extractedMoneyString != "")
                    {
                        decimal.TryParse(extractedMoneyString, out extractedMoney);
                        if (extractedMoney > receipttotal) { receipttotal = extractedMoney; }
                    }
                }

                var moneyString = ExtractMoney(currentlinetext);
                var money = 0.00M;
                if (moneyString != "")
                {
                    decimal.TryParse(moneyString, out money);
                    if (money > 0 && currentlinetext.Contains("$")) { amounts.Add(money); }
                }

            }
            amounts = amounts.OrderByDescending(v => v).ToList();
            if (abn != "") { receipt.abn = abn; }
            if (receiptdate != new DateTime(1900, 01, 01)) { receipt.receiptdate = receiptdate.ToString("dd-MMM-yyyy"); }
            if (receipttotal != 0) { receipt.receipttotal = receipttotal.ToString(); }

            var maxAmount = amounts.FirstOrDefault();

            if (receipttotal < maxAmount) { receipt.receipttotal = maxAmount.ToString(); }

            // send our final processed receipt object back
            return receipt;
        }

        // Returns the last valid money string found in the line
        static string ExtractMoney(string line)
        {

            string moneystring = "";
            decimal money = 0.00M;
            string pat = @"[0-9]+( ?\. ?[0-9][0-9])"; // also match $21 .80 or 21. 80 (with space)

            foreach (Match match in Regex.Matches(line, pat))
            {
                Decimal extractedMoney = 0.00M;
                moneystring = match.Value.Replace(" ", "");
                Decimal.TryParse(moneystring, out extractedMoney);
                if (extractedMoney > money) { money = extractedMoney; }
            }
            return money.ToString(); //trim spaces from string before returning: fixes 21 .80
        }

        // Returns last valid date string found in the line
        static string ExtractDate(string line)
        {
            string receiptdate = "";
            // match dates "01/05/2018" "01-05-2018" "01-05-18" "01 05 18" "01 05 2018"
            string pat = @"\s*((31([-/ .])((0?[13578])|(1[02]))\3(\d\d)?\d\d)|((([012]?[1-9])|([123]0))([-/ .])((0?[13-9])|(1[0-2]))\12(\d\d)?\d\d)|(((2[0-8])|(1[0-9])|(0?[1-9]))([-/ .])0?2\22(\d\d)?\d\d)|(29([-/ .])0?2\25(((\d\d)?(([2468][048])|([13579][26])|(0[48])))|((([02468][048])|([13579][26]))00))))\s*";
            foreach (Match match in Regex.Matches(line, pat))
            {
                receiptdate = match.Value.Trim();
                receiptdate = receiptdate.Replace("-", "/");
                receiptdate = receiptdate.Replace(".", "/");
                receiptdate = receiptdate.Replace(" ", "/");
            }

            // didnt find date?  now we'll try searching with month names.  03 OCT 2017, 03 October 2017 etc
            if (receiptdate == "")
            {
                pat = @"((31(?![-/ .](Feb(ruary)?|Apr(il)?|June?|(Sep(?=\b|t)t?|Nov)(ember)?)))|((30|29)(?![-/ .]Feb(ruary)?))|(29(?=[-/ .]Feb(ruary)?[-/ .](((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))|(0?[1-9])|1\d|2[0-8])[-/ .](Jan(uary)?|Feb(ruary)?|Ma(r(ch)?|y)|Apr(il)?|Ju((ly?)|(ne?))|Aug(ust)?|Oct(ober)?|(Sep(?=\b|t)t?|Nov|Dec)(ember)?)[-/ .]((1[6-9]|[2-9]\d)\d{2})";

                foreach (Match match in Regex.Matches(line, pat, RegexOptions.IgnoreCase))
                {
                    receiptdate = match.Value.Trim();
                    receiptdate = receiptdate.Replace("/", "-");
                    receiptdate = receiptdate.Replace(".", "-");
                    receiptdate = receiptdate.Replace(" ", "-");
                }
            }

            return receiptdate;
        }

        // Returns the last #valid# ABN number found in the line

        static string ExtractABN(string line)
        {

            // search line for ABN
            // could be 11 digit block, or 11 digits with spaces
            // e.g. 44418573722 or 44 418 573 722
            // only return if passes checksum validation via ValidateABN()
            // 

            string abn = "";
            string pat = @"(\d *?){11}";

            Regex regexObj = new Regex(pat);
            Match matchObj = regexObj.Match(line);
            while (matchObj.Success)
            {
                string extractedABN = matchObj.Value.Replace(" ", "");
                if (ValidateABN(extractedABN)) { abn = extractedABN; }
                matchObj = regexObj.Match(line, matchObj.Index + 1);
            }

            return abn;
        }




        static string ExtractBpay(ReceiptLine line, List<ReceiptLine> lines)
        {
            var items = lines.Where(l => l.text.ToUpper().Contains("BILLER")).ToList();


            List<int> list = items.Select(l => l.x).ToList();
            int number = line.x;

            int closest = list.Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);

            var item = items.Where(l => l.x == closest).FirstOrDefault();

            string bpayLine = item.text;

            return bpayLine;
        }
        // Validates if an 11 digit number could be an ABN
        //1. Subtract 1 from the first (left) digit to give a new eleven digit number         
        //2. Multiply each of the digits in this new number by its weighting factor         
        //3. Sum the resulting 11 products         
        //4. Divide the total by 89, noting the remainder         
        //5. If the remainder is zero the number is valid          
        static bool ValidateABN(string abn)
        {
            bool isValid = true; int[] weight = { 10, 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 }; int weightedSum = 0;
            //0. ABN must be 11 digits long                         
            if (isValid &= (!string.IsNullOrEmpty(abn) && Regex.IsMatch(abn, @"^\d{11}$")))
            {
                //Rules: 1,2,3                                  
                for (int i = 0; i < weight.Length; i++) { weightedSum += (int.Parse(abn[i].ToString()) - ((i == 0) ? 1 : 0)) * weight[i]; }
                //Rules: 4,5                 
                isValid &= ((weightedSum % 89) == 0);
            }
            return isValid;
        }
    }


    public class Receipt
    {
        [JsonIgnore]
        public string language { get; set; }
        [JsonIgnore]
        public float textAngle { get; set; }
        [JsonIgnore]
        public string orientation { get; set; }

        public string abn { get; set; }
        [JsonIgnore]
        public string businessname { get; set; }
        public string biller { get; set; }
        public string billerRef { get; set; }
        [JsonIgnore]
        public string receiptdate { get; set; }
        [JsonIgnore]
        public string taxtotal { get; set; }
        public string receipttotal { get; set; }
        [JsonIgnore]
        public List<ReceiptLine> lines;
    }

    public class ReceiptLine
    {
        public string boundingBox { get; set; }
        public int x;
        public int y;
        public int width;
        public int height;
        public string text { get; set; }
    }

    public class ImageRequest
    {
        public string content { get; set; }
    }


    public class OCRVisionResponse
    {
        public string language { get; set; }
        public float textAngle { get; set; }
        public string orientation { get; set; }
        public Region[] regions { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public Line[] lines { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public Word[] words { get; set; }
    }

    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }

    public class Secrets
    {
        public const string apikey = "f6b384f96e5b4e3dbe6c87bc9845bea8";
        public const string apiendpoint_ocr = @"https://ocrfinstro.cognitiveservices.azure.com/vision/v2.0/ocr";
    }
}
