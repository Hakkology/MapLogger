using MapLogger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class CesiumLogUpdateService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public CesiumLogUpdateService(IServiceProvider serviceProvider)
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

        var latestTimestamp = dbContext.cesiumLogEntries.Max(e => e.Timestamp);
        var newLogs = ReadLogs().Where(log => log.Timestamp > latestTimestamp);

        dbContext.cesiumLogEntries.AddRange(newLogs);
        dbContext.SaveChanges();
    }


    private List<CesiumLogEntry> ReadLogs()
    {
        string logFilePath = "~/Loggers/cesium.txt"; // Provide the path to your log file.
        var logs = new List<CesiumLogEntry>();
        
        foreach (var line in File.ReadAllLines(logFilePath))
        {
            var parts = line.Split(',');
            
            if (parts.Length != 3) continue; 
            
            if (DateTime.TryParse(parts[0], out var timestamp) &&
                double.TryParse(parts[1], out var longitude) &&
                double.TryParse(parts[2], out var latitude))
            {
                logs.Add(new CesiumLogEntry
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
