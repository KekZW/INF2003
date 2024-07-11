using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RentingSystemMVC.Models;

public class Support
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; }
    
    [BsonElement("creationDate")]
    public DateTime creationDate { get; set; }
    
    [BsonElement("updatedDate")]
    public DateTime updatedDate { get; set; }
    
    [BsonElement("userID")]
    public int userID { get; set; }
    
    [BsonElement("status")]
    public string status { get; set; }
    
    [BsonElement("email")]
    public string email { get; set; }
    
    [BsonElement("subject")]
    public string subject { get; set; }
    
    [BsonElement("description")]
    public string description { get; set; }
    
    [BsonElement("comments")]
    public List<Comment> comments { get; set; } = new List<Comment>();
}

public class Comment
{
    [BsonElement("comment_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId comment_id { get; set; }
    
    [BsonElement("comment_date")]
    public DateTime comment_date { get; set; }

    [BsonElement("admin_id")]
    public int admin_id { get; set; }

    [BsonElement("comment_text")]
    public string comment_text { get; set; }
}