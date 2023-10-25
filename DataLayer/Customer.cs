using System.IO;

namespace DataLayer
{
    public  class Customer
    {
        public Customer(int customerId, string firstName, string lastName, string street, string houseNumber, string city, string? taxNumber)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
            Street = street;
            HouseNumber = houseNumber;
            City = city; 
            TaxNumber = taxNumber;
        }

        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string City { get; set; }
        public string? TaxNumber { get; set; } //if available
    }
}