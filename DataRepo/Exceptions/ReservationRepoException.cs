namespace DataRepo.Exceptions
{
    public class ReservationRepoException : Exception
    {
        public ReservationRepoException(string? message) : base(message)
        {
            Console.WriteLine(message);
        }
    }
}
