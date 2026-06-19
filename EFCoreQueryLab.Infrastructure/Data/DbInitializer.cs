using EFCoreQueryLab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryLab.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Customers.AnyAsync())
            return;

        var electronics = new Category { Name = "Electronics" };
        var books = new Category { Name = "Books" };
        var home = new Category { Name = "Home" };

        var laptop = new Product
        {
            Name = "Laptop",
            Price = 35000,
            Stock = 15,
            Category = electronics
        };

        var phone = new Product
        {
            Name = "Phone",
            Price = 22000,
            Stock = 30,
            Category = electronics
        };

        var cleanCodeBook = new Product
        {
            Name = "Clean Code Book",
            Price = 750,
            Stock = 50,
            Category = books
        };

        var chair = new Product
        {
            Name = "Office Chair",
            Price = 4500,
            Stock = 20,
            Category = home
        };

        var customer1 = new Customer
        {
            FullName = "Samet Kalkan",
            Email = "samet@test.com"
        };

        var customer2 = new Customer
        {
            FullName = "Ahmet Yılmaz",
            Email = "ahmet@test.com"
        };

        var order1 = new Order
        {
            OrderNumber = "ORD-1001",
            Customer = customer1,
            OrderDate = DateTime.UtcNow.AddDays(-3),
            OrderItems =
            {
                new OrderItem
                {
                    Product = laptop,
                    Quantity = 1,
                    UnitPrice = laptop.Price
                },
                new OrderItem
                {
                    Product = cleanCodeBook,
                    Quantity = 2,
                    UnitPrice = cleanCodeBook.Price
                }
            },
            Payment = new Payment
            {
                Amount = 36500,
                PaymentDate = DateTime.UtcNow.AddDays(-3)
            }
        };

        var order2 = new Order
        {
            OrderNumber = "ORD-1002",
            Customer = customer2,
            OrderDate = DateTime.UtcNow.AddDays(-1),
            OrderItems =
            {
                new OrderItem
                {
                    Product = phone,
                    Quantity = 1,
                    UnitPrice = phone.Price
                },
                new OrderItem
                {
                    Product = chair,
                    Quantity = 1,
                    UnitPrice = chair.Price
                }
            },
            Payment = new Payment
            {
                Amount = 26500,
                PaymentDate = DateTime.UtcNow.AddDays(-1)
            }
        };

        await context.Orders.AddRangeAsync(order1, order2);
        await context.SaveChangesAsync();
    }
}