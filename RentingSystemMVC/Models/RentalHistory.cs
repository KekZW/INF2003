using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using RentingSystemMVC.Models;
using Org.BouncyCastle.Utilities.Collections;

namespace RentingSystemMVC.Models
{
    public class RentalHistory
    {
        public int rentalID { get; set; }
        public int userid { get; set; }
        public string name { get; set; }
        public DateTime startRentalDate { get; set; }
        public DateTime endRentalDate { get; set; }
    }
}

public class RentalHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } = null;
    public int vehicleID { get; set; }
    public List<RentalHistory> historys { get; set; } = new List<RentalHistory>();
}




