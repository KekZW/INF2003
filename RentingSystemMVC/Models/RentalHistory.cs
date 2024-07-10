using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using RentingSystemMVC.Models;


namespace RentingSystemMVC.Models;

public class RentalHistory
{   
    [BsonElement("rentalID")]
    public int rentalID { get; set; }

    [BsonElement("userID")]
    public int userID { get; set; }

    [BsonElement("name")]
    public string name { get; set; }

    [BsonElement("startRentalDate")]
    public DateTime startRentalDate { get; set; }

    [BsonElement("endRentalDate")]
    public DateTime endRentalDate { get; set; }

    [BsonElement("status")]
    public string status { get; set; }
}

public class RentalHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } = null;

    [BsonElement("vehicleID")]
    public int vehicleID { get; set; }

    public List<RentalHistory> historys { get; set; } = new List<RentalHistory>();
}
