
using ECommerce.DAL;
using ECommerce.Common;

namespace ECommerce.DAL;

public class Order : IAuditEntity
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty; 

    public string UserId { get; set; } = string.Empty;
    public virtual ApplicationUser User { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

public class OrderItem : IAuditEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    //public decimal SubTotal => UnitPrice * Quantity;
}