using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;


namespace RentingSystemMVC.Models;

public class Review
{
public int userid { get; set; }
public int rating { get; set; }
public string comments { get; set; }
}

public class VehicleReview
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } = null;

    public int vehicleID { get; set; }

    public List<Review> reviews { get; set; } = new List<Review>(); 
}
