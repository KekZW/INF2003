using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentingSystemMVC.Models
{
    public class Maintenance
    {
        public int MaintenanceID { get; set; }

        public int VehicleID { get; set; }

        public string WorkshopStatus { get; set; }

        public DateTime startMaintDate { get; set; }

        public DateTime endMaintDate { get; set; }
        
        // Navigation Property
        public Vehicle Vehicle { get; set; }
    }
}
