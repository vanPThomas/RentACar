namespace DataLayer
{
    public class LocationAvailability
    {
        public LocationAvailability()
        {
        }

        public LocationAvailability(Location location)
        {
            Location = location;
        }

        public Location Location { get; set; }
        public Dictionary<Car, int> CarsToAmount { get; set; } = new Dictionary<Car, int>();
    }
}
