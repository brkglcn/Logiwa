using Logiwa.Models.Requests;
using Logiwa.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Logiwa.Services.Logiwa
{
    public class LogiwaService : ILogiwaService
    {
        public LogiwaService()
        {
        }

        async Task<GetAllProductsResponse[]> ILogiwaService.GetProducts()
        {
            using (var connection = new Factory.Database.MongoConnection("Logiwa"))
            {
                var collection = connection.GetCollection<Models.MongoEntities.Products>(Models.MongoEntities.Products.COLLECTIONNAME, null);
                var collectionCategory = connection.GetCollection<Models.MongoEntities.Categories>(Models.MongoEntities.Categories.COLLECTIONNAME, null);
                var products = await collection.Find(_ => true).ToListAsync();
                var categories = await collectionCategory.Find(_ => true).ToListAsync();
                var response = products.Select(x => new GetAllProductsResponse
                {
                    Title = x.Title,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    CategoryId = categories.Find(y => y.CategoryId == x.CategoryId).Name
                }).ToArray();
                return response;
            }
        }

        async Task<ActionResult> ILogiwaService.CreateProduct(CreateProductRequest request)
        {
            
            if (request.Title.Count() == 0)
            {
                return new BadRequestObjectResult("Title cannot be null.");
            }
            if (request.Title.Count() > 200)
            {
                return new BadRequestObjectResult("Title cannot exceed 200 characters.");
            }
            if (String.IsNullOrEmpty(request.CategoryId.ToString()))
            {
                return new BadRequestObjectResult("Category cannot be null.");
            }
            if (request.Quantity <= 0)
            {
                return new BadRequestObjectResult("Quantity cannot be 0 or lower");
            }

            using (var connection = new Factory.Database.MongoConnection("Logiwa"))
            {
                var collection = connection.GetCollection<Models.MongoEntities.Products>(Models.MongoEntities.Products.COLLECTIONNAME, null);
                var collectionCategory = connection.GetCollection<Models.MongoEntities.Categories>(Models.MongoEntities.Categories.COLLECTIONNAME, null);
                var categories = await collectionCategory.Find(_ => true).ToListAsync();
                var categoryCheck = categories.Where(x => x.CategoryId.ToString() == request.CategoryId).Count();
                if (categoryCheck == 0)
                {
                    return new BadRequestObjectResult("Category not found.");
                }
                await collection.InsertOneAsync(new Models.MongoEntities.Products
                {
                     Title = request.Title,
                     Description = request.Description,
                     Quantity = request.Quantity,
                     CategoryId = ObjectId.Parse(request.CategoryId)
                });
                return new CreatedResult("","");
            }
        }

        async Task<SearchedProductsResponse[]> ILogiwaService.SearchProduct(SearchProductRequest request)
        {
            var properSeachValue = request.SearchString.ToUpper(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"));
            using (var connection = new Factory.Database.MongoConnection("Logiwa"))
            {
                var collection = connection.GetCollection<Models.MongoEntities.Products>(Models.MongoEntities.Products.COLLECTIONNAME, null);
                var collectionCategory = connection.GetCollection<Models.MongoEntities.Categories>(Models.MongoEntities.Categories.COLLECTIONNAME, null);
                var allProducts = await collection.Find(_ => true).ToListAsync();
                var allCategories = await collectionCategory.Find(_ => true).ToListAsync();
                var products = await collection.Find(p => p.Title.ToUpper().Contains(properSeachValue) || p.Description.ToUpper().Contains(properSeachValue)).ToListAsync();
                var categories = await collectionCategory.Find(p => p.Name.ToUpper().Contains(properSeachValue)).ToListAsync();
                
                var datas = new List<SearchedProductsResponse>();

                
                foreach (var item in products)
                {
                    datas.Add(new SearchedProductsResponse
                    {
                        Title = item.Title,
                        Description  = item.Description,
                        Quantity = item.Quantity,
                        Category = allCategories.Find(y => y.CategoryId == item.CategoryId).Name
                    });
                }
                if (categories.Count() > 0)
                {
                    foreach (var cats in categories)
                    {
                        var foundProducts = allProducts.Where(x => x.CategoryId == cats.CategoryId).Select(x => new { x.Title,x.Quantity,x.Description }).ToList();
                        foreach (var item in foundProducts)
                        {
                            datas.Add(new SearchedProductsResponse
                            {
                                Title = item.Title,
                                Description = item.Description,
                                Quantity = item.Quantity,
                                Category = cats.Name
                            });
                        }
                        
                    }
                }

                return datas.ToArray();
            }
        }

        async Task<ActionResult> ILogiwaService.DeleteById(ObjectId id)
        {
            using (var connection = new Factory.Database.MongoConnection("Logiwa"))
            {
                var collection = connection.GetCollection<Models.MongoEntities.Products>(Models.MongoEntities.Products.COLLECTIONNAME, null);
                if (id != null)
                    await collection.DeleteOneAsync(a => a.Id == id);

                return new OkResult();
            }
        }

        async Task<ActionResult> ILogiwaService.UpdateProduct(UpdateProductRequest request)
        {
            ObjectId idValue;
            if (string.IsNullOrWhiteSpace(request?.id) || !ObjectId.TryParse(request.id, out idValue))
                return new BadRequestResult();

            if (request.Title.Count() == 0)
            {
                return new BadRequestObjectResult("Title cannot be null.");
            }
            if (request.Title.Count() > 200)
            {
                return new BadRequestObjectResult("Title cannot exceed 200 characters.");
            }
            if (String.IsNullOrEmpty(request.CategoryId))
            {
                return new BadRequestObjectResult("Category cannot be null.");
            }
            if (request.Quantity <= 0)
            {
                return new BadRequestObjectResult("Quantity cannot be 0 or lower");
            }

            using (var connection = new Factory.Database.MongoConnection("Logiwa"))
            {
                var collectionCategory = connection.GetCollection<Models.MongoEntities.Categories>(Models.MongoEntities.Categories.COLLECTIONNAME, null);
                var categories = await collectionCategory.Find(_ => true).ToListAsync();
                var categoryCheck = categories.Where(x => x.CategoryId.ToString() == request.CategoryId).Count();
                if (categoryCheck == 0)
                {
                    return new BadRequestObjectResult("Category not found.");
                }
                await connection.UpdateByRecordIdAsync(Models.MongoEntities.Products.COLLECTIONNAME, null, idValue,
                Builders<Models.MongoEntities.Products>.Update
                    .Set(p => p.Title, request.Title)
                    .Set(p => p.Description, request.Description)
                    .Set(p => p.Quantity, request.Quantity)
                    .Set(p => p.CategoryId, ObjectId.Parse(request.CategoryId)));

                return new OkResult();
            }
        }

        async Task<GetAllProductsResponse[]> ILogiwaService.GetProductsByQuantity(string order,int min,int max)
        {
            
            using (var connection = new Factory.Database.MongoConnection("Logiwa"))
            {
                var collection = connection.GetCollection<Models.MongoEntities.Products>(Models.MongoEntities.Products.COLLECTIONNAME, null);
                var collectionCategory = connection.GetCollection<Models.MongoEntities.Categories>(Models.MongoEntities.Categories.COLLECTIONNAME, null);
                var products = await collection.Find(_ => true).ToListAsync();
                var categories = await collectionCategory.Find(_ => true).ToListAsync();
                var response = products.Where(x => x.Quantity >= min && x.Quantity <= max).Select(x => new GetAllProductsResponse
                {
                    Title = x.Title,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    CategoryId = categories.Find(y => y.CategoryId == x.CategoryId).Name
                });

                response = order == "desc" ? response.OrderBy(x => x.Quantity) : response.OrderByDescending(x => x.Quantity);
                return response.ToArray();
            }
        }

        async Task<ActionResult> ILogiwaService.CreateCategory(CreateCategoryRequest request)
        {

            if (string.IsNullOrEmpty(request.Name))
            {
                return new BadRequestObjectResult("Name cannot be null.");
            }

            using (var connection = new Factory.Database.MongoConnection("Logiwa"))
            {
                var collectionCategory = connection.GetCollection<Models.MongoEntities.Categories>(Models.MongoEntities.Categories.COLLECTIONNAME, null);
                var categories = await collectionCategory.Find(_ => true).ToListAsync();
                await collectionCategory.InsertOneAsync(new Models.MongoEntities.Categories
                {
                    Name = request.Name,
                    CategoryId = ObjectId.GenerateNewId()
            });
                return new CreatedResult("", "");
            }
        }
    }
}
