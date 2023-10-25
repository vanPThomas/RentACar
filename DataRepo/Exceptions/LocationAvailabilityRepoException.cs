namespace DataRepo.Exceptions
{
    public class LocationAvailabilityRepoException : Exception
    {
        public LocationAvailabilityRepoException(string? message) : base(message)
        {
            Console.WriteLine(message);
        }
    }
}
