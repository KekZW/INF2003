using RentingSystemMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace RentingSystemMVC.Data;


public class MongoDBContext
{
    private readonly IMongoDatabase _rentalCollection;

    public MongoDBContext(
        IOptions<DatabaseSettings> DatabaseSettings)
    {
        var mongoClient = new MongoClient(
            DatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            DatabaseSettings.Value.DatabaseName);

    }

    // Define your collections (tables) here
    public IMongoCollection<VehicleReview> VehicleReview =>
        _rentalCollection.GetCollection<VehicleReview>("vehicleReview");

}
