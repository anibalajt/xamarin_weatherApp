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
        ObservableCollection<City> listSearchCity = new ObservableCollection<City>();
        ObservableCollection<City> listCity = new ObservableCollection<City>();
        //List<City> listCity = new List<City>();
        FileService FileDB = new FileService();

        public string lat { get; set; }
        public string lon { get; set; }
        public SearchCity()
        {
            InitializeComponent();
            //searchResults.ItemsSource = listSearchCity;

        }
        protected override void OnAppearing()
        {
            listCity = new ObservableCollection<City>(FileDB.OpenFile());;
            searchResults.ItemsSource = listCity;
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            //get data from modalView after closed it
            searchBar.Text = "";
            base.OnDisappearing();
        }
        async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        private async void searchResults_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            City selectedNote = searchResults.SelectedItem as City;

            lat = selectedNote.Lat;
            lon = selectedNote.Lon;
            selectedNote.Active = true;
            FileDB.WriteFile(selectedNote,false);
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
                searchResults.ItemsSource = listCity;
                return;
            }
            else
            {
                searchResults.ItemsSource = listSearchCity;
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
        }
    }
}
