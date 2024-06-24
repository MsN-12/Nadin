using System;
using System.ComponentModel.DataAnnotations;

namespace Nadin.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Produce Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Produce Date must be a valid date")]
        public DateTime ProduceDate { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Manufacture Phone must be a 10-digit number")]
        public string ManufacturePhone { get; set; }

        [Required(ErrorMessage = "Manufacture Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Manufacture Email format")]
        public string ManufactureEmail { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Produce Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Produce Date must be a valid date")]
        public DateTime ProduceDate { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Manufacture Phone must be a 10-digit number")]
        public string ManufacturePhone { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class TestCreateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Produce Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Produce Date must be a valid date")]
        public DateTime ProduceDate { get; set; }

        [Required(ErrorMessage = "Manufacture Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Manufacture Email format")]
        public string ManufactureEmail { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Manufacture Phone must be a 10-digit number")]
        public string ManufacturePhone { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Produce Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Produce Date must be a valid date")]
        public DateTime ProduceDate { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Manufacture Phone must be a 10-digit number")]
        public string ManufacturePhone { get; set; }

        [Required(ErrorMessage = "Manufacture Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Manufacture Email format")]
        public string ManufactureEmail { get; set; }

        public bool IsAvailable { get; set; }
    }
}