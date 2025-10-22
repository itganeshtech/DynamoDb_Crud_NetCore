using Amazon.DynamoDBv2.DataModel;

namespace ProductDIExample.Models
{
    [DynamoDBTable("Products")] // DynamoDB table name
    public class Product
    {
        [DynamoDBHashKey] // Partition key
        public string Id { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public double Price { get; set; }
    }
}