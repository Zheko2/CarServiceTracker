using System.ComponentModel.DataAnnotations;

namespace CarServiceTracker.Models
{
    public class Garage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = null!;

        [Required]
        public string OwnerId { get; set; } = null!;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}