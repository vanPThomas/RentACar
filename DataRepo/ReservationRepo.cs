using DataLayer;
using DataLayer.Interfaces;
using DataRepo.Exceptions;
using System.Data;
using System.Data.SqlClient;

namespace DataRepo
{
    public class ReservationRepo : IReservationRepo
    {
        private string connectionString;
        public ReservationRepo(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void WriteReservationInDB(Reservation reservation)
        {
            string sql = "insert into Reservation(ReservationDate, CustomerId, Arrangement, CommenceDateTime, NumberOfHours, StartLocation, EndLocation) output inserted.ID values(@ReservationDate,@CustomerId,@Arrangement, @CommenceDateTime, @NumberOfHours, @StartLocation,@EndLocation)";
            string sqlreservIdGet = "select * from Reservation where CustomerId=@CustomerId and ReservationDate=@ReservationDate";
            // string sqlgetid = "select count(*) from Reservation";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    cmd.Transaction = transaction;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@ReservationDate", reservation.Date.ToString());
                    cmd.Parameters.AddWithValue("@CustomerId", reservation.Customer.CustomerId);
                    cmd.Parameters.AddWithValue("@Arrangement", reservation.Arrangement.ToString());
                    cmd.Parameters.AddWithValue("@CommenceDateTime", reservation.StartTime.ToString());
                    cmd.Parameters.AddWithValue("@NumberOfHours", reservation.NumberOfHours);
                    cmd.Parameters.AddWithValue("@StartLocation", reservation.StartLocation.ToString());
                    cmd.Parameters.AddWithValue("@EndLocation", reservation.EndLocation.ToString());

                    int newID = (int)cmd.ExecuteScalar();
                    
                    List<int> carIDs = GetCarIDList(reservation);
                    //WriteCarsInReservation(reservation, carIDs, newID);
            
                try
                {
                    string sqlInsertCars = "insert into CarsInReservation(ReservationID, CarID) values(@ReservationID, @CarID)";
                    string sqlInsertCars2 = "insert into CarsInReservation(ReservationID, CarID) values(@ReservationID, @CarID2)";

                    using (SqlCommand cmd3 = connection.CreateCommand())
                    {
                        //SqlTransaction transaction = connection.BeginTransaction();
                        cmd3.Transaction = transaction;
                        cmd3.Parameters.AddWithValue("@ReservationID", newID);

                        if(carIDs.Count == 1)
                        {
                            cmd3.CommandText = sqlInsertCars;
                            cmd3.Parameters.AddWithValue("@CarID", carIDs[0]);

                            cmd3.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd3.CommandText = sqlInsertCars;
                            cmd3.Parameters.AddWithValue("@CarID", carIDs[0]);

                            cmd3.ExecuteNonQuery();
                            cmd3.CommandText = sqlInsertCars2;
                            cmd3.Parameters.AddWithValue("@CarID2", carIDs[1]);
                            cmd3.ExecuteNonQuery();
                        
                        }
                        //transaction.Commit();

                    transaction.Commit();
                        connection.Close();
                    }

                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw new CarRepoException("Write Car Reservation Error");
                }
            
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw new ReservationRepoException("Reservation writing failure");
                }
            }
        }

        public List<Reservation> GetReservations()
        {
                List<Reservation> reservations = new List<Reservation>();
            try
            {
                CustomerRepo cr = new CustomerRepo(connectionString);
                CarRepo carRepo = new CarRepo(connectionString);
                List<Customer> customers = cr.GetCustomerList();
                List<Car> carList = carRepo.GetCars();
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    connection.Open();
                    string sqlCars = "select * from Reservation";
                    cmd.CommandText = sqlCars;
                    IDataReader reader = cmd.ExecuteReader();
                    Console.WriteLine("x");
                    Dictionary<int, List<int>> reservationIDToCarID = GetReservationIdtoCarId();
                    Console.WriteLine(reservationIDToCarID.Count());
                    while (reader.Read())
                    {
                        int reservationId = (int)reader["id"];
                        DateTime reservationDate = DateTime.Parse((string)reader["ReservationDate"]);
                        Customer reservCustomer = null;
                        foreach(Customer customer in customers)
                        {
                            if (customer.CustomerId == (int)reader["CustomerId"])
                            {
                                reservCustomer = customer;
                                break;
                            }
                        }
                        List<Car> cars = new List<Car>();
                        try
                        {
                            foreach (int carid in reservationIDToCarID[reservationId])
                            {
                                foreach(Car car in carList)
                                {
                                    if(car.Id == carid)
                                    {
                                        cars.Add(car);
                                        break;
                                    }
                                }
                            }

                        }catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Arrangement ar = GetArrangementFromString((string)reader["Arrangement"]);
                        DateTime startTime = DateTime.Parse((string)reader["CommenceDateTime"]);
                        int numberOfHours = (int)reader["NumberOfHours"];
                        Location startLocation = GetLocationFromString((string)reader["StartLocation"]);
                        Location endLocation = GetLocationFromString((string)reader["EndLocation"]);

                        Reservation r = new Reservation(reservationId, reservationDate, reservCustomer, cars, ar, startTime, numberOfHours, startLocation, endLocation);

                        reservations.Add(r);
                    }

                    connection.Close();
                }


            }catch(Exception ex)
            {
                Console.WriteLine("Error");
            }
                return reservations;
        }

        private Dictionary<int, List<int>> GetReservationIdtoCarId()
        {
            Dictionary<int, List<int>> reservationIDToCarID = new Dictionary<int, List<int>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                connection.Open();
                string sqlCars = "select * from CarsInReservation";
                cmd.CommandText = sqlCars;
                IDataReader reader = cmd.ExecuteReader();
                int i = 1;
                while (reader.Read())
                {
                    int reservationId = (int)reader["ReservationID"];
                    int CarId = (int)reader["CarID"];


                    if (reservationIDToCarID.ContainsKey(reservationId))
                    {
                        Console.WriteLine(i);
                        i++;
                        reservationIDToCarID[reservationId].Add(CarId);
                    }
                    else
                    {
                        Console.WriteLine(i);
                        i++;
                        List<int> carIDList = new List<int> { CarId };
                        reservationIDToCarID.Add(reservationId, carIDList);
                    }
                }
                connection.Close();
            }
            return reservationIDToCarID;
        }

        private Dictionary<int, int> GetReservationIdtoCarId(Car c)
        {
            Dictionary<int, int> reservationIDToCarID = new Dictionary<int, int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                connection.Open();
                string sqlCars = "select * from CarsInReservation";
                cmd.CommandText = sqlCars;
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int reservationId = (int)reader["ReservationID"];
                    int CarId = (int)reader["CarID"];

                    
                        
                    reservationIDToCarID.Add(reservationId, CarId);
                    Console.WriteLine(reservationIDToCarID.Count());
                    
                }
                connection.Close();
            }
            return reservationIDToCarID;
        }


        private List<int> GetCarIDList(Reservation reservation)
        {
            List<int> carIDs = new List<int>();
            int CarId;

            foreach (Car car in reservation.Cars)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    connection.Open();
                    string sqlCars = "select * from Car where Name = @Name";
                    cmd.CommandText = sqlCars;
                    cmd.Parameters.AddWithValue("@Name", car.Name);
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (car.Name == (string)reader["Name"])
                        {
                            CarId = (int)reader["Id"];
                            carIDs.Add(CarId);
                        }
                    }

                    connection.Close();
                }
            }
            return carIDs;
        }
        private void WriteCarsInReservation(Reservation reservation, List<int> carIDs, int newID)
        {
        }
        private Arrangement GetArrangementFromString(string ar)
        {
            Arrangement actAr;

            if(Arrangement.Airport.ToString() == ar)
                actAr = Arrangement.Airport;
            else if(Arrangement.Business.ToString() == ar)
                actAr = Arrangement.Business;
            else if (Arrangement.Wedding.ToString() == ar)
                actAr = Arrangement.Wedding;
            else
                actAr = Arrangement.NightLife;
            return actAr;
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
