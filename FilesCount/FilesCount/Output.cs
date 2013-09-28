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
        const string TOTALSTRING = "___TOTAL___";
        const string ALLTYPESSTRING = "___FILESCOUNT___";

        public bool IsReportRequired {get; private set;}
        public bool IsStamp {get; private set;}
        public string ReportPath { get; private set; }
        public string CsvFilePath {get; private set; }

        public Output(bool isReportRequired, bool isStamp, string reportPath, string fileNamePrefix)
        {
            IsReportRequired = isReportRequired;
            IsStamp = isStamp;
            ReportPath = reportPath;
            CsvFilePath = Path.Combine(ReportPath, fileNamePrefix + COUNTERFILE_EXT + CSV_EXT);
            if (isStamp) CsvFilePath = Path.Combine(ReportPath, fileNamePrefix + "-" + DateTime.Today.ToString("yyyyMMdd") + COUNTERFILE_EXT + CSV_EXT);
            if (File.Exists(CsvFilePath)) File.Delete(CsvFilePath);
        }


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
            WriteLine("FilesCount.exe DirPath [CountReportsPath [STAMP]]");

            WriteLine("\nSAMPLES");
            WriteLine("FilesCount.exe c:\\repo\\allfiles");
            WriteLine("FilesCount.exe c:\\repo\\allfiles c:\\repo\\reports");
            WriteLine("FilesCount.exe c:\\repo\\allfiles c:\\repo\\reports STAMP");

            WriteLine("\nUSAGE");
            WriteLine("FilesCount counts recursively each file's type in the DirPath folder.");
            WriteLine("It ouputs the result to screen.");
            WriteLine("If CountReportsPath is present, it generates a .csv report with the counters.");
            WriteLine("If STAMP option is present, it set a timestamp in the report file name.");
        }

        public void DisplayResultFilePath()
        {
            WriteLine("Results in " + CsvFilePath);
        }

        // Delete previous filecount files
        public void DeleteFilesCounterFiles()
        {
            // Delete any previous FilesCount file
            string cfpattern = "*" + COUNTERFILE_EXT;
            if (ReportPath.Length > 3)
            {
                WriteLine(String.Format("Deleting {0} file(s) in {1}...", cfpattern, ReportPath));
                foreach (FileInfo f in new DirectoryInfo(ReportPath).GetFiles(cfpattern))
                {
                    f.Delete();
                }
            }
        }

        // Display and optionnaly save column's header titles
        public void SaveColumnsHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DIRNAME");
            sb.Append(CSV_SEP);
            sb.Append("FILE TYPE");
            sb.Append(CSV_SEP);
            sb.Append("FILE COUNT");
            if (!IsReportRequired) Console.WriteLine(sb.ToString());
            if (IsReportRequired)
            {
                // A unique csv file as report
                StreamWriter sw = new StreamWriter(CsvFilePath, true);
                sw.WriteLine(sb.ToString());
                sw.Close();
            }
        }

        // Display and optionnaly save global results or package results to file(s)
        public void SaveCounters(IDictionary<string, int> counters, string optionalPackageName, long total)
        {
            StringBuilder sb = new StringBuilder();
            string packageName = optionalPackageName;
            if (String.IsNullOrEmpty(optionalPackageName)) packageName = TOTALSTRING;
            foreach (var keyValue in counters)
            {
                sb.Append(packageName);
                sb.Append(CSV_SEP);
                sb.Append(keyValue.Key);
                sb.Append(CSV_SEP);
                sb.Append(keyValue.Value);
                sb.Append(Environment.NewLine);
            }
            sb.Append(packageName + CSV_SEP + ALLTYPESSTRING + CSV_SEP + total);

            if (!IsReportRequired) Console.WriteLine(sb.ToString());
            if (IsReportRequired)
            {
                // A unique csv file as report
                StreamWriter sw = new StreamWriter(CsvFilePath, true);
                sw.WriteLine(sb.ToString());
                sw.Close();
                Console.WriteLine(packageName);
            }
        }

    }
}
