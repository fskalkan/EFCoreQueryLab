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
        // AsNoTracking kullanılmadığı için EF Core gelen Product kayıtlarını ChangeTracker içinde takip eder.
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
        // Sadece okuma yapılan sorgularda tracking maliyetini azaltmak için AsNoTracking kullanılır.
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
        // Entity'nin tamamını çekmek yerine response için gereken kolonları seçiyoruz.
        // CategoryName için Include yazmadık; EF Core gerekli JOIN'i SQL tarafında oluşturur.
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

        // ToQueryString, EF Core'un bu LINQ sorgusunu hangi SQL'e çevirdiğini görmek için kullanılır.
        var sql = query.ToQueryString();

        return Ok(sql);
    }

    [HttpGet("products/include-category")]
    public async Task<IActionResult> GetProductsWithIncludeCategory()
    {
        // Entity'yi doğrudan dönmek object cycle problemine yol açabilir.
        // Bu yüzden Category bilgisini de kontrollü şekilde projection ile dönüyoruz.
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

    [HttpGet("orders/include-theninclude")]
    public async Task<IActionResult> GetOrdersWithIncludeThenInclude()
    {
        // Include ile Order'ın Customer bilgisini,
        // ThenInclude ile OrderItems içindeki Product bilgisini yüklüyoruz.
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
            .ToListAsync();

        // Entity grafiğini direkt dönmek yerine temiz bir response modeli hazırlıyoruz.
        var result = orders.Select(x => new
        {
            x.Id,
            x.OrderNumber,
            CustomerName = x.Customer.FullName,
            x.OrderDate,
            Items = x.OrderItems.Select(i => new
            {
                ProductName = i.Product.Name,
                i.Quantity,
                i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }),
            TotalAmount = x.OrderItems.Sum(i => i.Quantity * i.UnitPrice)
        });

        return Ok(result);
    }

    [HttpGet("orders/detail-projection")]
    public async Task<IActionResult> GetOrdersDetailProjection()
    {
        // Include kullanmadan, ilişkili tabloları projection içinde seçiyoruz.
        // EF Core Customer, OrderItems ve Product için gerekli JOIN'leri kendisi üretir.
        var orders = await _context.Orders
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.OrderNumber,
                CustomerName = x.Customer.FullName,
                x.OrderDate,
                Items = x.OrderItems.Select(i => new
                {
                    ProductName = i.Product.Name,
                    i.Quantity,
                    i.UnitPrice,
                    TotalPrice = i.Quantity * i.UnitPrice
                }),
                TotalAmount = x.OrderItems.Sum(i => i.Quantity * i.UnitPrice)
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("orders/detail-sql")]
    public IActionResult GetOrdersDetailSql()
    {
        var query = _context.Orders
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.OrderNumber,
                CustomerName = x.Customer.FullName,
                TotalAmount = x.OrderItems.Sum(i => i.Quantity * i.UnitPrice)
            });

        // Projection, JOIN ve SUM ifadelerinin SQL'e nasıl çevrildiğini görmek için.
        var sql = query.ToQueryString();

        return Ok(sql);
    }

    [HttpGet("products/filter-sort-page")]
    public async Task<IActionResult> GetProductsWithFilteringSortingPagination(
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? categoryId,
        [FromQuery] string? search,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortDirection = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 50)
            pageSize = 50;

        // IQueryable sayesinde filtreleri adım adım sorguya ekliyoruz.
        // ToListAsync çağrılana kadar veritabanına sorgu gitmez.
        var query = _context.Products
            .AsNoTracking()
            .AsQueryable();

        if (minPrice.HasValue)
            query = query.Where(x => x.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(x => x.Price <= maxPrice.Value);

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search));

        var isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        // Kullanıcıdan gelen sortBy değerini kontrollü şekilde işliyoruz.
        // Direkt dinamik SQL yazmadığımız için gereksiz risk almıyoruz.
        query = sortBy.ToLower() switch
        {
            "price" => isDescending
                ? query.OrderByDescending(x => x.Price)
                : query.OrderBy(x => x.Price),

            "stock" => isDescending
                ? query.OrderByDescending(x => x.Stock)
                : query.OrderBy(x => x.Stock),

            _ => isDescending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name)
        };

        // Pagination'dan önce toplam kayıt sayısını alıyoruz.
        // Böylece frontend toplam kaç kayıt olduğunu bilebilir.
        var totalCount = await query.CountAsync();

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                ProductName = x.Name,
                x.Price,
                x.Stock,
                CategoryName = x.Category.Name
            })
            .ToListAsync();

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = products
        });
    }

    [HttpGet("products/group-by-category")]
    public async Task<IActionResult> GetProductsGroupByCategory()
    {
        // Kategori bazlı ürün sayısı, toplam stok ve ortalama fiyatı hesaplıyoruz.
        // Bu sorgu SQL tarafında GROUP BY olarak çalışır.
        var result = await _context.Products
            .AsNoTracking()
            .GroupBy(x => x.Category.Name)
            .Select(g => new
            {
                CategoryName = g.Key,
                ProductCount = g.Count(),
                TotalStock = g.Sum(x => x.Stock),
                AveragePrice = g.Average(x => x.Price)
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("products/group-by-category-sql")]
    public IActionResult GetProductsGroupByCategorySql()
    {
        var query = _context.Products
            .AsNoTracking()
            .GroupBy(x => x.Category.Name)
            .Select(g => new
            {
                CategoryName = g.Key,
                ProductCount = g.Count(),
                TotalStock = g.Sum(x => x.Stock),
                AveragePrice = g.Average(x => x.Price)
            });

        // GROUP BY sorgusunun SQL karşılığını görmek için.
        var sql = query.ToQueryString();

        return Ok(sql);
    }

    [HttpGet("products/having-sql")]
    public IActionResult GetProductsHavingSql()
    {
        var query = _context.Products
            .AsNoTracking()
            .GroupBy(x => x.Category.Name)
            .Where(g => g.Count() >= 2)
            .Select(g => new
            {
                CategoryName = g.Key,
                ProductCount = g.Count()
            });

        // GroupBy sonrası filtreleme SQL tarafında HAVING olarak karşılık bulur.
        var sql = query.ToQueryString();

        return Ok(sql);
    }

    [HttpGet("orders/left-join-payment-sql")]
    public IActionResult GetOrdersWithPaymentSql()
    {
        var query = _context.Orders
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.OrderNumber,
                CustomerName = x.Customer.FullName,

                // Payment nullable olduğu için EF Core burada LEFT JOIN üretir.
                PaymentAmount = x.Payment == null ? null : (decimal?)x.Payment.Amount
            });

        var sql = query.ToQueryString();

        return Ok(sql);
    }
}