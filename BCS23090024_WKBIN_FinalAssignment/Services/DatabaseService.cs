using SQLite;
using BCS23090024_WKBIN_FinalAssignment.Models;

namespace BCS23090024_WKBIN_FinalAssignment.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        async Task Init()
        {
            if (_database is not null) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AircraftData.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<FlightTrip>();
        }

        public async Task<int> SaveTripAsync(FlightTrip trip)
        {
            await Init();
            return await _database!.InsertAsync(trip);
        }
        public async Task<List<FlightTrip>> GetTripsAsync()
        {
            await Init();
            return await _database!.Table<FlightTrip>().ToListAsync();
        }
    }
}