using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Finstro.Serverless.Helper
{
    public static class Common
    {
        public static string GetRandomString(int lenOfTheNewStr)
        {
            string output = string.Empty;
            while (true)
            {
                output = output + Path.GetRandomFileName().Replace(".", string.Empty);
                if (output.Length > lenOfTheNewStr)
                {
                    output = output.Substring(0, lenOfTheNewStr);
                    break;
                }
            }
            return output;
        }


        public static string GetTransactionAutoNumber(int lenOfTheNewStr)
        {
            const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random rng = new Random();
            return rng.NextStrings(AllowedChars, (lenOfTheNewStr, lenOfTheNewStr), 1).FirstOrDefault();
            
        }

        private static IEnumerable<string> NextStrings(this Random rnd, string allowedChars, (int Min, int Max) length, int count)
        {
            ISet<string> usedRandomStrings = new HashSet<string>();
            (int min, int max) = length;
            char[] chars = new char[max];
            int setLength = allowedChars.Length;

            while (count-- > 0)
            {
                int stringLength = rnd.Next(min, max + 1);

                for (int i = 0; i < stringLength; ++i)
                {
                    chars[i] = allowedChars[rnd.Next(setLength)];
                }

                string randomString = new string(chars, 0, stringLength);

                if (usedRandomStrings.Add(randomString))
                {
                    yield return randomString;
                }
                else
                {
                    count++;
                }
            }
        }

    }
}
