using MapLogger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class OlmapLogUpdateService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public OlmapLogUpdateService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Set the timer to start in 1 minute and execute every 5 minutes thereafter
        _timer = new Timer(UpdateDatabase, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void UpdateDatabase(object state)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();

        var latestTimestamp = dbContext.olmapLogEntries.Any() ? 
            dbContext.olmapLogEntries.Max(e => e.Timestamp) : DateTime.MinValue;

        var newLogs = ReadLogs(latestTimestamp);

        dbContext.olmapLogEntries.AddRange(newLogs);
        dbContext.SaveChanges();
    }

    private List<OlmapLogEntry> ReadLogs(DateTime afterTimestamp)
    {
        string logFilePath = "Loggers/olmap.txt";
        var logs = new List<OlmapLogEntry>();
        
        foreach (var line in File.ReadAllLines(logFilePath))
        {
            var parts = line.Split(',');
            
            if (parts.Length != 3) continue; 
            
            if (DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude) &&
                double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude) &&
                timestamp > afterTimestamp) // Only consider entries newer than the provided timestamp
            {
                logs.Add(new OlmapLogEntry
                {
                    Timestamp = timestamp,
                    Longitude = longitude,
                    Latitude = latitude
                });
            }
        }

        return logs;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}