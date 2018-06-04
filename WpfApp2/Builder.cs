using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class Builder
    {
        public static (string, List<string>) Build(string msbuildFileName)
        {
            string assemblyExecutionPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string logFilePath = assemblyExecutionPath.Remove(assemblyExecutionPath.LastIndexOf("\\") + 1) + "build.log";

            FileLogger logger = new FileLogger();
            logger.Parameters = @"logfile=" + logFilePath;
            ProjectCollection pc = new ProjectCollection();
            Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
            GlobalProperty.Add("Configuration", "Debug");
            GlobalProperty.Add("Platform", "Any CPU");
            GlobalProperty.Add("OutputPath", @"bin\\Debug");
            BuildRequestData buildRequest = new BuildRequestData(msbuildFileName, GlobalProperty, null, new string[] { "Build" }, null);
            BuildParameters bp = new BuildParameters(pc);
            bp.DetailedSummary = true;
            bp.Loggers = new List<Microsoft.Build.Framework.ILogger> { logger }.AsEnumerable();
            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, buildRequest);
            pc.UnregisterAllLoggers();

            List<string> errors = new List<string>();
            string[] solutionBuildOutputs = File.ReadAllLines(logFilePath);
            string allOutput = string.Join("\n", solutionBuildOutputs);
            foreach (string s in solutionBuildOutputs)
            {
                if (s.Contains("error"))
                    errors.Add(s);
            }
            File.Delete(logFilePath);

            return (allOutput, errors);
        }
    }
}
