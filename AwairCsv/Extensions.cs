using CsvHelper;
using QuickType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using AwairApi;

namespace AwairCsv
{
    public static class Extensions
    {
        public static void CreateCsv(this MultiDeviceRawData raw, string folder)
        {
            foreach (QuickType.Comp sensorType in Enum.GetValues(typeof(QuickType.Comp)))
            {
                CreateCsv(raw, folder, sensorType);
            }
        }

        public static void CreateCsv(this MultiDeviceRawData raw, string folder, QuickType.Comp sensorType)
        {
            var file = Path.Combine(folder, $"Multi-{sensorType}.csv");

            Console.WriteLine($"Writing {sensorType} data from all devices to {file}");

            using var writer = new StreamWriter(file);
            using var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);

            var names = raw.RawAirData.Select(x => x.Device.Name).ToList();
            names.Sort();
            WriteHeader(csv, names);

            var data = raw.RawAirData.SelectMany(x => x.FlatData);

            // Write the rows
            foreach (var item in data)
            {
                csv.WriteField(item.Timestamp);
                foreach (var name in names)
                {
                    if (item.Device.Name == name)
                    {
                        var value = sensorType switch
                        {
                            QuickType.Comp.Co2 => item.Co2,
                            QuickType.Comp.Humid => item.Humid,
                            QuickType.Comp.Lux => item.Lux,
                            QuickType.Comp.Pm25 => item.Pm25,
                            QuickType.Comp.SplA => item.SplA,
                            QuickType.Comp.Temp => item.Temp,
                            QuickType.Comp.Voc => item.Voc,
                            _ => item.Score,
                        };

                        csv.WriteField(value);
                    }
                    else
                    {
                        csv.WriteField(string.Empty);
                    }
                }

                csv.NextRecord();
            }
        }

        public static void CreateDeviceCsv(this RawAirData raw, string folder)
        {
            var file = Path.Combine(folder, raw.Device.Name + ".csv");
            Console.WriteLine($"Writing CSV data from device ({raw.Device.DeviceUuid}) to {file}");
            using (var writer = new StreamWriter(file))
            using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(raw.FlatData);
            }
        }

        private static void WriteHeader(CsvWriter csv, List<string> names)
        {
            var headers = new[] { "Timestamp" }.Concat(names).ToList();

            // Write the headers
            foreach (var header in headers)
            {
                csv.WriteField(header);
            }
            csv.NextRecord();
        }
    }
}