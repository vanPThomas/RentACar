namespace DataLayer
{
    public class Reservation
    {
        public Reservation(DateTime date, Customer customer, List<Car> cars, Arrangement arrangement, DateTime startTime, int numberOfHours, Location startLocation, Location endLocation)
        {
            Date = date;
            Customer = customer;
            Cars = cars;
            Arrangement = arrangement;
            StartTime = startTime;
            NumberOfHours = numberOfHours;
            StartLocation = startLocation;
            EndLocation = endLocation;
            NumberOfNighthours = GetNightHours(numberOfHours, startTime);
            TotalExclTax = CalculateCostNoTax();
            TotalInclTax = CalculateCostWithTax();
        }

        public Reservation(int reservationId, DateTime date, Customer customer, List<Car> cars, Arrangement arrangement, DateTime startTime, int numberOfHours, Location startLocation, Location endLocation)
        {
            ReservationId = reservationId;
            Date = date;
            Customer = customer;
            Cars = cars;
            Arrangement = arrangement;
            StartTime = startTime;
            NumberOfHours = numberOfHours;
            StartLocation = startLocation;
            EndLocation = endLocation;
            NumberOfNighthours = GetNightHours(numberOfHours, startTime);
            TotalExclTax = CalculateCostNoTax();
            TotalInclTax = CalculateCostWithTax();

        }

        public int ReservationId { get; set; }
        public DateTime Date { get; set; }
        public Customer Customer { get; set; }
        public List<Car> Cars { get; } = new List<Car>();
        public Arrangement Arrangement { get; set; }
        public DateTime StartTime { get; set; }
        public int NumberOfHours { get; set; }
        public int NumberOfNighthours { get; set; }
        public Location StartLocation { get; set; }
        public Location EndLocation { get; set; }
        public double TotalInclTax { get; set; }
        public double TotalExclTax { get; set; }

        private int GetNightHours(int numberOfHours, DateTime startTime)
        {
            int nighthours = 0;
            int startHour = startTime.Hour;
            int endHour = startHour + numberOfHours;

            if (startHour < 7)
            {
                int nighthourstemp = 7 - startHour;
                if(nighthourstemp <= numberOfHours)
                    nighthours = nighthourstemp;
                else
                nighthours = numberOfHours;
            }
            else if (startHour >= 22 )
            {
                
                if (numberOfHours <= (31 - startHour))
                    nighthours = numberOfHours;
                else
                nighthours = 31 - startHour;
            }
            else if( startHour + numberOfHours >= 22)
            {
                if(startHour+numberOfHours >= 31)
                    nighthours = 9;
                else
                    nighthours = (startHour + numberOfHours)-22;
            }
            return nighthours;
        }

        public double CalculateCostNoTax()
        {
            double cost = 0;

            foreach (Car car in Cars)
            {
                if (Arrangement == Arrangement.NightLife)
                {
                    cost += car.NightLifePrice;
                }
                else if (Arrangement == Arrangement.Wedding)
                {
                    cost += car.WeddingPrice;
                }
                else
                {
                    cost += (NumberOfHours - NumberOfNighthours - 1) * ((car.FirstHourPrice / 100) * 60);
                    cost += NumberOfNighthours * ((car.FirstHourPrice / 100) * 120);
                    cost += car.FirstHourPrice;
                }
            }
            cost = Math.Round(cost, 2);
            return cost;
        }
        public double CalculateCostWithTax()
        {
            double cost = 0;
            cost = CalculateCostNoTax();

            cost = cost  * 1.21;
            cost = Math.Round(cost, 2);
            return cost;

        }

    }
}
