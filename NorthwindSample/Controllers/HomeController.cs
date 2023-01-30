//using LinqKit;
//using Microsoft.AspNetCore.Mvc;
//using NorthwindSample.Condition;
//using NorthwindSample.Extensions;
//using NorthwindSample.Models;

//namespace NorthwindSample.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class HomeController : ControllerBase
//    {
//        private readonly NorthwindContext _context;

//        public HomeController(NorthwindContext context)
//        {
//            _context = context;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Get(Search search)
//        {
//            var expression = search.Where.GetLambdaExpression<Product>();

//            var products = await _context.Products.Where(expression).ToPagedListAsync(1, 10);
//            return Ok(products);
//        }

//        [HttpPost("Order")]
//        public async Task<IActionResult> GetOrder(Search search)
//        {
//            var expression = search.Where.GetLambdaExpression<Order>();

//            var products = await _context.Orders.Where(expression).ToPagedListAsync(1, 10);
//            return Ok(products);
//        }
//    }
//}