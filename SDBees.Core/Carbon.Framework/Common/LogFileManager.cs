using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Carbon.Common
{
	public static class LogFileManager
	{
		/// <summary>
		/// Initializes the logging system for use. Installs a trace listener that will output 
		/// diagnostic information to a log file located in the application directory.
		/// </summary>
		internal static void Initialize()
		{
			// use the default logs folder
			var logsFolder = DefaultLogsFolder;

			// create the filename that will power the current log file listener
			var filename = Path.Combine(logsFolder, DefaultLogName);

			// make sure it exists
			if (!CreateLogsFolder(logsFolder))
			{
				// if it already existed, backup the existing log files
				BackupExistingLogFiles(logsFolder);

				// and then delete any that are too old
				DeleteOldLogFiles(logsFolder);
			}

			// initialize a new log file listener
			var listener = new TextWriterTraceListener(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Read));
			listener.Name = "LogFileTraceListener";
			listener.TraceOutputOptions =
				TraceOptions.Callstack |
				TraceOptions.LogicalOperationStack |
				TraceOptions.DateTime |
				TraceOptions.ProcessId |
				TraceOptions.ThreadId |
				TraceOptions.Timestamp;

			// add it to the Debug Listeners for this application
			Debug.Listeners.Add(listener);
			Debug.AutoFlush = true;

			Debug.WriteLine("################################################################################");
			Debug.WriteLine($"# Log file opened at '{DateTime.Now.ToString()}'.");
			Debug.WriteLine("# OSVersion: '{0}'.", Environment.OSVersion);
			Debug.WriteLine($"# MachineName: '{Environment.MachineName}'.");
			Debug.WriteLine("# ProcessorCount: '{0}'.", Environment.ProcessorCount);
			Debug.WriteLine($"# UserName: '{Environment.UserName}'.");
			Debug.WriteLine("# UserInteractive: '{0}'.", Environment.UserInteractive);
			Debug.WriteLine($"# Plugin Context created by '{Assembly.GetEntryAssembly().Location}'.");			
			Debug.WriteLine("################################################################################\n");						
		}

		/// <summary>
		/// Shuts down the log file.
		/// </summary>
		internal static void Shutdown()
		{
			Debug.WriteLine("\n################################################################################");
			Debug.WriteLine($"# Log file closed at '{DateTime.Now.ToString()}'.");
			Debug.WriteLine("################################################################################");
		}

		/// <summary>
		/// Returns the default folder where the logs are stored.
		/// </summary>
		private static string DefaultLogsFolder
		{
			get
			{
				return Path.Combine(CarbonConfig.LocalUsersDataPath, "Logs");
			}
		}

		/// <summary>
		/// Returns the default date format used to name log files.
		/// </summary>
		private static string DefaultDateFormat
		{
			get
			{
				return "MM-dd-yyyy";
			}
		}

		/// <summary>
		/// Returns the default name of the log file.
		/// </summary>
		private static string DefaultLogName
		{
			get
			{
				return $"{DateTime.Now.ToString(DefaultDateFormat)}.txt";
			}
		}

		/// <summary>
		/// Returns the default name of the backup log file.
		/// </summary>
		private static string DefaultBackupLogName
		{
			get
			{
				return $"{DateTime.Now.ToString(DefaultDateFormat)}-Backup.txt";
			}
		}

		/// <summary>
		/// Creates the logs folder. Returns true if the directory was created, false otherwise.
		/// </summary>
		/// <param name="logsFolder">The directory to create.</param>
		/// <returns></returns>
		private static bool CreateLogsFolder(string logsFolder)
		{
			try
			{
				if (Directory.Exists(logsFolder))
					return false;
				Directory.CreateDirectory(logsFolder);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return true;
		}

		/// <summary>
		/// Backs up the existing log file, as the last run backup.
		/// </summary>
		/// <param name="logsFolder"></param>
		private static void BackupExistingLogFiles(string logsFolder)
		{
			try
			{
				// calc the current filename
				var currentFilename = Path.Combine(logsFolder, DefaultLogName);

				// look for it, if it exists, then back it up
				if (File.Exists(currentFilename))
				{
					// calc the backup name
					var backupFilename = Path.Combine(logsFolder, DefaultBackupLogName);

					// if there was a previous backup, delete it
					if (File.Exists(backupFilename))
						File.Delete(backupFilename);

					// move the current file to the backup location
					File.Move(currentFilename, backupFilename);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Deletes any log files older than a week.
		/// </summary>
		/// <param name="logsFolder"></param>
		private static void DeleteOldLogFiles(string logsFolder)
		{
			try
			{
				// look for all of the log files
				var info = new DirectoryInfo(logsFolder);
				var files = info.GetFiles("*.txt", SearchOption.TopDirectoryOnly);

				// delete any files that are more than a week old
				var now = DateTime.Now;
				var allowedDelta = new TimeSpan(7, 0, 0);

				foreach (var file in files)
				{
					if (now.Subtract(file.LastWriteTime) > allowedDelta)
						file.Delete();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}    
}
