using AutoMapper;
using Ex_api_DTO.Autentication;
using Ex_api_DTO.Database;
using Ex_api_DTO.Entities;
using Ex_api_DTO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Ex_api_DTO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly FakeDatabase _db;
        private readonly IMapper _mapper;
        private readonly JwtAutenticationManager _jwtAutenticationManager;
        private readonly IMemoryCache _memoryCache;
        private const string CacheMemory = "Cache users"; 

        public ProductsController(FakeDatabase db, IMapper mapper, JwtAutenticationManager jwtAutenticationManager, IMemoryCache memoryCache)
        {
            _db = db; 
            _mapper = mapper;
            _jwtAutenticationManager = jwtAutenticationManager;
            _memoryCache = memoryCache;
        }

        [HttpPost("login")]
        public IActionResult autenticate([FromBody] User user)
        {
            //check user
            var userFind = Users.GetUser(user.UserName);

            if (userFind == null || userFind.Password != user.Password)
            {
                return Unauthorized();
            }
            

            //check token
            var token = _jwtAutenticationManager.Autenticate(userFind.UserName, userFind.Password);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new {Token = token});
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin + "," + Roles.User)]
        //[Authorize(Roles = Roles.Admin)]
        //[Authorize(Roles = Roles.User)]
        public IActionResult GetAllProducts()
        {
            var products = _db.Products.ToList();
            var response = _mapper.Map<List<ProductDto>>(products);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetProduct(int id)
        {
            var checkID = _db.Products.Find(id);

            if (checkID == null)
            {
                return NotFound();
            }
            return Ok(checkID);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            var checkProduct = _db.Products.Any(p => p.Id == product.Id);

            if (checkProduct)
            {
                return BadRequest($"The product with id {product.Id} already exist!");
            }   

            _db.Products.Add(product);
            _db.SaveChanges();
            return CreatedAtAction("GetProduct",new {id = product.Id}, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            var checkIDproduct = _db.Products.Find(id);

            if (checkIDproduct == null)
            {
                return NotFound();
            }

            if (id != product.Id)
            {
                return BadRequest("You can not edit id!");
            }

            _db.Entry(checkIDproduct).CurrentValues.SetValues(product);
            _db.SaveChanges();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteProduct(int id)
        {
            var checkIDproduct = _db.Products.Find(id);

            if (checkIDproduct == null)
            {
                return NotFound();
            }

            _db.Products.Remove(checkIDproduct);
            _db.SaveChanges();

            return NoContent();
        }

        /*
         * without caching memory
         * 
         * [HttpGet("research")]
                public IActionResult SearchProduct([FromQuery] string searchName = "", [FromQuery] string searchCategory = "", [FromQuery] string searchDescription = "", [FromQuery] double minPrice = 0.01)
                {
                    var SearchDb = _db.Products.AsQueryable();

                    if (searchName.TrimStart() != "")
                    {
                        SearchDb = SearchDb.Where(p => p.Name.Contains(searchName));
                    }

                    if (searchCategory.TrimStart() != "")
                    {
                        SearchDb = SearchDb.Where(p => p.Category.Contains(searchName));
                    }

                    if (searchDescription.TrimStart() != "")
                    {
                        SearchDb = SearchDb.Where(p => p.Description.Contains(searchName));
                    }

                    if (minPrice > 0.01)
                    {
                        SearchDb = SearchDb.Where(p => p.Price > minPrice);
                    }

                    return Ok(SearchDb);

                }*/


        //with caching memory
        [HttpGet("research")]
        public IActionResult SearchProduct([FromQuery] string searchName = "", [FromQuery] string searchCategory = "", [FromQuery] string searchDescription = "", [FromQuery] double minPrice = 0.01)
        {
            //create a unique key for request and save on cache
            string productsCaching = $"products-{searchName}-{searchCategory}-{searchDescription}-{minPrice}";
            Console.WriteLine(productsCaching);
            Console.WriteLine(_memoryCache.ToString());

            //check products if save on cache memory
            if (!_memoryCache.TryGetValue(productsCaching, out List<Product> resultDb))
            {
                var SearchDb = _db.Products.AsQueryable();

                if (searchName.TrimStart() != "")
                {
                    SearchDb = SearchDb.Where(p => p.Name.Contains(searchName));
                }

                if (searchCategory.TrimStart() != "")
                {
                    SearchDb = SearchDb.Where(p => p.Category.Contains(searchCategory));
                }

                if (searchDescription.TrimStart() != "")
                {
                    SearchDb = SearchDb.Where(p => p.Description.Contains(searchDescription));
                }

                if (minPrice >= 0.01)
                {
                    SearchDb = SearchDb.Where(p => p.Price >= (double)minPrice);
                }

                resultDb = SearchDb.ToList();

                // save data in memory
                _memoryCache.Set(productsCaching, SearchDb, TimeSpan.FromMinutes(5));
            }

            return Ok(resultDb);

        }

    }
}
