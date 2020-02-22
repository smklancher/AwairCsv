using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;

namespace AwairCsv
{
    internal class MultiDeviceRawData
    {
        public MultiDeviceRawData(List<QuickType.RawAirData> rawAirDatas)
        {
            RawAirData = rawAirDatas;
        }

        public List<QuickType.RawAirData> RawAirData { get; }

        public void CreateCsv(string folder)
        {
            foreach (QuickType.Comp sensorType in Enum.GetValues(typeof(QuickType.Comp)))
            {
                CreateCsv(folder, sensorType);
            }
        }

        public void CreateCsv(string folder, QuickType.Comp sensorType)
        {
            var file = Path.Combine(folder, $"Multi-{sensorType}.csv");

            Console.WriteLine($"Writing {sensorType} data from all devices to {file}");

            using var writer = new StreamWriter(file);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var names = RawAirData.Select(x => x.Device.Name).ToList();
            names.Sort();
            WriteHeader(csv, names);

            var data = RawAirData.SelectMany(x => x.FlatData);

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

        private void WriteHeader(CsvWriter csv, List<string> names)
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