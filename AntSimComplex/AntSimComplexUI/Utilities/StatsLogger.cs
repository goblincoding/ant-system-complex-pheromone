using System;
using System.Collections.Generic;
using System.IO;

namespace AntSimComplexUI.Utilities
{
  internal class StatsLogger
  {
    public static StatsLogger Logger => _logger ?? (_logger = new StatsLogger());

    /// <summary>
    /// Logs every message on a separate line.
    /// </summary>
    /// <param name="logMessages"></param>
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