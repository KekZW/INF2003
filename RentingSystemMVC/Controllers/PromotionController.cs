using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;
using System;

namespace RentingSystemMVC.Controllers
{
    public class PromotionController : Controller
    {
        private readonly MongoDBContext _mongoContext;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(MongoDBContext mongoContext, ILogger<PromotionController> logger)
        {
            _mongoContext = mongoContext;
            _logger = logger;
        }

        public ActionResult GetPromotion(string promotionCode)
        {
            try
            {
                var filterBuilder = Builders<Promotion>.Filter;
                var filter = filterBuilder.Eq("promotionCode", promotionCode);
                var dateFilter = Builders<Promotion>.Filter.Gte("ExpiryDate", new DateTime());
                var combinedFilter = filterBuilder.And(filter,dateFilter);
                var promotion = _mongoContext.Promotion.Find(combinedFilter).FirstOrDefault();

                if (promotion == null) return BadRequest("promotion not found");


                return Json(new { promotion }); // Return promotion as JSON
            }
            catch (Exception ex)
            {
                return BadRequest("promotion not found");
            }
        }
    }
}