using System;
using System.Collections.Generic;

namespace Marcet_Api_V2.Models;

public partial class Review
{
    public Guid ReviewId { get; set; }

    public Guid? ProductId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public virtual Product? Product { get; set; }
}
