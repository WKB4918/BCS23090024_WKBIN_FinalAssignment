using BCS23090024_WKBIN_FinalAssignment.Models;
using BCS23090024_WKBIN_FinalAssignment.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace BCS23090024_WKBIN_FinalAssignment;

public partial class MainPage : ContentPage
{
    public ObservableCollection<FlightTrip> FlightList { get; set; } = new ObservableCollection<FlightTrip>();
    private DatabaseService _dbService = new DatabaseService();

    public MainPage()
    {
        InitializeComponent();

        FlightsCollection.ItemsSource = FlightList;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await FetchAircraftData();
        await LoadSavedData(); 
    }

    private async Task LoadSavedData()
    {
        try
        {
            var savedTrips = await _dbService.GetTripsAsync();
            FlightList.Clear();
            foreach (var trip in savedTrips)
            {
                FlightList.Insert(0, trip); 
            }
        }
        catch {}
    }

    private async Task FetchAircraftData()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
            if (location != null)
            {
                lblLatitude.Text = location.Latitude.ToString("F6");
                lblLongitude.Text = location.Longitude.ToString("F6");
            }

            var access = Connectivity.Current.NetworkAccess;
            lblConnectivity.Text = access.ToString();
            lblConnectivity.TextColor = (access == NetworkAccess.Internet) ? Colors.Green : Colors.Red;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "GPS Error: " + ex.Message, "OK");
        }
    }

    private async void OnRecordTripClicked(object sender, EventArgs e)
    {
        string tripId = entryTripID.Text?.Trim().ToUpper() ?? "";

        string pattern = @"^[A-Z]{2}[0-9]{3,4}$";
        if (string.IsNullOrWhiteSpace(tripId) || !Regex.IsMatch(tripId, pattern))
        {
            entryTripID.Text = string.Empty;
            entryTripID.PlaceholderColor = Colors.Red;
            entryTripID.Placeholder = "Invalid ID (e.g., AB123)";
            await DisplayAlert("Validation Failed", "Use 2 letters followed by 3-4 numbers.", "OK");
            return;
        }
        entryTripID.PlaceholderColor = Colors.Gray;

        var newTrip = new FlightTrip
        {
            TripID = tripId,
            LocationData = $"Lat: {lblLatitude.Text}, Long: {lblLongitude.Text}",
            Timestamp = DateTime.Now
        };

        try
        {
            await _dbService.SaveTripAsync(newTrip);

            FlightList.Insert(0, newTrip);

            await DisplayAlert("Success", $"Flight {tripId} recorded!", "OK");
            entryTripID.Text = string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Database Error", ex.Message, "OK");
        }
    }
}