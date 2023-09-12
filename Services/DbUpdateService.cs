using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MapLogger
{
    public class DbUpdateService
    {
        private readonly LogDbContext _context;

        public DbUpdateService(LogDbContext context)
        {
            _context = context;
        }

        public bool SaveLogtoDb(string message)
        {
            var parts = message.Split(',');
            if (parts.Length == 4)
            {
                var logEntry = new LogEntryModel
                {
                    Timestamp = DateTime.Parse(parts[0]),
                    Longitude = double.Parse(parts[1]),
                    Latitude = double.Parse(parts[2]),
                    Maptype = parts[3]
                };

                _context.LogEntries.Add(logEntry);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}