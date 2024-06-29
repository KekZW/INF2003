using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentingSystemMVC.Models
{
    public class VehicleReview
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Make")]
        public string Make { get; set; }

        [BsonElement("Model")]
        public string Model { get; set; }

        [BsonElement("Year")]
        public int Year { get; set; }

        [BsonElement("Color")]
        public string Color { get; set; }

        [BsonElement("LicensePlate")]
        public string LicensePlate { get; set; }

        // Add other properties as needed
    }
}