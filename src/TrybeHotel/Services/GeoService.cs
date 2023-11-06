using System.Net.Http;
using Azure;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
         private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
        }

        public async Task<object> GetGeoStatus()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://nominatim.openstreetmap.org/status.php?format=json");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "aspnet-user-agent");
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadFromJsonAsync<object>();
            }
            return null;

        }
        
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "aspnet-user-agent");
            var response = await _client.SendAsync(request);
            var location = await response.Content.ReadFromJsonAsync<GeoDtoResponse[]>();
            return new GeoDtoResponse {
                lat = location[0].lat,
                lon = location[0].lon
            };
        }

        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            var userGps = await GetGeoLocation(geoDto);
            var hotels = repository.GetHotels();

            var hotelsGps = new List<GeoDtoHotelResponse>();

            foreach (var h in hotels)
            {
                var hotelGps = await GetGeoLocation(new GeoDto { Address = h.Address, City = h.CityName, State = h.State });
                var distance = CalculateDistance(userGps.lat, userGps.lon, hotelGps.lat, hotelGps.lon);

                hotelsGps.Add(new GeoDtoHotelResponse
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    Address = h.Address,
                    CityName = h.CityName,
                    State = h.State,
                    Distance = distance
                });
            }

            return hotelsGps.OrderBy(h => h.Distance).ToList();
        }
        
        public int CalculateDistance (string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny) {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.',','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.',','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.',','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.',','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            double distance = R * c;
            return int.Parse(Math.Round(distance,0).ToString());
        }

        public double radiano(double degree) {
            return degree * Math.PI / 180;
        }

    }
}