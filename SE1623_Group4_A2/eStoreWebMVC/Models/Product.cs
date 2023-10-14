using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eStoreWebMVC.Models;

public class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public float? Weight { get; set; }

    public int UnitPrice { get; set; }

    public int UnitInStock { get; set; }
    [JsonPropertyName("OrderDetails")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
