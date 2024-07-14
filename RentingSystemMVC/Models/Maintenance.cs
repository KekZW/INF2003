using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentingSystemMVC.Models
{
    public class Maintenance
    {
        [BsonElement("MaintenanceID")]
        public int MaintenanceID { get; set; }

        public int VehicleID { get; set; }

        [BsonElement("WorkshopStatus")]
        public string WorkshopStatus { get; set; }

        [BsonElement("startMaintDate")]
        public DateOnly startMaintDate { get; set; }

        [BsonElement("endMaintDate")]
        public DateOnly endMaintDate { get; set; }
        
        // Navigation Property
        public Vehicle Vehicle { get; set; }
    }

    public class MaintenanceRecords
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = null;

        [BsonElement("vehicleID")]
        public int vehicleID { get; set; }

        [BsonElement("maintenanceRecords")]
        public List<Maintenance> maintenanceRecords { get; set; } = new List<Maintenance>();
    }
}
