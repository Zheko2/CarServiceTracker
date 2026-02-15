using System.ComponentModel.DataAnnotations;

namespace CarServiceTracker.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; } = null!;

        [Required]
        public string Model { get; set; } = null!;

        [Range(1886, 2100, ErrorMessage = "Year must be between 1886 and 2100.")]
        public int Year { get; set; }

        public ICollection<ServiceRecord> ServiceRecords { get; set; }
            = new List<ServiceRecord>();
    }
}
