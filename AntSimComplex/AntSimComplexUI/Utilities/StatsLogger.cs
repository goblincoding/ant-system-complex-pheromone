using System;
using System.Collections.Generic;
using System.IO;

namespace AntSimComplexUI.Utilities
{
  internal class StatsLogger
  {
    public static StatsLogger Logger => _logger ?? (_logger = new StatsLogger());

    public void Log(IEnumerable<string> logMessages)
    {
      using (var sw = File.AppendText(_path))
      {
        foreach (var logMessage in logMessages)
        {
          sw.WriteLine(logMessage);
        }
      }
    }

    public void Log(string log)
    {
      using (var sw = File.AppendText(_path))
      {
        sw.WriteLine(log);
      }
    }

    private static StatsLogger _logger;
    private readonly string _path;

    private StatsLogger()
    {
      var fileName = $"{DateTime.Today.ToString("yyyy-dd-M")}_stats.log";
      var filePath = Path.GetFullPath(Properties.Settings.Default.LogPath);
      if (!Directory.Exists(filePath))
      {
        Directory.CreateDirectory(filePath);
      }
      _path = Path.Combine(filePath, fileName);
    }
  }
}