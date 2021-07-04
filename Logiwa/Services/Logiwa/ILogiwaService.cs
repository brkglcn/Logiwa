using Logiwa.Models.Requests;
using Logiwa.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logiwa.Services.Logiwa
{
    public interface ILogiwaService
    {
        Task<GetAllProductsResponse[]> GetProducts();
        Task<ActionResult> CreateProduct(CreateProductRequest request);
        Task<SearchedProductsResponse[]> SearchProduct(SearchProductRequest request);
        Task<ActionResult> DeleteById(ObjectId request);
        Task<ActionResult> UpdateProduct(UpdateProductRequest request);
        Task<GetAllProductsResponse[]> GetProductsByQuantity(string order,int min,int max);
        Task<ActionResult> CreateCategory(CreateCategoryRequest request);

    }
}
