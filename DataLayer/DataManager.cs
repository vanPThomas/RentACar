using DataLayer.Interfaces;
using System;
namespace DataLayer
{
    public class DataManager
    {
        private ICarRepo _carRepo;
        private ICustomerRepo _customerRepo;
        private ILocationAvailabilityRepo _locationAvailabilityRepo;
        private IReservationRepo _reservationRepo;

        public DataManager(ICarRepo carRepo, ICustomerRepo customerRepo, ILocationAvailabilityRepo locationAvailabilityRepo, IReservationRepo reservationRepo)
        {
            _carRepo = carRepo;
            _customerRepo = customerRepo;
            _locationAvailabilityRepo = locationAvailabilityRepo;
            _reservationRepo = reservationRepo;
        }

        public List<Car> GetCars()
        {
            return _carRepo.GetCars();
        }

        public List<Customer> GetCustomer()
        {
            return _customerRepo.GetCustomerList();
        }

        public void SetCustomer(Customer c)
        {
            _customerRepo.WriteCustomerInDB(c);
        }

        public List<Reservation> GetReservation()
        {
            return _reservationRepo.GetReservations();
        }

        public void SetReservation(Reservation r)
        {
            _reservationRepo.WriteReservationInDB(r);
        }

        public List<LocationAvailability> GetLocationAvailability()
        {
            return _locationAvailabilityRepo.GetLocationAvailability();
        }
        public Car GetCarById(int carId)
        {
            return _carRepo.GetCarById(carId);
        }

        public bool IsValidReservation(Reservation reservation)
        {
            bool isValid = true;
            if (reservation.NumberOfHours > 11)
                isValid = false;
            if (reservation.Date > reservation.StartTime)
                isValid = false;
            if (reservation.Arrangement == Arrangement.NightLife || reservation.Arrangement == Arrangement.Wedding)
                reservation.NumberOfHours = 7;
            if (reservation.NumberOfHours < 1)
                isValid = false;
            if (reservation.Customer == null)
                isValid = false;
            if (!IsCarAvailability(reservation))
                isValid = false;
            if (reservation.Arrangement == Arrangement.NightLife && reservation.StartTime.Hour < 20)
                isValid = false;
            if (reservation.Arrangement == Arrangement.Wedding && (reservation.StartTime.Hour < 7 || reservation.StartTime.Hour > 15))
                isValid = false;
            if (reservation.Cars.Count == 2 && reservation.Cars[0].Id == reservation.Cars[1].Id)
                isValid = false;
            return isValid;
        }

        public bool IsCarAvailability(Reservation r)
        {
            bool isAvailable = false;

            List<LocationAvailability> locationAvailabilities = GetLocationAvailability();
            List<Reservation> reservationList = GetReservation();

            isAvailable = CarAvalability(r, locationAvailabilities, reservationList);

            return isAvailable;
        }

        public bool CarAvalability(Reservation r, List<LocationAvailability> locationAvailabilities, List<Reservation> reservationList)
        {
            bool isAvailable = false;

            if (r.Cars.Count == 1)
                isAvailable = CarChecker(r, locationAvailabilities, reservationList, r.Cars[0]);

            if (r.Cars.Count == 2)
            {
                //begin car 1
                isAvailable = CarChecker(r, locationAvailabilities, reservationList, r.Cars[0]);
                //begin car2
                if (isAvailable)
                {
                    isAvailable = false;
                    isAvailable = CarChecker(r, locationAvailabilities, reservationList, r.Cars[1]);
                }
            }
            return isAvailable;
        }

        private bool CarChecker(Reservation r, List<LocationAvailability> locationAvailabilities, List<Reservation> reservationList, Car c)
        {
            bool isAvailable = false;
            foreach (LocationAvailability la in locationAvailabilities)
            {
                if (la.Location == r.StartLocation)
                {
                    foreach (Car car in la.CarsToAmount.Keys)
                        if (car.Id == c.Id)
                            if (la.CarsToAmount[car] > 0)
                            {
                                isAvailable = true;
                                break;
                            }
                    break;
                }
            }


            DateTime checkTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

            checkTime2 = checkTime2.AddHours(7);

            if (!isAvailable)
            {
                foreach (LocationAvailability la in locationAvailabilities)
                    foreach (Car car in la.CarsToAmount.Keys)
                        if (car.Id == c.Id)
                            if (la.CarsToAmount[car] > 0)
                                if (DateTime.Compare(r.StartTime, checkTime2) >= 0)
                                {
                                    isAvailable = true;
                                    break;
                                }
            }

            Reservation mostRecentRes = null;

            if (!isAvailable)
            {
                foreach (Reservation reservation in reservationList)
                {
                    DateTime checkTime = reservation.StartTime;
                    checkTime = checkTime.AddHours(reservation.NumberOfHours + 4);
                    foreach(Car car in reservation.Cars)
                    {
                        if (car.Id == c.Id)
                        {
                            if (mostRecentRes != null && mostRecentRes.StartTime <= reservation.StartTime)
                                mostRecentRes = reservation;
                            else if (mostRecentRes == null)
                                mostRecentRes = reservation;
                        }
                    }
                }
            }

            if (!isAvailable)
            {
                if (mostRecentRes != null)
                {
                    DateTime goodLocTime = mostRecentRes.StartTime;
                    goodLocTime = goodLocTime.AddHours(mostRecentRes.NumberOfHours + 4);
                    DateTime farPlaceTime = mostRecentRes.StartTime;
                    farPlaceTime = farPlaceTime.AddHours(mostRecentRes.NumberOfHours + 6);
                    if (mostRecentRes.EndLocation == r.StartLocation && DateTime.Compare(r.StartTime, goodLocTime) >= 0)
                        isAvailable = true;
                    else if (mostRecentRes.EndLocation != r.StartLocation && DateTime.Compare(r.StartTime, farPlaceTime) >= 0)
                        isAvailable = true;

                }
            }
            return isAvailable;
        }

        public Customer GetCustomerByName(string name)
        {
            Customer c = null;
            List<Customer> CustomerList = GetCustomer();
            foreach (Customer customer in CustomerList)
            {
                string cName = customer.FirstName + " " + customer.LastName;
                if (cName == name)
                {
                    c = customer;
                    break;
                }
            }

            return c;
        }

        public List<Reservation> GetReservationsByCustomers(Customer c)
        {
            List<Reservation> reservations = GetReservation();
            List<Reservation> reservationsFromCustomer = new List<Reservation>();
            foreach (Reservation reservation in reservations)
                if (reservation.Customer.FirstName == c.FirstName && reservation.Customer.LastName == c.LastName)
                    reservationsFromCustomer.Add(reservation);
            return reservationsFromCustomer;
        }

        public List<Reservation> GetReservationsByCustomers(Customer c, DateTime? start, DateTime? end)
        {
            List<Reservation> reservations = GetReservation();
            List<Reservation> reservationsFromCustomer = new List<Reservation>();
            foreach (Reservation reservation in reservations)
                if (reservation.Customer.FirstName == c.FirstName && reservation.Customer.LastName == c.LastName && reservation.StartTime >= start && reservation.StartTime <= end)
                    reservationsFromCustomer.Add(reservation);
            return reservationsFromCustomer;
        }

    }
}
