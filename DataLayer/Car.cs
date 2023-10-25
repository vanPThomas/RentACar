namespace DataLayer
{
    public class Car
    {
        public Car(string name, double firstHourPrice, double nightLifePrice, double weddingPrice, int buildYear)
        {

            Name = name;
            FirstHourPrice = firstHourPrice;
            NightLifePrice = nightLifePrice;
            WeddingPrice = weddingPrice;
            BuildYear = buildYear;
        }

        public Car(int id, string name, double firstHourPrice, double nightLifePrice, double weddingPrice, int buildYear)
        {
            Id = id;
            Name = name;
            FirstHourPrice = firstHourPrice;
            NightLifePrice = nightLifePrice;
            WeddingPrice = weddingPrice;
            BuildYear = buildYear;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double FirstHourPrice { get; set; }
        public double NightLifePrice { get; set; }
        public double WeddingPrice { get; set; }
        public int BuildYear { get; set; }

    }
}
