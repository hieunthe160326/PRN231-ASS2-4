using System;
using System.Collections.Generic;

namespace eStoreAPI.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;
}
