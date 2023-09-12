
namespace MapLogger
{
    public class LogEntryModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Maptype { get; set; }
    }
}