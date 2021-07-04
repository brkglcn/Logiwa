using System;
using System.Threading.Tasks;
using Logiwa.Models.Requests;
using Logiwa.Services.Logiwa;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;

namespace Logiwa.Controllers
{
    [Route("logiwa")]
    [ApiController]
    public class LogiwaController : Controller
    {
        ILogiwaService _iLogiwaService;
        public LogiwaController(ILogiwaService LogiwaService)
        {
            _iLogiwaService = LogiwaService;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                return Json(await _iLogiwaService.GetProducts());
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost("create-product")]
        public async Task<ActionResult> CreateProduct([FromBody]CreateProductRequest request)
        {
            try
            {
                return new CreatedResult("",await _iLogiwaService.CreateProduct(request));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost("search-product")]
        public async Task<IActionResult> SearchProduct([FromBody]SearchProductRequest request)
        {
            try
            {
                return Json(await _iLogiwaService.SearchProduct(request));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost("delete-product")]
        public async Task<ActionResult> DeleteById([FromBody]DeleteProductRequest request)
        {
            ObjectId idValue;
            if (string.IsNullOrWhiteSpace(request?.id) || !ObjectId.TryParse(request.id, out idValue))
                return BadRequest();

            try
            {
                return new OkObjectResult(await _iLogiwaService.DeleteById(idValue));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPut("update-product")]
        public async Task<ActionResult> UpdateProduct([FromBody]UpdateProductRequest request)
        {
            try
            {
                return new OkObjectResult(await _iLogiwaService.UpdateProduct(request));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpGet("products-by-quantity")]
        public async Task<IActionResult> GetProductsByQuantity([FromQuery(Name = "order")] string order, [FromQuery(Name = "min")] int min, [FromQuery(Name = "max")] int max)
        {
            try
            {
                return Json(await _iLogiwaService.GetProductsByQuantity(order,min,max));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost("create-category")]
        public async Task<ActionResult> CreateCategory([FromBody]CreateCategoryRequest request)
        {
            try
            {
                return new CreatedResult("", await _iLogiwaService.CreateCategory(request));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
    }
}