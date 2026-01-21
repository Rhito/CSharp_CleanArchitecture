using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace CleanArchitecture.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinQController : ControllerBase
    {

        [HttpGet("/group")]
        public IActionResult Grouping()
        {
            return Ok(
                MockData.Products.GroupBy(p => p.CategoryId).Select(g => new { CategoryId = g.Key, Count = g.Count() }).ToList()
                );
        }
        [HttpGet("/join")]
        public IActionResult Joining()
        {
            return Ok(
                MockData.Products.Where(p => p.Price > 100m).Join(MockData.Categories, p => p.CategoryId, c => c.Id, (p, c) => new
                {
                    ProductName = p.Name,
                    CategoryName = c.Name,
                }
                ));
        }

        [HttpGet("/aggregation")]
        public IActionResult Aggregation()
        {
            return Ok(
                MockData.Orders.GroupBy(o => o.CustomerId).Join(MockData.Customers, g => g.Key, c => c.Id, (g, c) => new { CustomerName = c.Name, TotalSpent = g.Sum(o => o.TotalAmount) }).OrderByDescending(c => c.TotalSpent)

                );
        }

        [HttpGet("/i")]
        public IActionResult Index()
        {
            return Ok(

                //MockData.Products.Where(p => (p.Name.ToLower().Contains('a') && !string.IsNullOrWhiteSpace(p.Description) && (p.Price >= 100 && p.Price <= 1000)))

                //MockData.OrderDetails.GroupBy(o => o.OrderId).Join(MockData.Orders, x => x.Key, y => y.Id, (x, y) => new
                //{
                //    OrderId = y.Id,
                //    MaxUnitPrice = x.Max(o => o.UnitPrice)
                //})

                MockData.Orders.Join(MockData.Customers, o => o.CustomerId, c => c.Id, (o, c) => new
                {
                    CustomerName = c.Name,
                    o.Id
                })
                );
        }
    }
}
