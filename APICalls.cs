using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows;

// Final Project for CS50
// (C)opyright 2023 Jonathan E. Styles

namespace TimeCheck
{

    // Geolocation API: Nominatim (nominatim.org)
    // ------------------------------------------
    // Search URL: https://nominatim.openstreetmap.org/search?<params>
    // ZIP: postalcode=<zip>
    // Country: country<countryname>
    // JSON: format=json
    //
    // Example: https://nominatim.openstreetmap.org/search?postalcode=24019&country=usa&format=json
    // ------------------------------------------

    // Timezone API: TimeAPI (timeapi.io)
    // ------------------------------------------
    // Timezone: Search URL: https://www.timeapi.io/api/Time/current/coordinate?latitude=<lat>&longitude=<long>
    // ------------------------------------------

    class APICalls
    {

        // Structs to store and exchange data.

        private struct Coords
        {
            public string lat;
            public string lng;
        };

        private struct TZInfo
        {
            public string timeZone;
            public string currTime;
        };

        private Coords geo = new Coords();
        private TZInfo tz = new TZInfo();

        /// <summary>
        /// Controlling Method to retrieve requested data.
        /// </summary>
        /// <param name="zip">Accepts five digit postal zip code.</param>
        public void GetTimeData(string zip)
        {
            geo = GrabCoords(zip);

            if (geo.lat != null)
            {
                tz = GrabTZ(geo.lat, geo.lng);
            }
            else
            {
                MessageBox.Show("This ZIP does not exist.");
            }

        }

        /// <summary>
        /// Used to communicate with the GEOLOCATION API to get Latitude and Longitude of ZIP.
        /// </summary>
        /// <param name="zip">Accepts five digit postal zip code.</param>
        /// <returns>Returns a Coords struct.</returns>
        private Coords GrabCoords(string zip)
        {
            Task<string> responseBody = GetData("https://nominatim.openstreetmap.org/search?postalcode=" + zip + "&country=usa&format=json");
            string[] breakdown = responseBody.Result.Split(',');
            Coords retCoords = new Coords();
            foreach (string line in breakdown)
            {
                if ((line.Contains("lat")) || (line.Contains("lon")))
                {
                    string[] ex = line.Split(':');
                    if (ex[0].Contains("lat")) { retCoords.lat = ex[1]; }
                    if (ex[0].Contains("lon")) { retCoords.lng = ex[1]; }
                }
            }
            return retCoords;
        }

        /// <summary>
        /// Used to communicate with Timezone API to get Time and Timezone of given location.
        /// </summary>
        /// <param name="lat">Accepts latitude coordinates provided as a string.</param>
        /// <param name="lon">Accepts longitude coordinates provided as a string.</param>
        /// <returns>Returns a TZInfo struct.</returns>
        private TZInfo GrabTZ(string lat, string lon)
        {
            Task<string> responseBody = GetData("https://www.timeapi.io/api/Time/current/coordinate?latitude=" + lat.Trim('"') + "&longitude=" + lon.Trim('"'));
            TZInfo retTZ = new TZInfo();
            string[] divvy = responseBody.Result.Split(',');
            foreach (string line in divvy)
            {
                if ((line.Contains("\"time\"")) || (line.Contains("\"timeZone\"")))
                {
                    string[] ex = line.Split(new char[] { ':' }, 2);
                    if (ex[0].Contains("\"time\"")) { retTZ.currTime = ex[1].Trim('"'); }
                    if (ex[0].Contains("\"timeZone\"")) { retTZ.timeZone = ex[1].Trim('"'); }
                }
            }
            return retTZ;
        }

        /// <summary>
        /// Communicates with API vendor through HTTP and saves response as a string object.
        /// </summary>
        /// <param name="url">Prepared URL string given by calling method.</param>
        /// <returns>Returns a JSON reply from targeted API.</returns>
        private async Task<string> GetData(string url)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("User-Agent", "Other");
            HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public string[] GetTZInfo()
        {
            string[] tzinfo = new string[2];
            tzinfo[0] = tz.timeZone;
            tzinfo[1] = tz.currTime;

            return tzinfo;
        }

    }
}
