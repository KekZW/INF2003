using RentingSystemMVC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace RentingSystemMVC.Data;
public class MongoDBContext
{
    private readonly IMongoDatabase mongoDatabase;

    public MongoDBContext(
string connectionString, IOptions<DatabaseSettings> DatabaseSettings)
    {
        var mongoClient = new MongoClient(
            DatabaseSettings.Value.ConnectionString);

        mongoDatabase = mongoClient.GetDatabase(
            DatabaseSettings.Value.DatabaseName);

    }

    // Define your collections (tables) here
    public IMongoCollection<VehicleReview> VehicleReview =>
        mongoDatabase.GetCollection<VehicleReview>("vehicleReview");

    public IMongoCollection<Promotion> Promotion => 
        mongoDatabase.GetCollection<Promotion>("promotion");
    
    public IMongoCollection<Support> Support => 
        mongoDatabase.GetCollection<Support>("support");

    public IMongoCollection<RentalHistory> RentalHistory =>
        mongoDatabase.GetCollection<RentalHistory>("RentalHistory");

    public IMongoCollection<MaintenanceRecords> MaintenanceRecords =>
        mongoDatabase.GetCollection<MaintenanceRecords>("MaintenanceRecords");

    public void checkCollectionExists<T>(string collectionName){
        var collectionNames = mongoDatabase.ListCollectionNames().ToList();

        if (!collectionNames.Contains(collectionName)){
            mongoDatabase.CreateCollection(collectionName);
        }
    }

    public void EnsureAllCollectionsExist()
    {
        checkCollectionExists<VehicleReview>("vehicleReview");
        checkCollectionExists<Promotion>("promotion");
        checkCollectionExists<Support>("support");
        checkCollectionExists<RentalHistory>("RentalHistory");
        checkCollectionExists<MaintenanceRecords>("MaintenanceRecords");
    }

    internal void EnsureCollectionsExist()
    {
        throw new NotImplementedException();
    }
}
