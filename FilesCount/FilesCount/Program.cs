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
            string filesRepoPath = "";
            string reportsPath = "";
            bool isReportsRequired = false;
            bool isStamp = false;

            // Check params
            ArgsChecker checker = new ArgsChecker();
            ArgsErrors rc = checker.CheckAndInit(args, ref filesRepoPath, ref reportsPath, ref isReportsRequired, ref isStamp);

            // I/O Manager
            string fileNamePrefix = Path.GetFileName(filesRepoPath);
            Output output = new Output(isReportsRequired, isStamp, reportsPath, fileNamePrefix);

            if (rc == ArgsErrors.NoError)
            {

                // Count results
                IDictionary<string, int> results = new Dictionary<string, int>();

                int dirCount = 0;
                long fileCount = 0;

                output.SaveColumnsHeader();

                foreach (var packdir in Directory.EnumerateDirectories(filesRepoPath))
                {
                    // Package count results
                    IDictionary<string, int> packageResults = new Dictionary<string, int>();
                    dirCount++;
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
                        // Display and optionnaly store directory counters to files
                        output.SaveCounters(packageResults, Path.GetFileName(packdir), packageFileCount);

                        packageFileCount = 0;
                    }
                    catch (Exception e)
                    {
                        output.DisplayError(ENUMERATEFILES_ERROR, " " + e.Message);
                    }
                }

                // Display and optionnaly store global counters to files
                output.SaveCounters(results, "", fileCount);
                if (isReportsRequired) output.DisplayResultFilePath();
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
