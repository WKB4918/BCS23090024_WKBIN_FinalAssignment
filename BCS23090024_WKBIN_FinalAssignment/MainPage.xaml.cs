using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using BCS23090024_WKBIN_FinalAssignment.Models;
using BCS23090024_WKBIN_FinalAssignment.Services;

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
        string tripId = entryTripID.Text;

        if (string.IsNullOrWhiteSpace(tripId))
        {
            entryTripID.PlaceholderColor = Colors.Red; 
            await DisplayAlert("Validation Failed", "Trip ID is required.", "OK");
            return;
        }

        entryTripID.PlaceholderColor = Colors.Gray;

        var newTrip = new FlightTrip
        {
            TripID = tripId,
            LocationData = $"Lat: {lblLatitude.Text}, Long: {lblLongitude.Text}"
        };

        var dbService = new DatabaseService();
        await dbService.SaveTripAsync(newTrip);

        await DisplayAlert("Success", "Flight data recorded offline!", "OK");

        entryTripID.Text = string.Empty;
    }
}