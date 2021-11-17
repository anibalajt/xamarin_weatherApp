using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Weather.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using Weather.Models;

namespace Weather
{
    public partial class SearchCity : ContentPage
    {
        ApiService api;
        List<City> listSearchCity =new List<City>();
        List<City> listCity = new List<City>();
        FileService FileDB = new FileService();
        public bool EditListCity = false;
        public string lat { get; set; }
        public string lon { get; set; }
        public SearchCity()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            listCity = FileDB.OpenFile();;
            FillListView(listCity);
            base.OnAppearing();
        }
        void FillListView(List<City> listCity)
        {
            ListCities.Children.Clear();
            foreach (City city in listCity)
            {
                Grid grid = new Grid();
                grid.Children.Add(new Label()
                {
                    Text = city.CityName,
                }, 0, 0);
                grid.Children.Add(new Label()
                {
                    Text = city.Country,
                    FontSize = 12
                }, 0, 1);
                //add delete button for the whole city, excluding the active city
                if (EditListCity && !city.Active) {
                    // add event click to remove city from file
                    Button btn = new Button();
                    btn.Text = "Remove";
                    btn.Clicked += (object sender, EventArgs e) => {
                        RemoveCity(city);
                    };
                    grid.Children.Add(btn, 1, 0);
                }
                //add event click to result list cities
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += (object sender, EventArgs e) =>
                {
                    searchResults_ItemSelected(city);
                };
                grid.GestureRecognizers.Add(tap);
                //add the active city in the first position
                if (city.Active) { 
                    ListCities.Children.Insert(0, grid);
                }
                else
                {
                    ListCities.Children.Add(grid);
                }
            }
        }
        void RemoveCity(City city)
        {
           bool removed = FileDB.DeleteCityFile(city);
            if (removed)
            {
                listCity = FileDB.OpenFile(); ;
                FillListView(listCity);
            }
        }
        protected override void OnDisappearing()
        {
            searchBar.Text = "";
            base.OnDisappearing();
        }
        async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        private async void searchResults_ItemSelected(City city)
        {
            lat = city.Lat;
            lon = city.Lon;
            city.Active = true;
            FileDB.WriteFile(city, false);
            await Navigation.PopModalAsync();
        }
        private async void OnSearchBarButtonPressed(object sender, EventArgs args)
        {
            
            // Get the search text.
            SearchBar searchBar = (SearchBar)sender;
            string searchText = searchBar.Text;
            listSearchCity.Clear();
            if (searchBar.Text.Length == 0)
            {
                FillListView(listCity);
                return;
            }
            
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"{Constants.OpenStreetMapEndpoint}city={searchText}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string cities = await response.Content.ReadAsStringAsync();
                dynamic citiesJson = JsonConvert.DeserializeObject(cities);
                foreach (var cityJson in citiesJson)
                {

                    if (cityJson.address.city != null)
                    {
                        bool exist = false;
                        foreach (var item in listSearchCity)
                        {
                            string cJson = $"{cityJson.address.city} - {cityJson.address.state} - {cityJson.address.country}";
                            string cList = $"{item.CityName} - {item.Country}";
                            if (cJson == cList)
                            {
                                exist = true;
                                break;
                            }
                        }
                        if (!exist)
                        {
                            listSearchCity.Add(new City() { Lat = cityJson.lat, Lon = cityJson.lon, CityName = cityJson.address.city, Country = $"{cityJson.address.state} - {cityJson.address.country}" });
                        }
                    }

                }

            }
            FillListView(listSearchCity);
        }
    }
}
