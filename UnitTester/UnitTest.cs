using DataLayer;
using DataRepo;
using System.Data.Common;
using Xunit;

namespace UnitTester
{
    public class UnitTest
    {
        [Theory]
        [InlineData(10, Location.Gent, true, 1)]
        [InlineData(11, Location.Brussel, true, 1)]
        [InlineData(7, Location.Brussel, true, 1)]
        [InlineData(6, Location.Brussel, true, 1)]
        [InlineData(1, Location.Brussel, false, 1)]
        [InlineData(2, Location.Brussel, false, 1)]
        [InlineData(11, Location.Brussel, true, 0)]
        [InlineData(10, Location.Brussel, true, 0)]
        [InlineData(9, Location.Brussel, false, 0)]
        [InlineData(8, Location.Gent, true, 0)]
        [InlineData(7, Location.Gent, false, 0)]
        [InlineData(0, Location.Brussel, false, 1)]
        public void CarAvailability_Test(int startTimeHourToAdd, Location startLoc, bool expectedResult, int available)
        {
            string connectionString = "Data Source=HIMEKO\\SQLEXPRESS;Initial Catalog=RentACar;Integrated Security=True";
            ReservationRepo rr = new ReservationRepo(connectionString);
            CarRepo carRepo = new CarRepo(connectionString);
            LocationAvailabilityRepo locationAvailabilityRepo = new LocationAvailabilityRepo(connectionString);
            CustomerRepo cr = new CustomerRepo(connectionString);
            DataManager dataManager = new DataManager(carRepo, cr, locationAvailabilityRepo, rr);

            Car car = new Car(1, "Limousine", 100, 500, 600, 2000);
            LocationAvailability la = new LocationAvailability(Location.Gent);
            la.CarsToAmount.Add(car, available);
            List<LocationAvailability> las = new List<LocationAvailability>();
            las.Add(la);
            List<Car> cars = new List<Car>() { car };
            Customer c = null;
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            DateTime startTimeOld = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            startTimeOld = startTimeOld.AddHours(1);
            startTime = startTime.AddHours(startTimeHourToAdd + 1);

            List<Reservation> reservations = new List<Reservation>();

            Reservation reservationOld = new Reservation(DateTime.Now, c, cars, Arrangement.Business, startTimeOld, 4, Location.Gent, Location.Gent);
            Reservation reservationToCheck = new Reservation(DateTime.Now, c, cars, Arrangement.Business, startTime, 8, startLoc, Location.Hasselt);
            reservations.Add(reservationOld);

            bool isAvailable = dataManager.CarAvalability(reservationToCheck, las, reservations);

            Assert.Equal(isAvailable, expectedResult);

        }

        [Theory]
        [InlineData(10, Location.Gent, true, 1, 1)]
        [InlineData(11, Location.Brussel, true, 1, 1)]
        [InlineData(7, Location.Brussel, true, 1, 1)]
        [InlineData(6, Location.Brussel, true, 1, 1)]
        [InlineData(1, Location.Brussel, false, 1, 1)]
        [InlineData(2, Location.Brussel, false, 1, 1)]
        [InlineData(9, Location.Brussel, false, 0, 0)]
        [InlineData(7, Location.Gent, false, 0, 0)]
        [InlineData(10, Location.Brussel, true, 0, 0)]
        [InlineData(11, Location.Brussel, true, 0, 0)]
        [InlineData(8, Location.Gent, true, 0, 0)]
        public void DoubleCarAvailability_Test(int startTimeHourToAdd, Location startLoc, bool expectedResult, int available, int available2)
        {
            string connectionString = "Data Source=HIMEKO\\SQLEXPRESS;Initial Catalog=RentACar;Integrated Security=True";
            ReservationRepo rr = new ReservationRepo(connectionString);
            CarRepo carRepo = new CarRepo(connectionString);
            LocationAvailabilityRepo locationAvailabilityRepo = new LocationAvailabilityRepo(connectionString);
            CustomerRepo cr = new CustomerRepo(connectionString);
            DataManager dataManager = new DataManager(carRepo, cr, locationAvailabilityRepo, rr);

            Car car = new Car(1, "Limousine", 100, 500, 600, 2000);
            Car car2 = new Car(2, "Mazda Mx5", 200, 500, 600, 2000);
            LocationAvailability la = new LocationAvailability(Location.Gent);
            la.CarsToAmount.Add(car, available);
            la.CarsToAmount.Add(car2, available2);
            List<LocationAvailability> las = new List<LocationAvailability>();
            las.Add(la);
            List<Car> cars = new List<Car>() { car, car2 };
            Customer c = null;
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            DateTime startTimeOld = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            startTimeOld = startTimeOld.AddHours(1);
            startTime = startTime.AddHours(startTimeHourToAdd + 1);

            List<Reservation> reservations = new List<Reservation>();

            Reservation reservationOld = new Reservation(DateTime.Now, c, cars, Arrangement.Business, startTimeOld, 4, Location.Gent, Location.Gent);
            Reservation reservationToCheck = new Reservation(DateTime.Now, c, cars, Arrangement.Business, startTime, 8, startLoc, Location.Hasselt);
            reservations.Add(reservationOld);

            bool isAvailable = dataManager.CarAvalability(reservationToCheck, las, reservations);

            Assert.Equal(isAvailable, expectedResult);
        }

        [Fact]
        public void WeddingPriceCalc_Test()
        {
            Arrangement arrangement = Arrangement.Wedding;
            DateTime startTime = new DateTime(2023, 08, 01, 10, 0, 0, 0);
            Location startLoc = Location.Gent;
            Location endLoc = Location.Hasselt;
            DateTime date = DateTime.Now;
            Customer c = null;
            Car car = new Car(1, "Limousine", 100, 500, 600, 2000);
            Car car2 = new Car(2, "Mazda Mx5", 200, 500, 600, 2000);
            List<Car> cars = new List<Car>() { car, car2 };
            int NumberOfHours = 7;

            Reservation r = new Reservation(date, c, cars, arrangement, startTime, NumberOfHours, startLoc, endLoc);
            double priceResult = r.Cars[0].WeddingPrice + r.Cars[1].WeddingPrice;
            double priceResult2 = Math.Round(priceResult * 1.21, 2);

            Assert.Equal(priceResult, r.TotalExclTax);
            Assert.Equal(priceResult2, r.TotalInclTax);
        }
        [Fact]
        public void NightLifePriceCalc_Test()
        {
            Arrangement arrangement = Arrangement.NightLife;
            DateTime startTime = new DateTime(2023, 08, 01, 21, 0, 0, 0);
            Location startLoc = Location.Gent;
            Location endLoc = Location.Hasselt;
            DateTime date = DateTime.Now;
            Customer c = null;
            Car car = new Car(1, "Limousine", 100, 500, 600, 2000);
            Car car2 = new Car(2, "Mazda Mx5", 200, 500, 600, 2000);
            List<Car> cars = new List<Car>() { car, car2 };
            int NumberOfHours = 7;

            Reservation r = new Reservation(date, c, cars, arrangement, startTime, NumberOfHours, startLoc, endLoc);
            double priceResult = r.Cars[0].NightLifePrice + r.Cars[1].NightLifePrice;
            double priceResult2 = Math.Round(priceResult * 1.21, 2);

            Assert.Equal(priceResult, r.TotalExclTax);
            Assert.Equal(priceResult2, r.TotalInclTax);

        }

        [Theory]
        [InlineData(Arrangement.Airport, 4)]
        [InlineData(Arrangement.Business, 11)]
        [InlineData(Arrangement.Airport, 2)]
        [InlineData(Arrangement.Business, 2)]
        [InlineData(Arrangement.Business, 1)]
        [InlineData(Arrangement.Airport, 1)]
        [InlineData(Arrangement.Airport, 11)]
        public void NormalPriceCalc2Car_Test(Arrangement arrangement, int NumberOfHours)
        {
            DateTime startTime = new DateTime(2023, 08, 01, 12, 0, 0, 0);
            Location startLoc = Location.Gent;
            Location endLoc = Location.Hasselt;
            DateTime date = DateTime.Now;
            Customer c = null;
            Car car = new Car(1, "Limousine", 100, 500, 600, 2000);
            Car car2 = new Car(2, "Mazda Mx5", 200, 500, 600, 2000);
            List<Car> cars = new List<Car>() { car, car2 };

            Reservation r = new Reservation(date, c, cars, arrangement, startTime, NumberOfHours, startLoc, endLoc);
            double priceResult = 0;
            foreach(Car carRes in r.Cars)
            {
                priceResult += (NumberOfHours - r.NumberOfNighthours - 1) * (carRes.FirstHourPrice *0.6);
                priceResult += r.NumberOfNighthours * (carRes.FirstHourPrice * 1.2);
                priceResult += carRes.FirstHourPrice;
            }
            
            double priceResult2 = Math.Round(priceResult * 1.21, 2);



            Assert.Equal(priceResult, r.TotalExclTax);
            Assert.Equal(priceResult2, r.TotalInclTax);
        }

        [Theory]
        [InlineData(Arrangement.Airport, 4)]
        [InlineData(Arrangement.Business, 11)]
        [InlineData(Arrangement.Airport, 2)]
        [InlineData(Arrangement.Business, 2)]
        [InlineData(Arrangement.Business, 1)]
        [InlineData(Arrangement.Airport, 1)]
        [InlineData(Arrangement.Airport, 11)]
        public void NormalPriceCalc_Test(Arrangement arrangement, int NumberOfHours)
        {
            DateTime startTime = new DateTime(2023, 08, 01, 12, 0, 0, 0);
            Location startLoc = Location.Gent;
            Location endLoc = Location.Hasselt;
            DateTime date = DateTime.Now;
            Customer c = null;
            Car car = new Car(1, "Limousine", 100, 500, 600, 2000);
            List<Car> cars = new List<Car>() { car };

            Reservation r = new Reservation(date, c, cars, arrangement, startTime, NumberOfHours, startLoc, endLoc);
            double priceResult = 0;
            foreach (Car carRes in r.Cars)
            {
                priceResult += (NumberOfHours - r.NumberOfNighthours - 1) * (carRes.FirstHourPrice * 0.6);
                priceResult += r.NumberOfNighthours * (carRes.FirstHourPrice * 1.2);
                priceResult += carRes.FirstHourPrice;
            }

            double priceResult2 = Math.Round(priceResult * 1.21, 2);



            Assert.Equal(priceResult, r.TotalExclTax);
            Assert.Equal(priceResult2, r.TotalInclTax);
        }

        [Theory]
        [InlineData(Arrangement.Airport, 4, 0)]
        [InlineData(Arrangement.Business, 11, 5)]
        [InlineData(Arrangement.Airport, 2, 0)]
        [InlineData(Arrangement.Business, 2, 0)]
        [InlineData(Arrangement.Business, 8, 2)]
        [InlineData(Arrangement.Airport, 7, 1)]
        [InlineData(Arrangement.Airport, 11, 5)]
        public void NightHours_Test(Arrangement arrangement, int NumberOfHours, int result)
        {
            DateTime startTime = new DateTime(2023, 08, 01, 16, 0, 0, 0);
            Location startLoc = Location.Gent;
            Location endLoc = Location.Hasselt;
            DateTime date = DateTime.Now;
            Customer c = null;
            Car car = new Car(1, "Limousine", 100, 500, 600, 2000);
            List<Car> cars = new List<Car>() { car };

            Reservation r = new Reservation(date, c, cars, arrangement, startTime, NumberOfHours, startLoc, endLoc);

            Assert.Equal(result, r.NumberOfNighthours);
        }
    }
}