namespace DataLayer.Interfaces
{
    public interface IReservationRepo
    {
        List<Reservation> GetReservations();
        void WriteReservationInDB(Reservation reservation);

    }
}
