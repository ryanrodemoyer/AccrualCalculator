using System.Collections.Generic;
using AppName.Web.Models;

namespace AppName.Web.Repositories
{
    public interface IAirportRepository
    {
        List<Airport> GetAirports();

        Airport GetAirport(string code);

        void AddAirport(AirportInput airport);
    }
}