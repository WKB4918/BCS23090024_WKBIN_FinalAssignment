using SQLite;

namespace BCS23090024_WKBIN_FinalAssignment.Models
{
    public class FlightTrip
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TripID { get; set; } = string.Empty;
        public string LocationData { get; set; } = string.Empty;
    }
}