using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisDemo3.Cache;
using RedisDemo3.DBContext;
using RedisDemo3.Entity;

namespace RedisDemo3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController:ControllerBase
    {
        private readonly DbContextClass _dbContext;
        private readonly ICacheService _cacheService;
        public ProductController(DbContextClass dbContext, ICacheService cacheService) { 
            _dbContext = dbContext;
            _cacheService = cacheService;
        }

        [HttpGet("getall")]
        public IEnumerable<Product> GetProducts()
        {
            // get data in cache
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if(cacheData != null)
            {
                return cacheData;
            }
            // if not data in cache, query in database and set data to cache
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            cacheData = _dbContext.Products.ToList();
            _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
            return cacheData;
        }

        [HttpPost("add")]
        public async Task<Product> AddProduct(Product product)
        {
            var obj = await _dbContext.Products.AddAsync(product);
            // remove cache to get newest data when query
            _cacheService.RemoveData("product");
            _dbContext.SaveChanges();
            return obj.Entity;
        }

        [HttpPut("updateproduct")]
        public void Put(Product product)
        {
            _dbContext.Products.Update(product);
            _cacheService.RemoveData("product");
            _dbContext.SaveChanges();
        }

        [HttpDelete("deleteproduct")]
        public void Delete(int Id)
        {
            var filteredData = _dbContext.Products.Where(x => x.ProductId == Id).FirstOrDefault();
            _dbContext.Remove(filteredData);
            _cacheService.RemoveData("product");
            _dbContext.SaveChanges();
        }
    }
}
