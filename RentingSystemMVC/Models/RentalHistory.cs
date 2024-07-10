using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace RentingSystemMVC.Models
{

    public class History
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

        [BsonElement("History")]
        public List<History> History { get; set; } = new List<History>();
    }
}