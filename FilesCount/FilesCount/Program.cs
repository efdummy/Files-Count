using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FilesCount
{
    class Program
    {
        const int ENUMERATEFILES_ERROR = 99;

        static void Main(string[] args)
        {
            string filesRepoPath="";
            string reportsPath="";
            bool areReportsRequired = false;
            bool isSplitted=false;
            bool isStamp=false;

            // Check params
            ArgsChecker checker = new ArgsChecker();
            ArgsErrors rc = checker.CheckAndInit(args, ref filesRepoPath, ref reportsPath, ref areReportsRequired, ref isSplitted, ref isStamp);

            // I/O Manager
            Output output = new Output();

            if (rc==ArgsErrors.NoError)
            {
                // Echo parameters
                checker.DisplayParams();

                // Count results
                IDictionary<string, int> results = new Dictionary<string, int>();

                int dirCount = 0;
                long fileCount = 0;

                // If split mode, delete previous .FilesCounter files
                if (isSplitted) output.DeleteFilesCounterFiles(reportsPath);

                Stopwatch chrono = new Stopwatch();
                chrono.Start();

                foreach (var packdir in Directory.EnumerateDirectories(filesRepoPath))
                {
                    // Package count results
                    IDictionary<string, int> packageResults = new Dictionary<string, int>();
                    dirCount++;
                    Console.WriteLine(dirCount+" "+Path.GetFileName(packdir));
                    try
                    {
                        long packageFileCount = 0;
                        foreach (var file in Directory.EnumerateFiles(packdir, "*.*", SearchOption.AllDirectories))
                        {
                            packageFileCount++;
                            fileCount++;
                            string ext = Path.GetExtension(file).ToUpper();
                            // Package counters
                            if (packageResults.ContainsKey(ext)) packageResults[ext]++;
                            else packageResults.Add(ext, 1);
                            // Global counters
                            if (results.ContainsKey(ext)) results[ext]++;
                            else results.Add(ext, 1);
                        }
                        string prefix = Path.GetFileName(filesRepoPath);
                        // Save package result
                        output.SaveCounters(packageResults, prefix, Path.GetFileName(packdir), reportsPath, isSplitted, isStamp, packageFileCount);
                        packageFileCount = 0;
                    }
                    catch (Exception e)
                    {
                        output.DisplayError(ENUMERATEFILES_ERROR, " "+e.Message);
                    }
                }

                chrono.Stop(); TimeSpan ts = chrono.Elapsed;

                // Store counters to files
                if (!areReportsRequired)
                {
                    // Display counters to the screen
                    output.DisplayCounters(results, fileCount);
                }
                else
                {
                    string prefix = Path.GetFileName(filesRepoPath);
                    output.SaveCounters(results, prefix, "", reportsPath, isSplitted, isStamp, fileCount);
                    output.DisplayResultFilePath(prefix, reportsPath, isSplitted, isStamp);
                }
                // Display time used to count
                output.WriteLine(String.Format("Counted in : {0:00} h {1:00} m {2:00} s {3:00} ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10));
            }
            else
            {
                // No valid params, display doc
                output.DisplayError(rc);
                output.DisplayDoc();
            }

        }

    }
}
