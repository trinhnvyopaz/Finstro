using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Finstro.Serverless.Models;
using ServiceStack;
using ServiceStack.Aws;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Finstro.Serverless.Helper
{
    public class AwsHelper
    {
        private static readonly RegionEndpoint _region = RegionEndpoint.USEast2;


        public static async Task RemoveFile(AmazonS3Client client, string fileName)
        {

            string bucketName = AppSettings.AwsSettings.FinstroBucketName;

            fileName = new Uri(fileName).PathAndQuery.Replace("/", "");

            var result = await client.DeleteObjectAsync(new DeleteObjectRequest()
            {
                BucketName = bucketName,
                Key = fileName
            });

            Console.Write(result.ToString());

        }

        public static async Task RemoveFile(string fileName)
        {
            using (AmazonS3Client client = new AmazonS3Client(_region))
            {
                await RemoveFile(client, fileName);
            }

        }


        public static string UploadFiles(string folder, EnumIdFileType fileType, byte[] file, string extension)
        {

            string mimeType = string.Empty;

            switch (extension.ToLower())
            {
                case "jpg":
                case "jpeg":
                    mimeType = MimeTypes.ImageJpg;
                    break;
                case "png":
                    mimeType = MimeTypes.ImagePng;
                    break;
                case "xml":
                    mimeType = MimeTypes.Xml;
                    break;
                case "pdf":
                    mimeType = "application/pdf";
                    break;
                case "html":
                    mimeType = MimeTypes.Html;
                    break;
                case "json":
                    mimeType = MimeTypes.Json;
                    break;
                default:
                    mimeType = "application/octet-stream";
                    break;
            }

            using (AmazonS3Client client = new AmazonS3Client(_region))
            {

                string bucketName = AppSettings.AwsSettings.FinstroBucketName;

                string fileName = $"{folder}/{Guid.NewGuid().ToString()}_{fileType.ToString()}.{extension}";

                try
                {
                    var upload = client.PutObject(new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = fileName,
                        ContentType = mimeType,
                        InputStream = new MemoryStream(file),
                        StorageClass = S3StorageClass.Standard,
                        CannedACL = S3CannedACL.PublicRead

                    });

                    string url = $"https://{bucketName}.s3.{_region.SystemName}.amazonaws.com/{fileName}";

                    return url;
                }
                catch (Exception ex)
                {
                    var error = FinstroErrorType.Schema.FileNotFound;
                    error.Description = ex.Message;
                    throw error;
                }
            }
        }

    }
}
