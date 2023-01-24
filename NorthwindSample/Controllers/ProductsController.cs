using Bogus;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using NorthwindSample.Extensions;
using NorthwindSample.Models;
using NorthwindSample.SearchHelper;

namespace NorthwindSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public ProductsController(NorthwindContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Get(Search search)
        {
            var predicateChain = PredicateBuilder.New<Product>();

            var va = SearchHelper.SearchValueType.String;
            foreach (var item in search.Where)
            {
                switch (item.Operator)
                {
                    case Operator.Equal:
                        var equal = ExpressionHelper.GetExpressionEqual<Product>(item.Column, item.Value, item.SearchValueType);
                        predicateChain.And(equal);
                        break;

                    case Operator.NotEqual:
                        break;

                    case Operator.GreaterThan:
                        break;

                    case Operator.LessThan:
                        break;

                    case Operator.Like:
                        if (item.SearchValueType != SearchValueType.String)
                            throw new ArgumentException();

                        var like = ExpressionHelper.GetExpressionContains<Product>(item.Column, item.Value);
                        predicateChain.And(like);
                        break;

                    default:
                        break;
                }
            }

            //_context.Orders.Where(predicateChain).ToList();

            ////var products = _context.Orders.Where(predicateChain).ToList();
            //var products = _context.Orders.Where(x => EF.Functions.Like(x.ShipName, $"%{keyword}%")).ToList();

            var products = await _context.Products.Where(predicateChain).ToPagedListAsync(1, 10);
            return Ok(products);
        }

        [HttpPost("OrderSabe")]
        public IActionResult Save()
        {
            var orders = _context.Orders.ToList();
            var customerId = orders.Select(orders => orders.CustomerId);
            var employeeId = orders.Select(orders => orders.EmployeeId);
            var country = orders.Select(orders => orders.ShipCountry);
            var city = orders.Select(orders => orders.ShipCity);
            var ordersa = new Faker<Order>()
                .RuleFor(o => o.CustomerId, f => f.PickRandom(customerId))
                .RuleFor(o => o.EmployeeId, f => f.PickRandom(employeeId))
                .RuleFor(o => o.Freight, f => f.Random.Decimal())
                .RuleFor(o => o.OrderDate, f => f.Date.Recent(0))
                .RuleFor(o => o.RequiredDate, f => f.Date.Recent(0))
                .RuleFor(o => o.ShipAddress, f => f.Address.StreetAddress())
                .RuleFor(o => o.ShipCity, f => f.PickRandom(city))
                .RuleFor(o => o.ShipCountry, f => f.PickRandom(country))
                .RuleFor(o => o.ShipName, f => f.Address.Country())
                .RuleFor(o => o.ShipPostalCode, f => f.Address.ZipCode())
                .RuleFor(o => o.ShipVia, f => 1)
                .RuleFor(o => o.ShippedDate, f => f.Date.Recent(0))
                .Generate(140);

            _context.Orders.AddRange(ordersa);
            _context.SaveChanges();

            return Ok();
        }
    }
}