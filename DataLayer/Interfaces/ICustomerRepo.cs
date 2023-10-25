namespace DataLayer.Interfaces
{
    public interface ICustomerRepo
    {
        void WriteCustomerInDB(Customer customer);
        List<Customer> GetCustomerList();
    }
}
