namespace DataRepo.Exceptions
{
    public class CustomerRepoException : Exception
    {
        public CustomerRepoException(string? message) : base(message)
        {
            Console.WriteLine(message);
        }
    }
}
