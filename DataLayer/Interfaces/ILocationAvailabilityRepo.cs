namespace DataLayer.Interfaces
{
    public interface ILocationAvailabilityRepo
    {
        void WriteLocationAvailabilityInDB(LocationAvailability la);
        List<LocationAvailability> GetLocationAvailability();


    }
}
