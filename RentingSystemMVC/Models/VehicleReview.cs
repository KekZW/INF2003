using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;


namespace RentingSystemMVC.Models;

public class Review
{   
    [BsonElement("name")]
    public string name { get; set; }

    [BsonElement("rating")]
    public int rating { get; set; }

    [BsonElement("comment")]
    public string comment { get; set; }
}

public class VehicleReview
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } = null;

    [BsonElement("vehicleID")]
    public int vehicleID { get; set; }

    [BsonElement("reviews")]
    public List<Review> reviews { get; set; } = new List<Review>(); 
}
