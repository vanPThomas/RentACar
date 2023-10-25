namespace DataLayer.Interfaces
{
    public interface ICarRepo
    {
        void WriteCarInDB(Car car);
        List<Car> GetCars();
        Car GetCarById(int carId);
    }
}
