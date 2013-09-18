using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilesCount
{
    class Output
    {
        const string COUNTERFILE_EXT = ".FilesCount";
        const string CSV_EXT = ".csv";
        const string CSV_SEP = ";";

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        // Display error to the screen
        public void DisplayError(int rc, string message)
        {
            WriteLine("*** " + message + " Error");
        }
        public void DisplayError(ArgsErrors rc)
        {
            WriteLine("*** " + rc + " Error");
        }

        // Display doc to the screen
        public void DisplayDoc()
        {
            WriteLine("\nSYNTAX");
            WriteLine("FilesCount.exe DirPath [CountReportsPath [STAMP | SPLIT]]");

            WriteLine("\nSAMPLES");
            WriteLine("FilesCount.exe c:\\repo\\allfiles");
            WriteLine("FilesCount.exe c:\\repo\\allfiles c:\\repo\\reports");
            WriteLine("FilesCount.exe c:\\repo\\allfiles c:\\repo\\reports STAMP");
            WriteLine("FilesCount.exe c:\\repo\\allfiles c:\\repo\\reports SPLIT");

            WriteLine("\nUSAGE");
            WriteLine("FilesCount counts recursively each file's type in the DirPath folder.");
            WriteLine("It ouputs the result to screen.");
            WriteLine("If CountReportsPath is present, it generates a .csv report with the counters.");
            WriteLine("If STAMP option is present, it set a timestamp in the report file name.");
            WriteLine("Use SPLIT option as third arg to generate multiple .extcounter file for each counter.");
        }

        // Output global results to the screen
        public void DisplayCounters(IDictionary<string, int> results, long total)
        {
            foreach (var keyValue in results)
            {
                WriteLine(keyValue.Key + CSV_SEP + keyValue.Value);
            }
            WriteLine("TOTAL NUMBER OF FILES: " + total);
        }

        public string CsvFilePath(string prefix, string repPath, bool isStamp)
        {
            string csvFile = Path.Combine(repPath, prefix + COUNTERFILE_EXT + CSV_EXT);
            if (isStamp) csvFile = Path.Combine(repPath, prefix + "-" + DateTime.Today.ToString("yyyyMMdd") + COUNTERFILE_EXT + CSV_EXT);
            return csvFile;
        }
        public string SplitFilesPathPattern(string repPath)
        {
            return Path.Combine(repPath, "#.EXT" + COUNTERFILE_EXT);
        }
        public void DisplayResultFilePath(string prefix, string repPath, bool isSplitted, bool isStamp)
        {
            if (isSplitted)
                WriteLine("Results in " + SplitFilesPathPattern(repPath));
            else
                WriteLine("Results in " + CsvFilePath(prefix, repPath, isStamp));
        }

        // Delete previous filecount files
        public void DeleteFilesCounterFiles(string reportsPath)
        {
            // Delete any previous FilesCount file
            string cfpattern = "*" + COUNTERFILE_EXT;
            if (reportsPath.Length > 3)
            {
                WriteLine(String.Format("Deleting {0} file(s) in {1}...", cfpattern, reportsPath));
                foreach (FileInfo f in new DirectoryInfo(reportsPath).GetFiles(cfpattern))
                {
                    f.Delete();
                }
            }
        }

        // Save global results or package results to file(s)
        public void SaveCounters(IDictionary<string, int> counters, string fileNamePrefix, string optionalPackageName, string reportsPath, bool isSplitted, bool isStamp, long total)
        {
            StringBuilder sb = new StringBuilder();
            if (!isSplitted)
            {
                // A unique csv file as report
                String csvfilePath = CsvFilePath(fileNamePrefix, reportsPath, isStamp);
                if (!String.IsNullOrEmpty(optionalPackageName)) csvfilePath=CsvFilePath(optionalPackageName, reportsPath, isStamp);
                foreach (var keyValue in counters)
                {
                    sb.Append(keyValue.Key);
                    sb.Append(CSV_SEP);
                    sb.Append(keyValue.Value);
                    sb.Append(Environment.NewLine);
                }
                sb.Append("TOTAL;" + total + Environment.NewLine);
                StreamWriter stream=File.CreateText(csvfilePath);
                stream.Write(sb);
                stream.Close();
            }
            else
            {
                // Multiple report files for each file extension
                String reportfilePath;
                // File name pattern
                string cfpattern = "*" + COUNTERFILE_EXT;
                if (!String.IsNullOrEmpty(optionalPackageName)) cfpattern = "*." + optionalPackageName + COUNTERFILE_EXT;

                // Create a #.ext.FilesCount file for each extension
                WriteLine(String.Format("Create {0} files in {1}...", cfpattern, reportsPath));
                foreach (var keyValue in counters)
                {
                    reportfilePath = Path.Combine(reportsPath, keyValue.Value.ToString() + keyValue.Key + COUNTERFILE_EXT);
                    if (!String.IsNullOrEmpty(optionalPackageName))
                        reportfilePath = Path.Combine(reportsPath, keyValue.Value.ToString() + keyValue.Key + "." + optionalPackageName + COUNTERFILE_EXT);
                    File.CreateText(reportfilePath);
                }
                //reportfilePath = Path.Combine(reportsPath, total+".#TOTAL#" + COUNTERFILE_EXT);
            }

        }

    }
}
