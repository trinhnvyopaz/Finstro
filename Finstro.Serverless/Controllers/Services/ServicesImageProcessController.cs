using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using Finstro.Serverless.Helper;

namespace Finstro.Serverless.API.Controllers
{
    public class ServicesImageProcessController : Controller
    {
       
        [Route("api/Services/Image/Base64")]
        [HttpPost]
        [FisntroServiceCall]
        [RequestFormSizeLimit(valueCountLimit: 200000)]

        public async Task<JsonResult> Post([FromBody]ImageRequest content)
        {
            OCRProcessing ocr = new OCRProcessing();

            byte[] bytes = Convert.FromBase64String(content.content);

            var data = await ocr.ProcessReceiptImage(bytes);
            return Json(new { Amount = data.receipttotal });
        }
    }
}
