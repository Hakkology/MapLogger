using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MapLogger
{
    public class LogDbContext : DbContext
    {
        public IConfiguration Configuration { get; }
        public LogDbContext(IConfiguration configuration, DbContextOptions<LogDbContext> options) 
            : base(options)
        {
            this.Configuration = configuration;
        }

        public DbSet<LogEntry> LogEntries { get; set; }
    }
}