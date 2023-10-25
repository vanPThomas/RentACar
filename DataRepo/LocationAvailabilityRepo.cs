using DataLayer;
using DataLayer.Interfaces;
using DataRepo.Exceptions;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace DataRepo
{
    public class LocationAvailabilityRepo : ILocationAvailabilityRepo
    {
        private string connectionString;
        public LocationAvailabilityRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void WriteLocationAvailabilityInDB(LocationAvailability la)
        {

            foreach(Car car in la.CarsToAmount.Keys)
            {
                string sql = "insert into Location(Location, CarId, Available) values(@Location,@CarId,@Available)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    try
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        cmd.Transaction = transaction;
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@Location", la.Location.ToString());

                        int carId = ReadCarInDBGiveID(car.Name);

                        cmd.Parameters.AddWithValue("@CarId", carId);
                        cmd.Parameters.AddWithValue("@Available", la.CarsToAmount[car]);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Location Error");
                        Console.WriteLine(ex.ToString());
                        throw new LocationAvailabilityRepoException("Location Writing Error");
                    }
                }
            }
        }
        public List<LocationAvailability> GetLocationAvailability()
        {
            List<LocationAvailability> LocationToCarAvailability = new List<LocationAvailability>();
            CarRepo carRepo = new CarRepo(connectionString);
            LocationAvailability laGent = new LocationAvailability(Location.Gent);
            LocationAvailability laHasselt = new LocationAvailability(Location.Hasselt);
            LocationAvailability laBrussel = new LocationAvailability(Location.Brussel);
            LocationAvailability laAntwerpen = new LocationAvailability(Location.Antwerpen);
            LocationAvailability laCharleroi = new LocationAvailability(Location.Charleroi);
            LocationToCarAvailability.Add(laGent);
            LocationToCarAvailability.Add(laBrussel);
            LocationToCarAvailability.Add(laCharleroi);
            LocationToCarAvailability.Add(laAntwerpen);
            LocationToCarAvailability.Add(laHasselt);

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                connection.Open();
                string sqlCustomer = "select * from Location";
                cmd.CommandText = sqlCustomer;
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Location location = GetLocationFromString((string)reader["Location"]);
                    foreach(LocationAvailability la in LocationToCarAvailability)
                    {
                        if(la.Location == location)
                        {
                            Car car = carRepo.GetCarById((int)reader["CarID"]);
                            la.CarsToAmount.Add(car, (int)reader["Available"]);
                        }
                    }
                }
                connection.Close();
            }
            return LocationToCarAvailability;
        }

        private int ReadCarInDBGiveID(string Name)
        {
            int carId = 0;

            string sql = "select * from Car";
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@Name", Name);
                    IDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if ((string)reader["Name"] == Name)
                        {
                            carId = (int)reader["Id"];
                            break;
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    //throw new TeamRepositoryException("SelecteerTeam", ex);
                }
                return carId;
            }
        }
        private Location GetLocationFromString(string loc)
        {
            Location location;
            if (Location.Hasselt.ToString() == loc)
                location = Location.Hasselt;
            else if (Location.Brussel.ToString() == loc)
                location = Location.Brussel;
            else if (Location.Charleroi.ToString() == loc)
                location = Location.Charleroi;
            else if (Location.Gent.ToString() == loc)
                location = Location.Gent;
            else
                location = Location.Antwerpen;
            return location;
        }

    }
}
