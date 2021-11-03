using System;
namespace Weather.Models
{
    public class Sys
    {

        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public double sunrise { get; set; }
        public double sunset { get; set; }

        public Sys()
        {
        }
    }
}
