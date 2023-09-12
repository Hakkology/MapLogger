namespace MapLogger
{
    public class LogUpdateService
    {
        private const string LogFileDirectory = "Loggers";
        private const string LogFileName = "maplogger.txt";
        private readonly string _logFilePath;

        public LogUpdateService()
        {
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), LogFileDirectory, LogFileName);
            EnsureLogDirectoryExists();
        }

        private void EnsureLogDirectoryExists()
        {
            string dir = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void LogInput(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"{message}\n");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to write log to file. Error: {ex.Message}");
            }
        }
    }
}
