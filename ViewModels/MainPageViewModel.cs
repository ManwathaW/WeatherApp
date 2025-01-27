using System;
using WeatherApp.Services;
using WeatherApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WeatherApp.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly WeatherService _weatherService;
    private readonly IGeolocation _geolocation;
    private WeatherData? _weatherData;
    private bool _isLoading;

    public MainPageViewModel(WeatherService weatherService, IGeolocation geolocation)
    {
        _weatherService = weatherService;
        _geolocation = geolocation;
        RefreshCommand = new Command(async () => await RefreshWeatherAsync());

        // Automatically load weather data when the view model is created
        MainThread.BeginInvokeOnMainThread(async () => await RefreshWeatherAsync());
    }

    public ICommand RefreshCommand { get; }

    public WeatherData? WeatherData
    {
        get => _weatherData;
        set
        {
            if (_weatherData != value)
            {
                _weatherData = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public async Task RefreshWeatherAsync()
    {
        try
        {
            IsLoading = true;

            // Check if location is enabled
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await ShowAlert("Permission Error", "Location permission is required to fetch weather data.");
                    return;
                }
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
            var location = await _geolocation.GetLocationAsync(request);

            if (location == null)
            {
                await ShowAlert("Location Error", "Could not get current location.");
                return;
            }

            Console.WriteLine($"Location obtained: {location.Latitude}, {location.Longitude}");
            WeatherData = await _weatherService.GetWeatherDataAsync(location.Latitude, location.Longitude);
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to get weather data: {ex.Message}");
            Console.WriteLine($"Error in RefreshWeatherAsync: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ShowAlert(string title, string message)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            await Application.Current.Windows[0].Page.DisplayAlert(title, message, "OK");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}