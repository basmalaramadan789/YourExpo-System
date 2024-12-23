﻿using System.ComponentModel.DataAnnotations.Schema;

namespace YourExpo.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }


    public Product Product { get; set; }
}
