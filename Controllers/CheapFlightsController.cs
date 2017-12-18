using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LowCostFlights.Controllers
{
    [Route("api/[controller]")]
    public class CheapFlightsController : Controller //ApiController
    {
        public string flightSearchUrl = "https://api.sandbox.amadeus.com/v1.2/flights/low-fare-search";
        private string apiKey = "xJkGTa34Al5WCaV73GA0Rx2jCz6i3YPy";
        // GET api/values
        [HttpGet]
        public List<TableData> Get(FlightSearchRequest flight) //[FromUri]
        {          
            var url = string.Format(flightSearchUrl + "?apikey={0}&origin={1}&destination={2}&departure_date={3:yyyy-MM-dd}&adults={4}&currency={5}", apiKey, flight.org, flight.dest, flight.departureDate, flight.passengers, flight.currency);

            var result = QueryRemoteApi(url).Result;
            var apiResult = JsonConvert.DeserializeObject<ApiResult>(result);

            var tableDataList = ResultToTableData(apiResult);
            
            return tableDataList;
        }

        // Api remote call.
        private async Task<string> QueryRemoteApi(string url)
        {
            var client = new HttpClient();
            Console.WriteLine(url);
            var response = await client.GetStringAsync(url);

            return response;
        }

        // From result make list of important data for table view.
        private List<TableData> ResultToTableData(ApiResult apiResult)
        {
            var listOfData = new List<TableData>();
            foreach(Result result in apiResult.results)
            {
                foreach(Itinerary itinerary in result.itineraries)
                {
                    var data = new TableData();
                    data.price = result.fare.total_price;
                    data.currency = apiResult.currency;
                    // Number of transfers is equal to flights count.
                    data.numberOfTransfers = itinerary.outbound.flights.Count;

                    data.origin = itinerary.outbound.flights[0].origin.airport;
                    data.destination = itinerary.outbound.flights[0].destination.airport;
                    data.departureDate = Convert.ToDateTime(itinerary.outbound.flights[0].departs_at);
                    data.arrivalDate = Convert.ToDateTime(itinerary.outbound.flights[0].arrives_at);
                    foreach(Flight flight in itinerary.outbound.flights)
                    {
                        if(DateTime.Compare(Convert.ToDateTime(flight.departs_at), data.departureDate) < 0)
                        {
                            data.departureDate = Convert.ToDateTime(flight.departs_at);
                            data.origin = flight.origin.airport;
                        }

                        if(DateTime.Compare(Convert.ToDateTime(flight.arrives_at), data.arrivalDate) > 0)
                        {
                            data.arrivalDate = Convert.ToDateTime(flight.arrives_at);
                            data.destination = flight.destination.airport;
                        }
                    }
                    listOfData.Add(data);
                }
            }  
            return(listOfData);             
        }
    }


    public class FlightSearchRequest 
    {
        public string org { get; set; }
        public string dest { get; set; }
        public DateTime departureDate { get; set; }    
        public DateTime arrivalDate { get; set; }
        public int passengers { get; set; }
        public string currency { get; set; }
    }

    public class Origin
    {
        public string airport { get; set; }
        public string terminal { get; set; }
    }

    public class Destination
    {
        public string airport { get; set; }
        public string terminal { get; set; }
    }

    public class BookingInfo
    {
        public string travel_class { get; set; }
        public string booking_code { get; set; }
        public int seats_remaining { get; set; }
    }

    public class Flight
    {
        public string departs_at { get; set; }
        public string arrives_at { get; set; }
        public Origin origin { get; set; }
        public Destination destination { get; set; }
        public string marketing_airline { get; set; }
        public string operating_airline { get; set; }
        public string flight_number { get; set; }
        public string aircraft { get; set; }
        public BookingInfo booking_info { get; set; }
    }

    public class Outbound
    {
        public List<Flight> flights { get; set; }
    }

    public class Itinerary
    {
        public Outbound outbound { get; set; }
    }

    public class PricePerAdult
    {
        public string total_fare { get; set; }
        public string tax { get; set; }
    }

    public class Restrictions
    {
        public bool refundable { get; set; }
        public bool change_penalties { get; set; }
    }

    public class Fare
    {
        public string total_price { get; set; }
        public PricePerAdult price_per_adult { get; set; }
        public Restrictions restrictions { get; set; }
    }

    public class Result
    {
        public List<Itinerary> itineraries { get; set; }
        public Fare fare { get; set; }
    }

    [JsonObject]
    public class ApiResult
    {
        public string currency { get; set; }
        public List<Result> results { get; set; }
    }

    public class TableData
    {
        public string origin  {get; set; }
        public string destination { get; set; }
        public DateTime departureDate { get; set; }    
        public DateTime arrivalDate { get; set; }
        public int numberOfTransfers { get; set; }
        public string currency { get; set; }
        public string price { get; set; }
    }
}
