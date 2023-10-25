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
using DataLayer;
using DataRepo;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for CreateCustomer.xaml
    /// </summary>
    public partial class CreateCustomer : Window
    {
        private string connectionString = "Data Source=HIMEKO\\SQLEXPRESS;Initial Catalog=RentACar;Integrated Security=True";
        private DataManager dataManager;
        private ReservationRepo rr;
        private CarRepo carRepo;
        private LocationAvailabilityRepo locationAvailabilityRepo;
        private CustomerRepo cr;
        private List<Customer> customers = new List<Customer>();

        public CreateCustomer()
        {
            InitializeComponent();
            rr = new ReservationRepo(connectionString);
            carRepo = new CarRepo(connectionString);
            cr = new CustomerRepo(connectionString);
            locationAvailabilityRepo = new LocationAvailabilityRepo(connectionString);
            dataManager = new DataManager(carRepo, cr, locationAvailabilityRepo, rr);
            customers = dataManager.GetCustomer();
        }
        private void CreateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            string firstName;
            string lastName;
            int customerID;
            string street;
            string houseNumber;
            string city;
            string? taxID = null;
            try
            {
                if (FirstNameTextBox.Text != "")
                    firstName = FirstNameTextBox.Text;
                else throw new Exception("Bad First Name");
                if(LastNameTextBox.Text != "")
                    lastName = LastNameTextBox.Text;
                else throw new Exception("Bad Last Name");
                if (CustomerIDTextBox.Text != "")
                {
                    string idasstring = CustomerIDTextBox.Text;
                    if (Int32.TryParse(idasstring, out customerID))
                        foreach(Customer c in customers)
                            if(c.CustomerId == customerID)
                            {
                                customerID = 0;
                                    break;
                            }
                }
                else throw new Exception("Bad Customer ID");
                if(StreetTextBox.Text != "")
                    street = StreetTextBox.Text;
                else throw new Exception("Bad Street");
                if(HouseNumberTextBox.Text != "")
                    houseNumber = HouseNumberTextBox.Text;
                else throw new Exception("Bad House Number");
                if(CityTextBox.Text != "")
                    city = CityTextBox.Text;
                else throw new Exception("Bad City");
                taxID = TaxNumberTextBox.Text;

                customer = new Customer(customerID, firstName, lastName, street, houseNumber, city, taxID);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (customer != null)
            {
                dataManager.SetCustomer(customer);
                MessageBox.Show($"Customer Registered");
            }
            else MessageBox.Show($"Registration Failed");


        }


    }
}
