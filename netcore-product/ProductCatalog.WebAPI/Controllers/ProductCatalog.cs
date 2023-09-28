using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.WebAPI.Models;
using ProductCatalog.WebAPI.Services;

namespace ProductCatalog.WebAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        


        /*
        Post api/products: This endpoint allows adding a new product, asynchronously invoking _productService.AddProduct, 
        and returning an HTTP 201 Created status.
        */

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                await _productService.AddProduct(product);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error - " + ex.Message);
            }

        }

        
        
        /*
         GET api/products: When a client sends a GET request to this endpoint, it asynchronously retrieves a list of products using the _productService, 
        and then returns those products as a response with an HTTP status code indicating success (200 OK).
        */

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error - " + ex.Message);
            }
        }
        
        /*
        GET api/products/{id}: This endpoint that handles HTTP GET requests with a specific "id" parameter, retrieving a product by its unique identifier asynchronously. 
        If the product is found, it returns it with an HTTP 200 OK status; otherwise, it returns an HTTP 404 Not Found status.
        */

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if(product == null) return NotFound();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error - " + ex.Message);
            }
        }
        
        /*
        GET api/products/search: This endpoint accepts a "name" query parameter, asynchronously retrieves products with matching names 
        using _productService.GetProductsByName, and returns them in an HTTP response.
        */

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetProductsByName([FromQuery] string name)
        {
            try
            {
                var products = await _productService.GetProductsByName(name);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error - " + ex.Message);
            }

        }
        
        /*
        GET api/products/total-count: This endpoint asynchronously retrieves the total count of products and returns it in an HTTP response,
        handling potential exceptions with a 500 Internal Server Error status.
        */
        [HttpGet]
        [Route("total-count")]
        public async Task<IActionResult> GetTotalProductCount()
        {
            try
            {
                var count = await _productService.GetTotalProductCount();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error - " + ex.Message);
            }

        }

       
        /*
        PUT api/products/{id}: This endpoint handles HTTP PUT requests to update a product by its unique identifier. 
        It checks if the provided product ID matches the one in the request body and if the product exists; 
        if so, it updates the product's attributes and returns a 204 No Content status to indicate success.
        */

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product productBody)
        {
            try
            {
                if(id != productBody.Id)
                {
                    return BadRequest("id in route is different id in body");
                }

                var product = await _productService.GetProductById(id);
                if(product == null)
                {
                    return NotFound();
                }

                product.Name = productBody.Name;
                product.Description = productBody.Description;
                product.Price = productBody.Price;
                product.Category = productBody.Category;

                await _productService.UpdateProduct(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
                return StatusCode(500, "Error - " + ex.Message);
            }
        }

        

    
       /*
       GET api/products/sort: This endpoint allows sorting products based on specified criteria (name, category, or price) and 
       order (ascending or descending), returning the sorted list in an HTTP response, with an option to specify the sorting order.
       */

       [HttpGet]
       [Route("sort")]
       public async Task<IActionResult> SortProduct([FromQuery] string criteria, [FromQuery] string order)
       {
           try
           {
               List<Product> products = new List<Product>();
               switch(criteria)
               {
                   case "name": 
                    products = await _productService.SortProductsByName(order);
                    break;
                   case "category" : 
                    products = await _productService.SortProductsByCategory(order);
                    break;
                   case "price" : 
                    products = await _productService.SortProductsByPrice(order);
                    break;
                   default : 
                    products = await _productService.GetAllProducts();
                    break;
               };

               return Ok(products);
           }
           catch (Exception ex)
           {
                return StatusCode(500, "Error - " + ex.Message);
           }
       }
       
       /*
       GET api/products/category/{category}: This endpoint asynchronously retrieves products by a specified category, 
       handling potential exceptions and returning them in an HTTP response, with a 404 status if no products are found for the category.
       */

       [HttpGet]
       [Route("category/{category}")]
       public async Task<IActionResult> GetProductsByCategory([FromRoute] string category)
       {
           try
           {
               var products = await _productService.GetProductsByCategory(category);
               return Ok(products);
           }
           catch (Exception ex)
           {
                return StatusCode(500, "Error - " + ex.Message);
           }
       }
       
       /*
       DELETE api/products/{id}: This endpoint handles HTTP DELETE requests to delete a product by its unique identifier,
       returning a 204 No Content status upon successful deletion or a 404 status if the product is not found.
       */

       [HttpDelete]
       [Route("{id}")]
       public async Task<IActionResult> DeleteProduct([FromRoute] int id)
       {
           try
           {
               var product = await _productService.GetProductById(id);
               if(product == null)
               {
                   return NotFound();
               }
               await _productService.DeleteProduct(id);
               return NoContent();
           }
           catch (Exception ex)
           {
                return StatusCode(500, "Error - " + ex.Message);
           }
       }
       
       
       /*
       DELETE api/products: This endpoint handles HTTP DELETE requests to delete all products, returning a 204 No Content status upon successful deletion.
       */
       [HttpDelete]
       public async Task<IActionResult> DeleteAllProducts()
       {
           await _productService.DeleteAllProducts();
           return NoContent();
       }
    }
    
}
