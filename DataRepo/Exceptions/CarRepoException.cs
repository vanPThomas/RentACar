namespace DataRepo.Exceptions
{
    public class CarRepoException : Exception
    {
        public CarRepoException(string? message) : base(message)
        {
            Console.WriteLine(message);
        }
    }
}
