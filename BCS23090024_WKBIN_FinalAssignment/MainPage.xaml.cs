using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using BCS23090024_WKBIN_FinalAssignment.Models;
using BCS23090024_WKBIN_FinalAssignment.Services;
using System.Text.RegularExpressions;

namespace BCS23090024_WKBIN_FinalAssignment;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await FetchAircraftData();
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
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnRecordTripClicked(object sender, EventArgs e)
    {
        string tripId = entryTripID.Text?.Trim() ?? "";

        string pattern = @"^[a-zA-Z]{2}[0-9]{3,4}$"; //formula is here
        if (string.IsNullOrWhiteSpace(tripId) || !Regex.IsMatch(tripId, pattern))
        {
            entryTripID.Text = string.Empty;

            entryTripID.PlaceholderColor = Colors.Red;

            entryTripID.Placeholder = "Invalid ID! (e.g., AB123)";

            await DisplayAlert("Validation Failed", "Trip ID must be 2 letters followed by 3-4 numbers.", "OK");

            return; 
        }

        entryTripID.PlaceholderColor = Colors.Gray;
        entryTripID.Placeholder = "Enter Flight ID (e.g., OD1906)";
        entryTripID.TextColor = Colors.Black;

        var newTrip = new FlightTrip
        {
            TripID = tripId.ToUpper(),
            LocationData = $"Lat: {lblLatitude.Text}, Long: {lblLongitude.Text}"
        };

        var dbService = new DatabaseService();
        await dbService.SaveTripAsync(newTrip);

        await DisplayAlert("Success", "Flight data recorded offline!", "OK");

        entryTripID.Text = string.Empty;
    }
}