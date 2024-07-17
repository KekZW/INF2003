using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace RentingSystemMVC.Models;

public class MaintenanceRecords
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }

    [BsonElement("vehicleID")]
    public int vehicleID { get; set; }

    [BsonElement("records")]
    public List<Maintenance> records { get; set; } = new List<Maintenance>();
}


public class Records {

    [BsonElement("startMaintDate")]
    public DateOnly startMaintDate { get; set; }
    [BsonElement("endMaintDate")]
    public DateOnly endMaintDate { get; set; }
    [BsonElement("Status")]
    public string? status { get; set; }
}