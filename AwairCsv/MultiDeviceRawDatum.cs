using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace AwairCsv
{
    internal class MultiDeviceRawDatum
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="data">Data points from different devices that are already determined to be matched</param>
        public MultiDeviceRawDatum(List<QuickType.Datum> data)
        {
            Data = data;
            DataByDeviceUuid = data.ToDictionary(x => x.Device.DeviceUuid, x => x);
        }

        public List<QuickType.Datum> Data { get; }

        public Dictionary<string, QuickType.Datum> DataByDeviceUuid { get; }
    }
}