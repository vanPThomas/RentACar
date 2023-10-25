using DataLayer;
using DataRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for SearchCustomer.xaml
    /// </summary>
    public partial class SearchCustomer : Window
    {
        private string connectionString = "Data Source=HIMEKO\\SQLEXPRESS;Initial Catalog=RentACar;Integrated Security=True";
        private DataManager dataManager;
        private ReservationRepo rr;
        private CarRepo carRepo;
        private LocationAvailabilityRepo locationAvailabilityRepo;
        private CustomerRepo cr;
        private Customer customer;
        private List<Reservation> reservationsForCustomers;
        private List<Car> cars;

        public SearchCustomer()
        {
            InitializeComponent();
            rr = new ReservationRepo(connectionString);
            carRepo = new CarRepo(connectionString);
            cr = new CustomerRepo(connectionString);
            locationAvailabilityRepo = new LocationAvailabilityRepo(connectionString);
            dataManager = new DataManager(carRepo, cr, locationAvailabilityRepo, rr);
            cars = dataManager.GetCars();
        }

        private void SearchCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = null;
            string lastName = null;
            if (!string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
                firstName = FirstNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                lastName = LastNameTextBox.Text;

            string fullName = firstName + " " + lastName;
            customer = dataManager.GetCustomerByName(fullName);
            if (customer != null)
            {
                NameTextBox.Text = fullName;
                StreetTextBox.Text = customer.Street;
                CityTextBox.Text = customer.City;
                HouseNumberTextBox.Text = customer.HouseNumber;
                CustomerIDTextBox.Text = customer.CustomerId.ToString();
                TaxNumberTextBox.Text = customer.TaxNumber;
            }
            else
            {
                NameTextBox.Text = "";
                StreetTextBox.Text = "";
                CityTextBox.Text = "";
                HouseNumberTextBox.Text = "";
                CustomerIDTextBox.Text = "";
                TaxNumberTextBox.Text = "";
            }
        }

        private void SearchReservationsButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? start = StartPicker.SelectedDate;
            DateTime? end = EndPicker.SelectedDate;
            if(customer != null && (!start.HasValue || !end.HasValue))
            {
                reservationsForCustomers = dataManager.GetReservationsByCustomers(customer);
                List<ReservationInfo> reservationsForCustomersInfo = CreateReservationInfoList(reservationsForCustomers);
                ReservationGrid.ItemsSource = reservationsForCustomersInfo;
            }
            else if(customer != null)
            {
                reservationsForCustomers = dataManager.GetReservationsByCustomers(customer, start, end);
                List<ReservationInfo> reservationsForCustomersInfo = CreateReservationInfoList(reservationsForCustomers);
                ReservationGrid.ItemsSource = reservationsForCustomersInfo;
            }

        }

        private void CreateReservationButton_Click(object sender, RoutedEventArgs e)
        {
             Reservation reservation = null;
            try
            {
                DateTime ReservationMadeDate = DateTime.Now;

                DateTime startTime;
                Location startLocation;
                Location endLocation;
                int numberOfHours = 0;
                Car car1 = null;
                Car car2 = null;
                List<Car> cars = new List<Car>();
                Arrangement arrangement;
                
                if(TimeTextBox.Text != "dd/MM/yyyy hh:mm")
                    startTime = Convert.ToDateTime(TimeTextBox.Text);
                else throw new Exception("Invalid Date");

                if(StartLocChoice.SelectedItem != null)
                    startLocation = GetLocation(((ComboBoxItem)StartLocChoice.SelectedItem).Content.ToString());
                else throw new Exception("No Start Location");

                if(EndLocChoice.SelectedItem != null)
                    endLocation = GetLocation(((ComboBoxItem)EndLocChoice.SelectedItem).Content.ToString());
                else throw new Exception("No End Location");

                if(Int32.TryParse(NumberOfHoursTextBox.Text, out numberOfHours))
                    numberOfHours = Convert.ToInt32(NumberOfHoursTextBox.Text);
                else throw new Exception("Invalid Number of hours");

                if(Car1Choice.SelectedItem != null)
                {
                    car1 = GetCar(((ComboBoxItem)Car1Choice.SelectedItem).Content.ToString());
                    cars.Add(car1);
                }
                else throw new Exception("Car1 Empty");

                if (Car2Choice.SelectedItem != null)
                    car2 = GetCar(((ComboBoxItem)Car2Choice.SelectedItem).Content.ToString());
                if (car2 != null)
                    cars.Add(car2);

                if (ArrangementChoice.SelectedItem != null)
                {
                    arrangement = GetArrangement(((ComboBoxItem)ArrangementChoice.SelectedItem).Content.ToString());
                }else throw new Exception("No Arrangement Selected");
                reservation = new Reservation(ReservationMadeDate, customer, cars, arrangement, startTime, numberOfHours, startLocation, endLocation);

                if (customer == null)
                    throw new Exception("Please Choose a Customer");
                
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (reservation != null && dataManager.IsValidReservation(reservation))
                {
                    dataManager.SetReservation(reservation);
                    MessageBox.Show($"Reservation Registered");
                }
                else
                    MessageBox.Show("Not a valid reservation!");
        }

        private Location GetLocation(string loc)
        {
            Location location = new Location();
            switch(loc)
            {
                case "Gent":
                    location = Location.Gent;
                    break;
                case "Antwerpen":
                    location = Location.Antwerpen; 
                    break;
                case "Hasselt":
                    location = Location.Hasselt;
                    break;
                case "Brussel":
                    location = Location.Brussel;
                    break;
                case "Charleroi":
                    location = Location.Charleroi;
                    break;
            }


            return location;
        }
        private Car GetCar(string cartext)
        {
            Car car = null;

            switch(cartext)
            {
                case "Audi A8":
                    car = cars[0];
                    break;
                case "Ford Mustang":
                    car = cars[1];
                    break;
                case "BMW i8":
                    car = cars[2];
                    break;
                case "BMW i8 Spyder":
                    car = cars[3];
                    break;
                case "Mercedes G63 AMG":
                    car = cars[4];
                    break;
                case "Bentley Continental GTC":
                    car = cars[5];
                    break;
                case "Volkswagen Kever Cabrio":
                    car = cars[6];
                    break;
                case "Austin Healey 3000 S":
                    car = cars[7];
                    break;
                case "Porsche 912":
                    car = cars[8];
                    break;
                case "Mercedes 190 SL":
                    car = cars[9];
                    break;
                case "Packard 120 (chauffeur)":
                    car = cars[10];
                    break;
            }

            return car;
        }
        private Arrangement GetArrangement(string arrtext)
        {
            Arrangement ar = new Arrangement();
            switch(arrtext)
            {
                case "Airport":
                    ar = Arrangement.Airport;
                    break;
                case "Business":
                    ar = Arrangement.Business;
                    break;
                case "Wedding":
                    ar = Arrangement.Wedding;
                    break;
                case "NightLife":
                    ar = Arrangement.NightLife;
                    break;
            }

            return ar;
        }

        private List<ReservationInfo> CreateReservationInfoList(List<Reservation> reservations)
        {
            List<ReservationInfo> list = new List<ReservationInfo>();
            foreach (Reservation reservation in reservations)
            {
                int ReservationId = reservation.ReservationId;
                DateTime Date = reservation.Date;
                string Customer = reservation.Customer.FirstName + " " + reservation.Customer.LastName;
                string Cars;
                if (reservation.Cars.Count == 1)
                {
                    Cars = reservation.Cars[0].Name;
                }
                else
                {
                    Cars = reservation.Cars[0].Name + ", " + reservation.Cars[1].Name;
                }
                Arrangement Arrangement = reservation.Arrangement;
                DateTime StartTime = reservation.StartTime;
                int NumberOfHours = reservation.NumberOfHours;
                int NumberOfNighthours = reservation.NumberOfHours;
                Location StartLocation = reservation.StartLocation;
                Location EndLocation = reservation.EndLocation;
                double TotalInclTax = reservation.TotalInclTax;
                double TotalExclTax = reservation.TotalExclTax;
                ReservationInfo reservationInfo = new ReservationInfo(ReservationId, Date, Customer, Cars, Arrangement, StartTime, NumberOfHours, NumberOfNighthours, StartLocation, EndLocation, TotalInclTax, TotalExclTax);
                list.Add(reservationInfo);
            }

            return list;
        }
        public class ReservationInfo
        {
            public ReservationInfo(int reservationId, DateTime date, string customer, string cars, Arrangement arrangement, DateTime startTime, int numberOfHours, int numberOfNighthours, Location startLocation, Location endLocation, double totalInclTax, double totalExclTax)
            {
                ReservationId = reservationId;
                Date = date;
                Customer = customer;
                Cars = cars;
                Arrangement = arrangement;
                StartTime = startTime;
                NumberOfHours = numberOfHours;
                NumberOfNighthours = numberOfNighthours;
                StartLocation = startLocation;
                EndLocation = endLocation;
                TotalInclTax = totalInclTax;
                TotalExclTax = totalExclTax;
            }

            public int ReservationId { get; set; }
            public DateTime Date { get; set; }
            public string Customer { get; set; }
            public string Cars { get; set; }
            public Arrangement Arrangement { get; set; }
            public DateTime StartTime { get; set; }
            public int NumberOfHours { get; set; }
            public int NumberOfNighthours { get; set; }
            public Location StartLocation { get; set; }
            public Location EndLocation { get; set; }
            public double TotalInclTax { get; set; }
            public double TotalExclTax { get; set; }
        }

    }
}
