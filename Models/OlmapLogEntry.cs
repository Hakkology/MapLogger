using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapLogger
{
    public class OlmapLogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}