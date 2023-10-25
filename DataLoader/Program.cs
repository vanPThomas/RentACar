using DataLayer;
using DataRepo;
using System.Data.SqlClient;

namespace DataLoader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=HIMEKO\\SQLEXPRESS;Initial Catalog=RentACar;Integrated Security=True";
            Console.WriteLine("--Start--");
            ClearDatabases(connectionString);
            ReadCarsFromFiles(connectionString);
            ReadCustomersFromFiles(connectionString);
            ReadAvailabilityFromFile(connectionString);
            ReadReservationsFromFile(connectionString);
            Console.WriteLine("--end--");
        }


        private static void ReadCarsFromFiles(string cs)
        {

            CarRepo cr = new CarRepo(cs);
            using (StreamReader sr = File.OpenText($"cardata.csv"))
            {
                string input = null;
                string[] inputArray;
                while ((input = sr.ReadLine()) != null)
                {
                    string[] cardata = input.Split(',');
                    Car car = new Car(cardata[0], double.Parse(cardata[1]), double.Parse(cardata[2]), double.Parse(cardata[3]), int.Parse(cardata[4]));
                    cr.WriteCarInDB(car);
                }
            }
        }

        private static void ReadCustomersFromFiles(string cs)
        {
            CustomerRepo cr = new CustomerRepo(cs);
            using (StreamReader sr = File.OpenText($"customerdata.csv"))
            {
                string input = null;
                string[] inputArray;
                while ((input = sr.ReadLine()) != null)
                {
                    string[] Customerdata = input.Split(',');
                    if (Customerdata[6] == "none")
                    {
                        Customer c = new Customer(int.Parse(Customerdata[0]), Customerdata[1], Customerdata[2], Customerdata[3], Customerdata[4], Customerdata[5], null);
                        cr.WriteCustomerInDB(c);
                    }
                    else
                    {
                        Customer c = new Customer(int.Parse(Customerdata[0]), Customerdata[1], Customerdata[2], Customerdata[3], Customerdata[4], Customerdata[5], Customerdata[6]);
                        cr.WriteCustomerInDB(c);
                    }
                }
            }
        }

        private static void ReadAvailabilityFromFile(string cs)
        {
            List<Car> cars = new List<Car>();
            using (StreamReader sr = File.OpenText($"cardata.csv"))
            {
                string input = null;
                string[] inputArray;
                while ((input = sr.ReadLine()) != null)
                {
                    string[] cardata = input.Split(',');
                    Car car = new Car(cardata[0], double.Parse(cardata[1]), double.Parse(cardata[2]), double.Parse(cardata[3]), int.Parse(cardata[4]));
                    cars.Add(car);
                }
            }

            List<LocationAvailability> laList = new List<LocationAvailability>();
            LocationAvailability laGent = new LocationAvailability();
            laGent.Location = Location.Gent;
            LocationAvailability laAntwerpen = new LocationAvailability();
            laAntwerpen.Location = Location.Antwerpen;
            LocationAvailability laBrussel = new LocationAvailability();
            laBrussel.Location = Location.Brussel;
            LocationAvailability laHasselt = new LocationAvailability();
            laHasselt.Location = Location.Hasselt;
            LocationAvailability laCharleroi = new LocationAvailability();
            laCharleroi.Location = Location.Charleroi;

            using (StreamReader sr = File.OpenText($"availabilitydata.csv"))
            {
                string input = null;
                string[] inputArray;
                while ((input = sr.ReadLine()) != null)
                {
                    string[] availabilitydata = input.Split(',');
                    Car c = null;
                    foreach (Car car in cars)
                    {
                        if (car.Name == availabilitydata[1])
                        {
                            c = car;
                            break;
                        }
                    }

                    if (availabilitydata[0] == "Gent")
                        laGent.CarsToAmount.Add(c, int.Parse(availabilitydata[2]));
                    else if (availabilitydata[0] == "Antwerpen")
                        laAntwerpen.CarsToAmount.Add(c, int.Parse(availabilitydata[2]));
                    else if (availabilitydata[0] == "Brussel")
                        laBrussel.CarsToAmount.Add(c, int.Parse(availabilitydata[2]));
                    else if (availabilitydata[0] == "Hasselt")
                        laHasselt.CarsToAmount.Add(c, int.Parse(availabilitydata[2]));
                    else if (availabilitydata[0] == "Charleroi")
                        laCharleroi.CarsToAmount.Add(c, int.Parse(availabilitydata[2]));

                }
            }
            laList.Add(laGent);
            laList.Add(laAntwerpen);
            laList.Add(laBrussel);
            laList.Add(laCharleroi);
            laList.Add(laHasselt);

            LocationAvailabilityRepo lar = new LocationAvailabilityRepo(cs);
            foreach (LocationAvailability la in laList)
            {
                lar.WriteLocationAvailabilityInDB(la);
            }


        }

        private static void ReadReservationsFromFile(string cs)
        {
            ReservationRepo rr = new ReservationRepo(cs);

            List<Car> cars = new List<Car>();
            using (StreamReader sr = File.OpenText($"cardata.csv"))
            {
                string input = null;
                string[] inputArray;
                while ((input = sr.ReadLine()) != null)
                {
                    string[] cardata = input.Split(',');
                    Car car = new Car(cardata[0], double.Parse(cardata[1]), double.Parse(cardata[2]), double.Parse(cardata[3]), int.Parse(cardata[4]));
                    cars.Add(car);
                }
            }

            List<Customer> customers = new List<Customer>();
            using (StreamReader sr = File.OpenText($"customerdata.csv"))
            {
                string input = null;
                string[] inputArray;
                while ((input = sr.ReadLine()) != null)
                {
                    string[] Customerdata = input.Split(',');
                    if (Customerdata[6] == "none")
                    {
                        Customer c = new Customer(int.Parse(Customerdata[0]), Customerdata[1], Customerdata[2], Customerdata[3], Customerdata[4], Customerdata[5], null);
                        customers.Add(c);
                    }
                    else
                    {
                        Customer c = new Customer(int.Parse(Customerdata[0]), Customerdata[1], Customerdata[2], Customerdata[3], Customerdata[4], Customerdata[5], Customerdata[6]);
                        customers.Add(c);
                    }
                }
            }

            using (StreamReader sr = File.OpenText($"reservationdata.csv"))
            {
                string input = null;
                while ((input = sr.ReadLine()) != null)
                {
                    string[] reservationdata = input.Split(',');
                    List<Car> reservedcars = new List<Car>();
                    for (int i = 7; i < reservationdata.Length; i++)
                        foreach (Car car in cars)
                            if (car.Name == reservationdata[i])
                                reservedcars.Add(car);

                    Arrangement a;
                    if (reservationdata[2] == Arrangement.Airport.ToString())
                        a = Arrangement.Airport;
                    else if (reservationdata[2] == Arrangement.NightLife.ToString())
                        a = Arrangement.NightLife;
                    else if (reservationdata[2] == Arrangement.Business.ToString())
                        a = Arrangement.Business;
                    else
                        a = Arrangement.Wedding;
                    Customer c = null;
                    foreach (Customer customer in customers)
                        if (int.Parse(reservationdata[1]) == customer.CustomerId)
                            c = customer;

                    Location startLoc;
                    if (reservationdata[5] == Location.Gent.ToString())
                        startLoc = Location.Gent;
                    else if (reservationdata[5] == Location.Antwerpen.ToString())
                        startLoc = Location.Antwerpen;
                    else if (reservationdata[5] == Location.Brussel.ToString())
                        startLoc = Location.Brussel;
                    else if (reservationdata[5] == Location.Charleroi.ToString())
                        startLoc = Location.Charleroi;
                    else
                        startLoc = Location.Hasselt;

                    Location endLoc;
                    if (reservationdata[6] == Location.Gent.ToString())
                        endLoc = Location.Gent;
                    else if (reservationdata[6] == Location.Antwerpen.ToString())
                        endLoc = Location.Antwerpen;
                    else if (reservationdata[6] == Location.Brussel.ToString())
                        endLoc = Location.Brussel;
                    else if (reservationdata[6] == Location.Charleroi.ToString())
                        endLoc = Location.Charleroi;
                    else
                        endLoc = Location.Hasselt;

                    Reservation r = new Reservation(DateTime.Parse(reservationdata[0]), c, reservedcars, a, DateTime.Parse(reservationdata[3]), int.Parse(reservationdata[4]), startLoc, endLoc);

                    rr.WriteReservationInDB(r);

                }
            }
        }
        private static void ClearDatabases(string cs)
        {
            string sql1 = "Delete From Car";
            string sql2 = "Delete From Customer";
            string sql3 = "Delete From Reservation";
            string sql4 = "Delete From CarsInReservation";
            string sql5 = "Delete From Location";
            string sql6 = "DBCC CHECKIDENT (Car, RESEED, 0)";
            string sql8 = "DBCC CHECKIDENT (Reservation, RESEED, 0)";

            using (SqlConnection connection = new SqlConnection(cs))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                connection.Open();
                cmd.CommandText = sql4;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql5;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql3;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql1;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql2;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql6;
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql8;
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}