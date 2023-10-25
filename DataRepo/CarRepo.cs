using System.Data;
using System.Data.SqlClient;
using DataLayer;
using DataLayer.Interfaces;
using DataRepo.Exceptions;

namespace DataRepo
{
    public class CarRepo : ICarRepo
    {
        private string connectionString;
        public CarRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void WriteCarInDB(Car car)
        {
            string sql = "insert into car(Name,FirstHourPrice,NightLifePrice,WeddingPrice,BuildYear) values(@Name,@FirstHourPrice,@NightLifePrice,@WeddingPrice,@BuildYear)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@Name", car.Name);
                    cmd.Parameters.AddWithValue("@FirstHourPrice", car.FirstHourPrice);
                    cmd.Parameters.AddWithValue("@NightLifePrice", car.NightLifePrice);
                    cmd.Parameters.AddWithValue("@WeddingPrice", car.WeddingPrice);
                    cmd.Parameters.AddWithValue("@BuildYear", car.BuildYear);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                }
                catch (Exception ex)
                { 
                    Console.WriteLine(ex.ToString());
                    throw new CarRepoException("Car Writing Error");
                 }
            }
        }

        public List<Car> GetCars()
        {
            List<Car> list = new List<Car>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                connection.Open();
                string sqlCustomer = "select * from Car";
                cmd.CommandText = sqlCustomer;
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int Id = (int)reader["Id"];
                    string Name = (string)reader["Name"];
                    int FirstHourPrice = (int)reader["FirstHourPrice"];
                    int NightLifePrice = (int)reader["NightLifePrice"];
                    int WeddingPrice = (int)reader["WeddingPrice"];
                    int BuildYear = (int)reader["BuildYear"];

                    Car c = new Car(Id, Name, FirstHourPrice,NightLifePrice, WeddingPrice,BuildYear);
                    list.Add(c);
                }

                connection.Close();
            }

            return list;
        }
        public Car GetCarById(int carId)
        {
            Car returnCar = null;
            List<Car> carList = GetCars();

            foreach(Car car in carList)
            {
                if(car.Id == carId)
                {
                    returnCar = car;
                }
            }
            
            return returnCar;
        }

    }
}