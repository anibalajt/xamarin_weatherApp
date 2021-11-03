using System;
namespace Weather.Models
{
    public class City
    {
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string CityName { get; set; }
        public string Country { get; set; }
        public bool Active { get; set; }
    }
}
