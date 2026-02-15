using System.ComponentModel.DataAnnotations;

namespace CarServiceTracker.Models
{
    public class ServiceType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<ServiceRecord> ServiceRecords { get; set; }
            = new List<ServiceRecord>();
    }
}
