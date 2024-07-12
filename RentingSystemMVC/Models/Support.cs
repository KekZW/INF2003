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
    
    // NEW
    [BsonElement("assigned_to")]
    public int? assigned_to { get; set; }
    
    [BsonElement("comments")]
    public List<Comment> comments { get; set; } = new List<Comment>();
    
    [BsonIgnore]
    public User user { get; set; }
}

public class Comment
{
    // NEW
    [BsonElement("comment_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId comment_id { get; set; }
    
    [BsonElement("comment_date")]
    public DateTime comment_date { get; set; }

    [BsonElement("user_id")]
    public int user_id { get; set; }

    [BsonElement("comment_text")]
    public string comment_text { get; set; }
    
    [BsonIgnore]
    public User user { get; set; }
}