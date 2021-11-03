using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;
using Weather.Models;

namespace Weather.Services
{
    public class FileService
    {
        String folderName = "MyFolder";
        String name = "cityDB.json";
        String contentToSave = "This is some random stuff to save.";
        String loadedContent = "";
        //IFolder folder = FileSystem.Current.LocalStorage;

        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cityDB.json");

        public bool CreateFile() {
            try
            {
                File.Create(fileName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating {name}", ex);
                return false;
            }
            
        }
        public bool FileExists()
        {
            if (File.Exists(fileName))
            {
                Console.WriteLine("existe");
                return true;
            }
            else
            {
                Console.WriteLine("No existe");
                return false;
            }
        }
        public bool WriteFile(City newCity, bool init)
        {
            //Open file to add new city
            if (init)
            {

                List<City> listCity = new List<City>();
                listCity.Add(newCity);
                //write file
                File.WriteAllText(fileName, JsonConvert.SerializeObject(listCity));

            }
            else
            {
                List<City> citiesJson = OpenFile();
               
                bool cityExist = false;
                foreach (City oldCity in citiesJson)
                {
                    oldCity.Active = false;
                    if (oldCity.CityName == newCity.CityName && oldCity.Country == newCity.Country)
                    {
                        oldCity.Active = true;
                        cityExist = true;
                    }
                }
                if (!cityExist)
                {
                    citiesJson.Add(newCity);
                }
                //write file
                File.WriteAllText(fileName, JsonConvert.SerializeObject(citiesJson));
            }
            
            return true;
        }
        public List<City> OpenFile()
        {
            string json = File.ReadAllText(fileName);
            Console.WriteLine($"json  {json}");
            List<City> citiesJson = JsonConvert.DeserializeObject<List<City>>(json);

            return citiesJson;
        }
    }
}
