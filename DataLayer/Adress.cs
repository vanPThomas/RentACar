namespace DataLayer
{
    public class Adress
    {
        public Adress(string street, string houseNumber, string city)
        {
            Street = street;
            HouseNumber = houseNumber;
            City = city;
        }

        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string City { get; set; }

    }
}
