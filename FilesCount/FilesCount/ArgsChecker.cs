using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilesCount
{
    // Error list
    public enum ArgsErrors { NoError = 0, InvalidArgsCount = 1, UriBadFormat = 2, DirNotFound = 3, badOption = 4 };

    class ArgsChecker
    {
        string DirPath;
        string ReportsPath;
        bool IsReportsRequired;
        bool IsStamp;
        // Command line arguments' check
        public ArgsErrors CheckAndInit(string[] args, ref string dirPath, ref string reportsPath, ref bool isReportsRequired, ref bool isStamp)
        {
            ArgsErrors rc = ArgsErrors.NoError;

            switch (args.Length)
            {
                case 0:
                    rc = ArgsErrors.InvalidArgsCount;
                    break;
                case 1:
                    DirPath = dirPath = args[0];
                    IsReportsRequired = isReportsRequired = false;
                    if (!Directory.Exists(dirPath)) rc = ArgsErrors.DirNotFound;
                    break;
                case 2:
                    DirPath = dirPath = args[0];
                    ReportsPath = reportsPath = args[1];
                    IsReportsRequired = isReportsRequired = true;
                    IsStamp = isStamp = false;
                    if ((!Directory.Exists(reportsPath)) || (!Directory.Exists(dirPath))) rc = ArgsErrors.DirNotFound;
                    break;
                case 3:
                    DirPath = dirPath = args[0];
                    ReportsPath = reportsPath = args[1];
                    IsReportsRequired = isReportsRequired = true;
                    IsStamp = isStamp = false;
                    switch (args[2].ToUpper())
                    {
                        case "STAMP":
                            IsStamp = isStamp = true;
                            break;
                        default:
                            rc = ArgsErrors.badOption;
                            break;
                    }
                    if ((!Directory.Exists(reportsPath)) || (!Directory.Exists(dirPath))) rc = ArgsErrors.DirNotFound;
                    break;
                default:
                    rc = ArgsErrors.InvalidArgsCount;
                    break;
            }
            return rc;
        }

    }
}
