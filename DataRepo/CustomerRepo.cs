using DataLayer;
using DataLayer.Interfaces;
using DataRepo.Exceptions;
using System.Data;
using System.Data.SqlClient;

namespace DataRepo
{
    public class CustomerRepo : ICustomerRepo
    {
        private string connectionString;
        public CustomerRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void WriteCustomerInDB(Customer customer)
        {
            string sql = "insert into Customer(CustomerId,FirstName,LastName,TaxNumber, Street, HouseNumber, City) values(@CustomerId,@FirstName,@LastName,@TaxNumber,@Street, @HouseNumber, @City)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@Street", customer.Street);
                    cmd.Parameters.AddWithValue("@HouseNumber", customer.HouseNumber);
                    cmd.Parameters.AddWithValue("@City", customer.City);
                    if (customer.TaxNumber == null)
                        cmd.Parameters.AddWithValue("@TaxNumber", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@TaxNumber", customer.TaxNumber);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw new CustomerRepoException("Customer Writing Error");
                }
            }
        }

        public List<Customer> GetCustomerList()
        {
            List<Customer> list = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                connection.Open();
                string sqlCustomer = "select * from Customer";
                cmd.CommandText = sqlCustomer;
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int customerId = (int)reader["CustomerId"];
                    string firstName = (string)reader["FirstName"];
                    string lastName = (string)reader["LastName"];
                    string houseNumber = (string)reader["HouseNumber"];
                    string street = (string)reader["Street"];
                    string city = (string)reader["City"];
                    int? taxNumber = !Convert.IsDBNull(reader["TaxNumber"]) ? (int?)reader["TaxNumber"] : null;
                    Customer c = new Customer(customerId, firstName, lastName, street, houseNumber, city, taxNumber.ToString());
                    list.Add(c);
                }

                connection.Close();
            }

            return list;
        }
    }
}
