using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using System.Linq;
using System.Collections.Generic;

namespace AwairCsv
{
    internal class Program
    {
        private static AwairService api;

        private static string bearer;
        private static HttpClient client;

        private static QuickType.Devices devices;

        private static async Task DoWork(DateTime endtime)
        {
            await Init();
            var from = endtime.AddHours(-1.0);

            Console.WriteLine($"Getting raw data from {from.ToLocalTime()} to {endtime.ToLocalTime()}");
            Console.WriteLine($"UTC times: {from} to {endtime}");

            var calls = devices.DevicesDevices.Select(x => api.GetDeviceRawAirDataAsync(x, from, endtime, true));
            var raws = (await Task.WhenAll(calls)).ToList();
            foreach (var r in raws)
            {
                r.CreateDeviceCsv(Environment.CurrentDirectory);
            }

            var multidata = new MultiDeviceRawData(raws);
            multidata.CreateCsv(Environment.CurrentDirectory);
        }

        private static string GetToken()
        {
            var bearer = Environment.GetEnvironmentVariable("AwairBearerToken", EnvironmentVariableTarget.User);
            bearer = string.IsNullOrEmpty(bearer) ? Environment.GetEnvironmentVariable("AwairBearerToken", EnvironmentVariableTarget.Machine) : bearer;

            if (string.IsNullOrEmpty(bearer))
            {
                Console.WriteLine("Missing environment variable: Set a user or system environment variable named AwairBearerToken with the bearer token from https://developer.getawair.com/console/access-token");
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine($"Using bearer token: {bearer}");
            }

            return bearer;
        }

        private static async Task Init()
        {
            bearer = GetToken();
            client = new HttpClient();
            api = new AwairService(client, bearer);
            devices = await api.GetDevicesAsync();
        }

        /// <summary>
        /// Uses the Awair API to output sets of data from your devices.  This uses the raw data which has 10 second intervals and only one hour can be pulled per API call.
        /// <br/>1. Raw API response per device
        /// <br/>2. CSV of sensor readings per device
        /// <br/>3. CSV of sensor readings (all devices) per sensor type.  Useful for graphing as a scatter plot in Excel.
        /// </summary>
        /// <param name="endtime">End of one hour time span to get data. Any string that can be parsed into a DateTime.  One example 2020-02-20T05:00:00.000Z</param>
        /// <returns></returns>
        private static async Task Main(string endtime = "")
        {
            DateTime to;
            if (string.IsNullOrWhiteSpace(endtime))
            {
                to = DateTime.UtcNow;
            }
            else
            {
                to = DateTime.Parse(endtime).ToUniversalTime();
            }

            await Init();
            var from = to.AddHours(-1.0);

            Console.WriteLine($"Getting raw data from {from.ToLocalTime()} to {to.ToLocalTime()}");
            Console.WriteLine($"UTC times: {from} to {endtime}");

            var calls = devices.DevicesDevices.Select(x => api.GetDeviceRawAirDataAsync(x, from, to, true));
            var raws = (await Task.WhenAll(calls)).ToList();
            foreach (var r in raws)
            {
                r.CreateDeviceCsv(Environment.CurrentDirectory);
            }

            var multidata = new MultiDeviceRawData(raws);
            multidata.CreateCsv(Environment.CurrentDirectory);
        }
    }
}