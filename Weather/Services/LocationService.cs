using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace Weather.Services
{
    public class LocationService
    {
        CancellationTokenSource cts;
        
        public LocationService()
        {
        }

        public async void GetCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Console.WriteLine("Latitude: " + location.Latitude.ToString());
                    Console.WriteLine("Longitude:" + location.Longitude.ToString());
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

        //protected override void OnDisappearing()
        //{
        //    if (cts != null && !cts.IsCancellationRequested)
        //        cts.Cancel();
        //    object p = base.OnDisappearing();
        //}
    }
}
