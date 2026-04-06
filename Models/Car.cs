using System.ComponentModel.DataAnnotations;

namespace CarServiceTracker.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = null!;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [StringLength(30)]
        public string Status { get; set; } = "Repairing";

        public int? GarageId { get; set; }
        public Garage? Garage { get; set; }

        [Required]
        public string OwnerId { get; set; } = null!;

        public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}