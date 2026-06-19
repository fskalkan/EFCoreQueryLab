using EFCoreQueryLab.Domain.Common;

namespace EFCoreQueryLab.Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}