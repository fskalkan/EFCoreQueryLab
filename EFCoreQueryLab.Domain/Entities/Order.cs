using EFCoreQueryLab.Domain.Common;

namespace EFCoreQueryLab.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Payment? Payment { get; set; }
}