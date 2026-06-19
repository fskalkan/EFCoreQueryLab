using EFCoreQueryLab.Domain.Common;

namespace EFCoreQueryLab.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}