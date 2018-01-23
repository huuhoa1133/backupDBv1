using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace backupDBv1
{
    public class CommandBackup
    {
        bool useShellExcute;
        bool redirectStandardInput;
        bool redirectStandardOutput;
        string path_out;
        DateTime date;

        public CommandBackup()
        {
            date = DateTime.Now;
            useShellExcute = false;
            redirectStandardInput = true;
            redirectStandardOutput = true;
            path_out = $"{ConfigurationSettings.AppSettings["AddressOut"]}/{date.Year}{date.Month}{date.Day}/{date.Hour}h{date.Minute}'{date.Second}";
        }

        public void RunCommand(string host, string port)
        {
            try
            {
                string path_out_temp = $"{path_out}/{host}_{port}";
                string dump_collection = $"mongodump --host { host} --port {port} --out {path_out_temp}";
                RunCommand(dump_collection, 1000000);
                string[] fileArray = Directory.GetDirectories(path_out_temp);
                foreach (var item in fileArray)
                {
                    //dump func
                    var dbname = item.Split('\\').Last();
                    string dump_function = $"mongodump --host { host} --port {port} --collection system.js --db { dbname} --out {path_out_temp}";
                    RunCommand(dump_function);

                    //delete collection location
                    if (File.Exists($"{item}\\location.bson"))
                    {
                        File.Delete($"{item}\\location.bson");
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Exception RunCommand : {ex.Message}");
            }
        }

        private void RunCommand(string writeLine, int maxTime = 100000)
        {
            var elapsedTime = 0;
            bool flag = false;
            Process process = new Process();
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.UseShellExecute = useShellExcute;
                processInfo.RedirectStandardInput = redirectStandardInput;
                processInfo.RedirectStandardOutput = redirectStandardOutput;
                processInfo.WorkingDirectory = ConfigurationSettings.AppSettings["MongoDBbin"];
                processInfo.FileName = Environment.ExpandEnvironmentVariables("%SystemRoot%") + @"\System32\cmd.exe";

                process.Exited += new EventHandler((sender,e)=> {
                    Console.WriteLine("**********************");
                    flag = true;
                });
                process.StartInfo = processInfo;
                process.EnableRaisingEvents = true;
                process.Start();

                process.StandardInput.WriteLine(writeLine);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Wait for Exited event, but not more than max time seconds.
            const int SLEEP_AMOUNT = 100;
            while (!flag)
            {
                elapsedTime += SLEEP_AMOUNT;
                if (elapsedTime > maxTime)
                {
                    Console.WriteLine($"Time out");
                    break;
                }
                Thread.Sleep(SLEEP_AMOUNT);
            }
        }
        
    }
}
