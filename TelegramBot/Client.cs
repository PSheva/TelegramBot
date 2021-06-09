//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Web_Api.Models;

//namespace TelegramBot.Clients
//{
//    class Client
//    {
//        private HttpClient _client;
//        private static string _adrdess;
//        public CovidClient()
//        {
//            _adress = Constants.address;

//            _client = new HttpClient();
//            _client.BaseAddress = new Uri(_adress);
//        }

//        //public async Task<ContinentStatistic> GetStatisticByContinent(string continent)
//        //{
//        //    var responce = await _client.GetAsync($"/covidstatistic/continent?continent={continent}");
//        //    responce.EnsureSuccessStatusCode();
//        //    var content = responce.Content.ReadAsStringAsync().Result;
//        //    var result = JsonConvert.DeserializeObject<ContinentStatistic>(content);
//        //    return result;
//        //}

//        public async Task<CurrencyValue> GetValueByName(string id)
//        {
//            var response = await _client.GetAsync($"/api/v3/coins/{id}");
//            response.EnsureSuccessStatusCode();

//            var content = response.Content.ReadAsStringAsync().Result;

//            var result = JsonConvert.DeserializeObject<CurrencyValue>(content);
//            return result;
//        }

//    }
//}



