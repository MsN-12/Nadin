using System.ComponentModel.DataAnnotations;

namespace Nadin.Domain.Entities;

public class Product
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ProduceDate { get; set; }

    [Required]
    [StringLength(10, MinimumLength = 10)]
    public string ManufacturePhone { get; set; }

    [Required]
    [EmailAddress]
    public string ManufactureEmail { get; set; }

    [Required]
    public bool IsAvailable { get; set; }
}