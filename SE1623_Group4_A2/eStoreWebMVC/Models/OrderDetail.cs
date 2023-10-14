using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eStoreWebMVC.Models;

public class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int ProductId { get; set; }

    public int OrderId { get; set; }

    public int UnitPrice { get; set; }

    public int Quantity { get; set; }

    public int Discount { get; set; }

    public virtual Order Order { get; set; } = null!;
    [JsonPropertyName("Product")]
    public virtual Product Product { get; set; } = null!;
}
