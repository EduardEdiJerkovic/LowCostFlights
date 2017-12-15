using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LowCostFlights.Controllers
{
    [Route("api/[controller]")]
    public class CheapFlightsController : Controller //ApiController
    {
        public string flightSearchUrl = "http://api.sandbox.amadeus.com/v1.2/flights/low-fare-search";
        private string apiKey = "xJkGTa34Al5WCaV73GA0Rx2jCz6i3YPy";
        // GET api/values
        [HttpGet]
        public string Get(FlightSearchRequest flight) //[FromUri]
        {
            //?origin=IST&destination=BOS&departure_date=2015-10-15&return_by=2015-10-21T20:00&adults=2&children=3&infants=1&direct=true&include_airlines=TK&apikey=<your API key>
            
            var url = string.Format(flightSearchUrl + "?apikey={0}&origin={1}&destination={2}&departure_date={3:yyyy-MM-dd}", apiKey, flight.org, flight.dest, flight.departureDate);
            if(flight.currency.

            var result = QueryRemoteApi(url).Result;
            
            return result;
        }

        private async Task<string> QueryRemoteApi(string url)
        {
            var client = new HttpClient();
            Console.WriteLine(url);
            var response = await client.GetStringAsync(url);

            return response;
        }
    }


    public class FlightSearchRequest 
    {
        public string org { get; set; }
        public string dest { get; set; }
        public DateTime departureDate { get; set; }    
        public string arrivalDate { get; set; }
        public int passengers { get; set; }
        public string currency { get; set; }
    }
}
