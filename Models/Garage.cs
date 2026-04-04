namespace CarServiceTracker.Models
{
    public class Garage
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}