using System.ComponentModel.DataAnnotations;

namespace CarServiceTracker.Models
{
    public class ServiceRecord
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        public int ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; } = null!;
    }
}

