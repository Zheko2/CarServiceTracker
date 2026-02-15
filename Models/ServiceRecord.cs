using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Models
{
    public class ServiceRecord
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Service date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Mileage must be at least 1.")]
        public int Mileage { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "1000000", ErrorMessage = "Price must be between 0 and 1,000,000.")]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Car")]
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        [Display(Name = "Service type")]
        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;
    }
}
