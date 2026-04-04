using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarServiceTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [Range(typeof(decimal), "0", "1000000")]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int CarId { get; set; }

        public Car? Car { get; set; }
    }
}