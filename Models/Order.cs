namespace YourExpo.Models;

public class Order
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } // User who placed the order
    public ApplicationUser ApplicationUser { get; set; }

    public string ShippingAddress { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public string PhoneNumber { get; set; }
}

public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered
}

