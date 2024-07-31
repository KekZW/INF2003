using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;


namespace RentingSystemMVC.Controllers
{
    public class PromotionController : Controller
    {
        private readonly MongoDBContext _mongoContext;
        private readonly ILogger<PromotionController> _logger;
        private static readonly FilterDefinitionBuilder<Promotion> filterBuilder = Builders<Promotion>.Filter;

        public PromotionController(MongoDBContext mongoContext, ILogger<PromotionController> logger)
        {
            _mongoContext = mongoContext;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetPromotion(string promotionCode)
        {
            try
            {
                var currentDate = DateTime.Now;
                var filter = filterBuilder.Eq("promotionCode", promotionCode);
                var dateFilter = filterBuilder.Gte("ExpiryDate", currentDate);
                var combinedFilter = filterBuilder.And(filter,dateFilter);
                var promotion = _mongoContext.Promotion.Find(combinedFilter).FirstOrDefault();

                if (promotion == null) return BadRequest("promotion not found");


                return Json(new { promotion.discountRate }); // Return promotion as JSON
            }
            catch (Exception ex)
            {
                return BadRequest("promotion not found");
            }
        }

        [HttpGet]
        [Authorize(Roles="Admin")]
        public IActionResult Manage(){

            var currentDate = DateTime.Now;
            var filter = filterBuilder.Gte("ExpiryDate",currentDate);

            var promotions = _mongoContext.Promotion.Find(filter).ToList();
            return View(promotions);
        }

        [HttpPost]
        [Authorize(Roles="Admin")]
        public IActionResult postPromotions(string promotionCode, DateTime ExpiryDate, int discountRate){
        try
        {   
          if (ModelState.IsValid)
            {
                var existingPromoCode = _mongoContext.Promotion.Find(filterBuilder.Eq("promotionCode",promotionCode)).FirstOrDefault();
            
                if (existingPromoCode != null) return BadRequest(ModelState);
                
                TimeZoneInfo sgTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                // Convert UTC to your local time zone (+8 hours)
                ExpiryDate = TimeZoneInfo.ConvertTimeFromUtc(ExpiryDate, sgTimeZone);

                Promotion promotion = new Promotion {
                    promotionCode = promotionCode,
                    ExpiryDate = ExpiryDate.ToUniversalTime(),
                    discountRate = discountRate,
                };
                _mongoContext.Promotion.InsertOne(promotion);
                return Json(new { Message = "Promotion added successfully." } );
            }
            else
            {
                return BadRequest(ModelState); // Return validation errors if ModelState is invalid
            }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                _logger.LogError(ex, "Error occurred while adding promotion");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult deletePromotions(string promotionCode)
        {
            try
            {
                if (string.IsNullOrEmpty(promotionCode))
                {
                    return BadRequest(new { Message = "Promotion code is required." });
                }
                {
                    var filter = Builders<Promotion>.Filter.Eq(p => p.promotionCode, promotionCode);
                    var result = _mongoContext.Promotion.Find(filter).FirstOrDefault();

                    if (result == null)
                    {
                        return NotFound(new { Message = "Promotion code not found." });
                    }

                    _mongoContext.Promotion.DeleteOne(filter);
                    return Json(new { Message = "Promotion deleted successfully." });
                }
                
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                _logger.LogError(ex, "Error occurred while adding promotion");
                return StatusCode(500, "Internal Server Error");
            }
        }
       

    }

}