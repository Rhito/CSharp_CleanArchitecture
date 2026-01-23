using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Xml.Linq;

namespace CleanArchitecture.API.Controllers
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

                //MockData.Customers.Where(c => !MockData.Orders.Any(o => (o.CustomerId == c.Id)))

                //MockData.OrderDetails.Join(MockData.Products, o => o.ProductId, p => p.Id, (o, p) => new
                //{
                //    o.Quantity,
                //    o.UnitPrice,
                //    Product = new { p.Id, p.CategoryId }
                //}).Join(MockData.Categories, obj => obj.Product.CategoryId, c => c.Id, (obj, c) => new
                //{
                //    CategoryName = c.Name,
                //    TotalRevenue = (obj.Quantity * obj.UnitPrice)
                //}).GroupBy(c => c.CategoryName).Select( g => new
                //{
                //    CategoryName = g.Key,
                //    TotalRevenue = g.Sum(x => x.TotalRevenue)
                //})

                // Thay vì Join thủ công, EF cho phép đi xuyên qua các lớp
                //MockData.OrderDetails.GroupBy(od => od.Product.Category.Id)
                //.Select(g => new {
                //    CategoryName = g.Key,
                //    TotalRevenue = g.Sum(od => (od.Quantity * od.UnitPrice))
                //})

                //MockData.Orders.Where(o => o.OrderDate >= DateTime.Now.AddDays(-7))
                //.Join(MockData.OrderDetails, o => o.Id, od => od.OrderId, (o, od) => new
                //{
                //    od.ProductId,
                //    od.UnitPrice,
                //    od.Quantity,
                //}).Join(MockData.Products, od => od.ProductId, p => p.Id, (od, p) => new
                //{
                //    p.CategoryId,
                //     od.UnitPrice,
                //    od.Quantity,
                //}).Join(MockData.Categories, p => p.CategoryId, c => c.Id, (p, c) => new
                //{
                //    c.Name,
                //    p.UnitPrice,
                //    p.Quantity,
                //}).GroupBy(x=>x.Name).Select(g => new
                //{
                //    g.Key,
                //    total = g.Sum(od => od.Quantity * od.UnitPrice)
                //})

                MockData.Categories.GroupJoin(
                    MockData.Products,
                    c => c.Id,
                    p => p.CategoryId,
                    (category, products) => new
                    {
                        CategoryName = category.Name,

                        TotalRevenue = products.Sum(p =>
                            MockData.Orders
                                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-7))
                                .Join(
                                    MockData.OrderDetails,
                                    o => o.Id,
                                    od => od.OrderId,
                                    (o, od) => od
                                )
                                .Where(od => od.ProductId == p.Id)
                                .Sum(od => od.Quantity * od.UnitPrice)
                        )
                    }
                )

                    );
        }

        [HttpGet("/Paging")]
        public IActionResult Paging([FromHeader] int pageNumber = 1, [FromHeader] int pageSize = 5)
        {
            return Ok(MockData.Products.Skip((pageSize - 1) * pageSize).Take(3));
        }
    }
}
