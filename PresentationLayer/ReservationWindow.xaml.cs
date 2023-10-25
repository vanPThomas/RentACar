using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using DataLayer;
using DataRepo;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for ReservationWindow.xaml
    /// </summary>
    public partial class ReservationWindow : Window
    {
        private string connectionString = "Data Source=HIMEKO\\SQLEXPRESS;Initial Catalog=RentACar;Integrated Security=True";
        private DataManager dataManager;
        private ReservationRepo rr;
        private CarRepo carRepo;
        private LocationAvailabilityRepo locationAvailabilityRepo;
        private CustomerRepo cr;
        private List<Reservation> reservations;
        private List<ReservationInfo> reservationInfoList;
        private List<ReservationInfo> reservationInfoTenList;
        private int readIndex = 0;

        public ReservationWindow()
        {
            InitializeComponent();
            fillData();
            reservationInfoList = CreateReservationInfoList(reservations);
            reservationInfoTenList = CreateListBasedOnIndex();
            ReservationGrid.ItemsSource = reservationInfoTenList;
        }
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if(readIndex > 0)
            {
                readIndex = readIndex - 10;
                reservationInfoTenList = CreateListBasedOnIndex();
                ReservationGrid.ItemsSource = reservationInfoTenList;
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if(readIndex + 10  < reservationInfoList.Count) 
            {
                readIndex = readIndex + 10;
                reservationInfoTenList = CreateListBasedOnIndex();
                ReservationGrid.ItemsSource = reservationInfoTenList;
            }
        }
        private void fillData()
        {
            rr = new ReservationRepo(connectionString);
            carRepo = new CarRepo(connectionString);
            cr = new CustomerRepo(connectionString);
            locationAvailabilityRepo = new LocationAvailabilityRepo(connectionString);
            dataManager = new DataManager(carRepo, cr, locationAvailabilityRepo, rr);
            reservations = dataManager.GetReservation();
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
                if(reservation.Cars.Count == 1)
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
        private List<ReservationInfo> CreateListBasedOnIndex()
        {
            List<ReservationInfo> list = new List<ReservationInfo>();
            int numberofchecks = readIndex + (reservationInfoList.Count - readIndex);
            if(readIndex + 10 > reservationInfoList.Count)
            {
                for (int i = readIndex; i < reservationInfoList.Count; i++)
                {
                    list.Add(reservationInfoList[i]);
                }
            }
            else
            {
                for( int i = readIndex; i < readIndex+10; i++)
                {
                    list.Add(reservationInfoList[i]);
                }
            }
            return list;
        }

    }
    public class  ReservationInfo
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
