using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AwairCsv
{
    internal class AwairService
    {
        private const string BaseUrl = "https://developer-apis.awair.is/v1/users/self/";
        private readonly HttpClient client;

        public AwairService(HttpClient client, string bearerToken)
        {
            this.client = client;
            this.client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        }

        public static string FormatIso8601(DateTime dt)
        {
            return dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
        }

        public async Task<QuickType.RawAirData> GetAllAsync()
        {
            var httpResponse = await client.GetAsync(BaseUrl + $"/devices/{{device_type}}/{{device_id}}/air-data/raw?from={{from}}&to={{to}}&limit={{limit}}&desc={{desc}}&fahrenheit={{fahrenheit}}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Cannot retrieve result {httpResponse.StatusCode} - {httpResponse.ReasonPhrase}");
            }

            var content = await httpResponse.Content.ReadAsStringAsync();
            var tasks = QuickType.RawAirData.FromJson(content);

            return tasks;
        }

        public async Task<string> GetAsync(string urlPath)
        {
            var url = BaseUrl + urlPath;
            var httpResponse = await client.GetAsync(url);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception($"{httpResponse.StatusCode} : {url}");
            }

            return await httpResponse.Content.ReadAsStringAsync();
        }

        public async Task<QuickType.RawAirData> GetDeviceRawAirDataAsync(QuickType.DevicesDevice d, DateTime from, DateTime to, bool fahrenheit)
        {
            var fromstr = FormatIso8601(from);
            var tostr = FormatIso8601(to);

            //fromstr = "2020-02-20T04:00:00.000Z";
            //tostr = "2020-02-20T05:00:00.000Z";

            var url = $"devices/{d.DeviceType}/{d.DeviceId}/air-data/raw?from={fromstr}&to={tostr}&limit=360&desc=true&fahrenheit={fahrenheit.ToString().ToLower()}";
            var content = await GetAsync(url);

            var file = Path.Combine(Environment.CurrentDirectory, d.Name + ".json");
            Console.WriteLine($"Writing raw API response to {file}");
            File.WriteAllText(file, content);

            var raw = QuickType.RawAirData.FromJson(content);
            raw.SetDevice(d);

            return raw;
        }

        public async Task<QuickType.Devices> GetDevicesAsync()
        {
            var content = await GetAsync("devices");
            return QuickType.Devices.FromJson(content);
        }
    }
}