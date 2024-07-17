using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentingSystemMVC.Models;

public class Records {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    [BsonElement("startMaintDate")]
    public DateTime startMaintDate { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    [BsonElement("endMaintDate")]
    public DateTime endMaintDate { get; set; }
    
    [BsonElement("status")]
    public string? status {get; set;}
}


public class MaintenanceRecords
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } = null;

    [BsonElement("vehicleID")]
    public int vehicleID { get; set; }

    [BsonElement("records")]
    public List<Maintenance> maintenanceRecords { get; set; } = new List<Maintenance>();
}