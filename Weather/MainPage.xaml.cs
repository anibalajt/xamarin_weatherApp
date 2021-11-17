using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Globalization;
using Weather.Services;
using Weather.Models;

namespace Weather
{
    public partial class MainPage : ContentPage
    {
        ApiService Api;
        LocationService LTS;
        WeatherObject weatherObject;
        float ConstDegrees = 273.15f;
        SearchCity SearchCity = new SearchCity();
        FileService FileDB = new FileService();
        
        public MainPage()
        {
            InitializeComponent();

            Api = new ApiService();

            if (FileDB.FileExists())
            {

                List<City> ListCity = FileDB.OpenFile();
                foreach (City city in ListCity)
                {
                    if (city.Active)
                    {
                        GetWeather(city.Lat, city.Lon, false);
                        break;
                    }
                }
            }
            else
            {
                FileDB.CreateFile();
                GetCurrentGeolocation();
            }
            string d = Preferences.Get("Degree", "c");
            if (d == "c")
            {
                iconCelsius.Opacity = 1;
                iconFahrenheit.Opacity = 0;
            }
            else
            {
                iconCelsius.Opacity = 0;
                iconFahrenheit.Opacity = 1;
            }
        }

        protected override void OnAppearing()
        {
            //get data from modalView after closed it
            if (SearchCity.lon != null && SearchCity.lat != null)
            {
                GetWeather(SearchCity.lat, SearchCity.lon, false);
            }
            base.OnAppearing();
        }
        void OnSettinButtonClicked(object sender, EventArgs e)
        {
            modalSetting.IsVisible = !modalSetting.IsVisible;
            backgroundModal.IsVisible = !backgroundModal.IsVisible;
        }
        async void OnNavigateButtonClicked(object sender, EventArgs e)
        {
            SearchCity.EditListCity = false;
            await Navigation.PushModalAsync(SearchCity);
        }
        async void OnEditListCityClicked(object sender, EventArgs e)
        {
            SearchCity.EditListCity = true;
            await Navigation.PushModalAsync(SearchCity);
            modalSetting.IsVisible = false;
            backgroundModal.IsVisible = false;
        }
        void OnDegreesClicked(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            if (label.Text == "Celsius")
            {
                iconCelsius.Opacity = 1;
                iconFahrenheit.Opacity = 0;
                Preferences.Set("Degree", "c");
            }
            else
            {
                iconCelsius.Opacity = 0;
                iconFahrenheit.Opacity = 1;
                Preferences.Set("Degree", "f");
            }
            updateWeather();
            modalSetting.IsVisible = false;
            backgroundModal.IsVisible = false;
        }
        private async void GetCurrentGeolocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    string lat = location.Latitude.ToString();
                    string lon = location.Longitude.ToString();
                    GetWeather(lat, lon, true);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                Console.WriteLine("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Faild", ex.Message, "OK");
            }
        }
        async void GetWeather(string lat, string lon, bool init)
        {
            string content = await Api.GetWeatherpByLonLat(lat, lon);
            weatherObject = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherObject>(content);
            if (init)
            {
                City firstCity = new City() { Active = true, Lat = weatherObject.coord.lat.ToString(), Lon = weatherObject.coord.lon.ToString(), CityName = weatherObject.name, Country = weatherObject.sys.country };

                FileDB.WriteFile(firstCity, true);
            }
            updateWeather();
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            contentView.WidthRequest = width;
            fullBackground.WidthRequest = width;
            fullBackground.HeightRequest = height;

            //backgroundModal size fullsize
            backgroundModal.WidthRequest = width;
            backgroundModal.HeightRequest = height;

            if (width > height)
            {
                temperature.FontSize = 40;
                if (Device.RuntimePlatform == Device.iOS)
                {
                    footerView.Padding = new Thickness(50, 0, 20, 20);
                    nameCityView.Padding = new Thickness(50, 10, 20, 0);
                }

            }
            else
            {
                temperature.FontSize = 90;
                footerView.Padding = new Thickness(20, 0, 20, 20);
                nameCityView.Padding = new Thickness(20, 10, 20, 0);
            }
        }
        private void updateWeather()
        {
            city.Text = weatherObject.name;

            string d = Preferences.Get("Degree", "c");
            if (d == "c")
            {
                temperature.Text = String.Format("{0}°C", (weatherObject.main.temp - ConstDegrees).ToString("N0"));
            }
            else
            {
                temperature.Text = String.Format("{0}°F", ((weatherObject.main.temp - ConstDegrees) * 9 / 5 + 32).ToString("N0"));
            }
            weatherDescription.Text = weatherObject.weather[0].main;
            lblHumidity.Text = weatherObject.main.humidity.ToString();
            string[] dateTime = UnixTimeStampToDateTime(weatherObject.dt, 1);
            lblTime.Text = dateTime[0];

            dateTime = UnixTimeStampToDateTime(weatherObject.sys.sunrise, 2);
            lblSunrise.Text = dateTime[0];
            lblAmSunrise.Text = dateTime[1];
            dateTime = UnixTimeStampToDateTime(weatherObject.sys.sunset, 2);
            lblSunset.Text = dateTime[0];
            lblAmPmSunset.Text = dateTime[1];

            int code = weatherObject.weather[0].id;
            string fileExtencion = (Device.RuntimePlatform == Device.Android) ? "":".jpeg";
            //Thunderstorm      200 - 232
            if (code >= 200 && code <= 232)
            {
                fullBackground.Source = ImageSource.FromFile($"thunderstorm{fileExtencion}");
            }
            //Drizzle           300 - 321
            if (code >= 300 && code <= 321)
            {
                fullBackground.Source = ImageSource.FromFile($"drizzle{fileExtencion}");
            }
            //Rain              500 - 531
            if (code >= 500 && code <= 531)
            {
                fullBackground.Source = ImageSource.FromFile($"rain{fileExtencion}");
            }
            
            //Clear             800
            if (code == 800)
            {
                fullBackground.Source = ImageSource.FromFile($"clear{fileExtencion}");
            }
            fileExtencion = (Device.RuntimePlatform == Device.Android) ? "" : ".jpg";
            //Snow              600 - 622
            if (code >= 600 && code <= 622)
            {
                fullBackground.Source = ImageSource.FromFile($"snow{fileExtencion}");
            }
            //Clouds            801 - 804
            if (code >= 801 && code <= 804)
            {
                fullBackground.Source = ImageSource.FromFile($"clouds{fileExtencion}");
            }

        }
        public static string[] UnixTimeStampToDateTime(double unixTimeStamp, int type)
        {
            // Unix timestamp is seconds past epoch
            //3:20pm - Wednesday, 26 May 2021
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            string dt = dateTime.ToString();
            switch (type)
            {
                case 1:
                    return new string[] { dateTime.ToString("hh:mmtt - dddd, dd MMM yyyy") };
                default:
                    return new string[] { dateTime.ToString("hh:mm"), dateTime.ToString("tt") };
            }
        }

    }
}
