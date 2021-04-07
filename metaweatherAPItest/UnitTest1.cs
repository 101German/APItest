using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace metaweatherAPItest
{
    public class Tests
    {
        HttpClient httpClient = new HttpClient();
        

        string realLatt = "53.55";
        string realLang = "27.33";

        [SetUp]
        public  void Setup()
        {
            

           
        }

        [Test]
        public async Task SearchMinskDataTest()
        {
            Uri requestUri = new Uri("https://www.metaweather.com/api/location//api/location/search/?query=min");

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            List<CityInfo> cityList;
            try
            {
               
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            CityInfo city;
            if (httpResponseBody.Contains("Minsk"))
            {
                cityList = JsonConvert.DeserializeObject<List<CityInfo>>(httpResponseBody);

                for (int i = 0; i < cityList.Count; i++)
                {
                    if (cityList[i].title == "Minsk")
                        city = cityList[i];
                }
            }

        }
        [Test]
        public async Task CompareLatt_longWithReal()
        {
            Uri requestUri = new Uri("https://www.metaweather.com/api/location/834463");

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            
            try
            {
                
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            CityInfo city = JsonConvert.DeserializeObject<CityInfo>(httpResponseBody);
            string[] latt_long = city.latt_long.Split(',');
            Assert.AreEqual(realLatt, latt_long[0],"Wrong Lattitude");
            Assert.AreEqual(realLang, latt_long[1],"Wrong Langitude");


        }
        [Test]
        public async Task GetActualForecast()
        {
            DateTime today = DateTime.Today;

            Uri requestUri = new Uri($"https://www.metaweather.com/api/location/834463/{today.Year}/{today.Month}/{today.Day}/?query=applicable_date");

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            
            try
            {

                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            


        }
        [Test]
        public async Task SearchForMatchesWeather_name()
        {
            DateTime today = DateTime.Today;
            Uri requestUri = new Uri($"https://www.metaweather.com/api/location/834463/{today.Year}/{today.Month}/{today.Day}/?query=applicable_date");

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            
            try
            {

                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            List<Weather_state_name> ActualWeatherList = JsonConvert.DeserializeObject<List<Weather_state_name>>(httpResponseBody);



            //request for foretime forecast
            requestUri = new Uri($"https://www.metaweather.com/api/location/834463/{today.Year-4}/{today.Month}/{today.Day}/?query=applicable_date");

                        
            httpResponse = new HttpResponseMessage();
           
            try
            {

                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            List<Weather_state_name> ForetimeWeatherList = JsonConvert.DeserializeObject<List<Weather_state_name>>(httpResponseBody);

            bool match = ActualWeatherList.Any(a => ForetimeWeatherList.Any(b => b.weather_state_name == a.weather_state_name));
            Assert.AreEqual(true, match, "No matches found.");
        }

    }
}