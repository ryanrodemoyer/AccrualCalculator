using System.Collections.Generic;
using System.Linq;
using AppName.Web.Models;

namespace AppName.Web.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private List<Airport> _airports = new List<Airport>
        {
            new Airport("PIT", "Pittsburgh International Airport" ),
            new Airport("DEN", "Denver International Airport" ),
            new Airport("MDW", "Chicago Midway International Airport" ),
            new Airport("FLL", "Fort Lauderdale-Hollywood International Airport" ),
            //new Airport{Code="PIT", Name = "Pittsburgh International Airport" },
            //new Airport{Code="DEN", Name = "Denver International Airport" },
            //new Airport{Code="MDW", Name = "Chicago Midway International Airport" },
            //new Airport{Code="FLL", Name = "Fort Lauderdale-Hollywood International Airport" },
        };

        public List<Airport> GetAirports()
        {
            return _airports;
        }

        public Airport GetAirport(string code)
        {
            return _airports.FirstOrDefault(x => x.Code == code);
        }

        public void AddAirport(AirportInput airport)
        {
            var a = new Airport(airport.Code, airport.Name);
            _airports.Add(a);
        }
    }
}