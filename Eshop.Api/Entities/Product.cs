using System.ComponentModel.DataAnnotations;

namespace Eshop.Api.Entities;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Name { get; set; }

    [Required]
    [StringLength(20)]
    public required string Genre { get; set; }

    [Range(1, 100)]
    public decimal UnitPrice { get; set; }

    [Range(0, 10000)]
    public int UnitInStock { get; set; }
    public DateTime ReleaseDate { get; set; }

    [Url]
    [StringLength(100)]
    public required string ImageUri { get; set; }
}
