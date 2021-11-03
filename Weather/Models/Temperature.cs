using System;
namespace Weather.Models
{
    public class Temperature
    {
        //Unit Default: Kelvin
        public float temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public float feels_like { get; set; }

        public Temperature()
        {
        }
    }
}
