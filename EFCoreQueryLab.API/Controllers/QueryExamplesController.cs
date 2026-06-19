using EFCoreQueryLab.Domain.Entities;
using EFCoreQueryLab.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryLab.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QueryExamplesController : ControllerBase
{
    private readonly AppDbContext _context;

    public QueryExamplesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("products/tracking")]
    public async Task<IActionResult> GetProductsWithTracking()
    {
        var products = await _context.Products
            .ToListAsync();

        var trackedProductCount = _context.ChangeTracker
            .Entries<Product>()
            .Count();

        return Ok(new
        {
            Message = "Products were loaded with tracking.",
            ProductCount = products.Count,
            TrackedProductCount = trackedProductCount,
            Products = products
        });
    }

    [HttpGet("products/no-tracking")]
    public async Task<IActionResult> GetProductsWithNoTracking()
    {
        var products = await _context.Products
            .AsNoTracking()
            .ToListAsync();

        var trackedProductCount = _context.ChangeTracker
            .Entries<Product>()
            .Count();

        return Ok(new
        {
            Message = "Products were loaded with AsNoTracking.",
            ProductCount = products.Count,
            TrackedProductCount = trackedProductCount,
            Products = products
        });
    }

    [HttpGet("products/projection")]
    public async Task<IActionResult> GetProductsWithProjection()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                ProductName = x.Name,
                x.Price,
                CategoryName = x.Category.Name
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("products/sql")]
    public IActionResult GetProductsSql()
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(x => x.Price > 1000)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Price,
                CategoryName = x.Category.Name
            });

        var sql = query.ToQueryString();

        return Ok(sql);
    }


    [HttpGet("products/include-category")]
    public async Task<IActionResult> GetProductsWithIncludeCategory()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                ProductName = x.Name,
                x.Price,
                x.Stock,
                Category = new
                {
                    x.Category.Id,
                    x.Category.Name
                }
            })
            .ToListAsync();

        return Ok(products);
    }
}