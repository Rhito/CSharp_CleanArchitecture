using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Infrastructure.Data
{
    public static class MockData
    {
        private static readonly List<Category> value = [
            new Category { Id = 1, Name = "Electronics", Description = "Electronic gadgets and devices" },
            new Category { Id = 2, Name = "Books", Description = "Various kinds of books" },
            new Category { Id = 3, Name = "Clothing", Description = "Apparel and accessories" },
        ];
        public static List<Category> Categories = value;

        public static List<Product> Products = [
            new Product { Id = 1, Name = "Smartphone", Description = "Latest model smartphone", Price = 699.99M, StockQuantity = 50, CategoryId = 1 },
            new Product { Id = 2, Name = "Laptop", Description = "High performance laptop", Price = 1299.99M, StockQuantity = 30, CategoryId = 1 },
            new Product { Id = 3, Name = "Mouse", Description = "Bestselling fiction mouse", Price = 19.99M, StockQuantity = 100, CategoryId = 2 },
            new Product { Id = 4, Name = "Display Screen", Description = "High quality and high light power", Price = 9.99M, StockQuantity = 200, CategoryId = 3 },
            new Product { Id = 5, Name = "Headphones", Description = "Noise-cancelling headphones", Price = 199.99M, StockQuantity = 75, CategoryId = 1 },
            new Product { Id = 6, Name = "T-Shirt", Description = "Comfortable cotton t-shirt", Price = 14.99M, StockQuantity = 150, CategoryId = 3 },
            new Product { Id = 7, Name = "Novel", Description = "Bestselling fiction novel", Price = 24.99M, StockQuantity = 80, CategoryId = 2 },
            new Product { Id = 8, Name = "Tablet", Description = "Lightweight tablet device", Price = 499.99M, StockQuantity = 40, CategoryId = 1 },
            new Product { Id = 9, Name = "Jeans", Description = "Stylish denim jeans", Price = 49.99M, StockQuantity = 120, CategoryId = 3 },
            new Product { Id = 10, Name = "E-Reader", Description = "Portable e-reader device", Price = 129.99M, StockQuantity = 60, CategoryId = 2 },
            new Product { Id = 11, Name = "Smartwatch", Description = "Feature-rich smartwatch", Price = 299.99M, StockQuantity = 45, CategoryId = 1 },
            ];
        public static List<Customer> Customers = [
            new Customer { Id = 1, Name = "John Doe", Email = "JohnDeo@example.com", PhoneNumber = "123-456-7890" },
            new Customer { Id = 2, Name = "Jane Smith", Email = "JaneSmith@example.com", PhoneNumber = "987-654-3210" },
            new Customer { Id = 3, Name = "Alice Johnson", Email = "Alice@mailer.com", PhoneNumber = "555-123-4567" },
            new Customer { Id = 4, Name = "Bob Brown", Email = "SpownBob@cc.com", PhoneNumber = "444-555-6666" },
            new Customer { Id = 5, Name = "Charlie Davis", Email = "unprogress", PhoneNumber = "333-222-1111" },
            new Customer { Id = 6, Name = "Diana Evans", Email = "unprogress", PhoneNumber = "777-888-9999" },
            new Customer { Id = 7, Name = "Frank Green", Email = "unprogress", PhoneNumber = "000-111-2222" },
            ];
        public static List<Order> Orders = [
            new Order { Id = 1, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-10), TotalAmount = 899.98M },
            new Order { Id = 2, CustomerId = 2, OrderDate = DateTime.Now.AddDays(-5), TotalAmount = 199.99M },
            new Order { Id = 3, CustomerId = 3, OrderDate = DateTime.Now.AddDays(-2), TotalAmount = 1299.99M },
            new Order { Id = 4, CustomerId = 4, OrderDate = DateTime.Now.AddDays(-1), TotalAmount = 699.99M },
            new Order { Id = 5, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-3), TotalAmount = 49.99M },
            new Order { Id = 6, CustomerId = 2, OrderDate = DateTime.Now.AddDays(-7), TotalAmount = 24.99M },
            new Order { Id = 7, CustomerId = 3, OrderDate = DateTime.Now.AddDays(-4), TotalAmount = 499.99M },
            new Order { Id = 8, CustomerId = 4, OrderDate = DateTime.Now.AddDays(-6), TotalAmount = 129.99M },
            new Order { Id = 9, CustomerId = 1, OrderDate = DateTime.Now.AddDays(-8), TotalAmount = 299.99M },
            new Order { Id = 10, CustomerId = 2, OrderDate = DateTime.Now.AddDays(-9), TotalAmount = 14.99M },
            ];
        public static List<OrderDetail> OrderDetails = [
            new OrderDetail { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 699.99M },
            new OrderDetail { Id = 2, OrderId = 1, ProductId = 3, Quantity = 1, UnitPrice = 199.99M },
            new OrderDetail { Id = 3, OrderId = 2, ProductId = 5, Quantity = 1, UnitPrice = 199.99M },
            new OrderDetail { Id = 4, OrderId = 3, ProductId = 2, Quantity = 1, UnitPrice = 1299.99M },
            new OrderDetail { Id = 5, OrderId = 4, ProductId = 1, Quantity = 1, UnitPrice = 699.99M },

            ];
    }
}
