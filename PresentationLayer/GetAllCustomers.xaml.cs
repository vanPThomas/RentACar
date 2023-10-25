using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
    /// Interaction logic for GetAllCustomers.xaml
    /// </summary>
    public partial class GetAllCustomers : Window
    {
        private string connectionString = "Data Source=HIMEKO\\SQLEXPRESS;Initial Catalog=RentACar;Integrated Security=True";
        private DataManager dataManager;
        private ReservationRepo rr;
        private CarRepo carRepo;
        private LocationAvailabilityRepo locationAvailabilityRepo;
        private CustomerRepo cr;
        private List<Customer> customers;
        private List<Customer> shortlistcust;
        private int readIndex = 0;

        public GetAllCustomers()
        {
            InitializeComponent();
            FillData();
            shortlistcust = CreateListBasedOnIndex();
            CustomerGrid.ItemsSource = shortlistcust;

        }

        public void FillData()
        {
            rr = new ReservationRepo(connectionString);
            carRepo = new CarRepo(connectionString);
            cr = new CustomerRepo(connectionString);
            locationAvailabilityRepo = new LocationAvailabilityRepo(connectionString);
            dataManager = new DataManager(carRepo, cr, locationAvailabilityRepo, rr);
            customers = dataManager.GetCustomer();
        }
        private List<Customer> CreateListBasedOnIndex()
        {
            List<Customer> list = new List<Customer>();
            int numberofchecks = readIndex + (customers.Count - readIndex);
            if (readIndex + 10 > customers.Count)
                for (int i = readIndex; i < customers.Count; i++)
                    list.Add(customers[i]);
            else
                for (int i = readIndex; i < readIndex + 10; i++)
                    list.Add(customers[i]);
            return list;
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (readIndex > 0)
            {
                readIndex = readIndex - 10;
                shortlistcust = CreateListBasedOnIndex();
                CustomerGrid.ItemsSource = shortlistcust;
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (readIndex + 10 < customers.Count)
            {
                readIndex = readIndex + 10;
                shortlistcust = CreateListBasedOnIndex();
                CustomerGrid.ItemsSource = shortlistcust;
            }
        }
    }
}
