using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
namespace RentingSystemMVC.Models

{
    public class Promotion
    {   
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("promotionCode")]
        public string promotionCode { get; set; }

        [BsonElement("ExpiryDate")]
        public DateTime ExpiryDate { get; set; }

        [BsonElement("discountRate")]
        public int discountRate { get; set; }
    }
}
