using System.Windows;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationWindow reservationWindow = new ReservationWindow();
            reservationWindow.Show();
        }

        private void SearchCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            SearchCustomer searchCustomer = new SearchCustomer();
            searchCustomer.Show();
        }
        private void GetAllCustomersButton_Click(object sender, RoutedEventArgs e)
        {
            GetAllCustomers getAllCustomers = new GetAllCustomers();
            getAllCustomers.Show();
        }
        private void CreateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            CreateCustomer createCustomer = new CreateCustomer();
            createCustomer.Show();
        }
    }
}
