using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AwairCsv;
using CsvHelper;
using Newtonsoft.Json;

namespace QuickType
{
    public partial class Datum
    {
        [JsonIgnore]
        public DevicesDevice Device { get; set; }
    }

    public partial class RawAirData
    {
        [JsonIgnore]
        public DevicesDevice Device { get; set; }

        [JsonIgnore]
        public List<FlatData> FlatData => Data.Select(x => new FlatData(x)).ToList();

        public void CreateDeviceCsv(string folder)
        {
            var file = Path.Combine(folder, Device.Name + ".csv");
            Console.WriteLine($"Writing CSV data from device ({Device.DeviceUuid}) to {file}");
            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(FlatData);
            }
        }

        public void SetDevice(DevicesDevice device)
        {
            Device = device;

            foreach (var d in Data)
            {
                d.Device = Device;
            }
        }
    }
}