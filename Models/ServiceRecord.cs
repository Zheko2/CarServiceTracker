using System;
using System.ComponentModel.DataAnnotations;

namespace CarServiceTracker.Models
{
    public class ServiceRecord
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "1000000")]
        public decimal Price { get; set; }

        public string? Notes { get; set; }

        // Foreign Key
        [Required]
        public int CarId { get; set; }
        public Car? Car { get; set; }

        // Foreign Key
        [Required]
        public int ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }
    }
}
