using System;
using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.Caching;
using ServiceStack.Configuration;

namespace Finstro.Serverless.DynamoDB
{
    public abstract class DynamoBase
    {

        private static readonly RegionEndpoint _region = RegionEndpoint.USEast2;
        public static bool UseLocalDb = true;

        public static IPocoDynamo CreatePocoDynamo()
        {
            var dynamoClient = CreateDynamoDbClient();

            var db = new PocoDynamo(dynamoClient);
            return db;
        }

        public static string DynamoDbUrl = Helper.AppSettings.AwsSettings.DynamoUrl;

        public static ICacheClient CreateCacheClient()
        {
            var cache = new DynamoDbCacheClient(CreatePocoDynamo());
            cache.InitSchema();
            return cache;
        }

        public static AmazonDynamoDBClient CreateDynamoDbClient()
        {
            var dynamoClient = new AmazonDynamoDBClient(_region); 
            return dynamoClient;
        }

    }


}
