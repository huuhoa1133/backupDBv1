using backupDBv1.Repository;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace backupDBv1
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Backup DB
            
            try
            {
                
                var host = ConfigurationSettings.AppSettings["host"];
                var port = ConfigurationSettings.AppSettings["port"];
                var arrport = port.Split(',');
                var cmd = new CommandBackup();

                foreach (var item in arrport)
                {
                    Console.WriteLine($"Backup collection host:{host} - port: {item} is start");
                    cmd.RunCommand(host, item);
                }
                Console.WriteLine("backup mongodb done");
                
                //delete folder backup after x day
                var dayDeleteFolder = int.Parse(ConfigurationSettings.AppSettings["deleteBackupAfterDay"]);
                var date = DateTime.Now;
                date = date.AddDays(-dayDeleteFolder);
                var folderPath = $"{ConfigurationSettings.AppSettings["AddressOut"]}/{date.Year}{date.Month}{date.Day}";
                //delete collection location
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);
                Console.WriteLine($"delete folder {folderPath} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"backup mongodb exception : {ex.Message}");
            }
            
            #endregion

            #region Update status contract agency admin
            
            try
            {
                string ConnectionStringAgencyAdmin = ConfigurationSettings.AppSettings["ConnectStringAgencyAdmin"];
                var connectDbAgencyAdmin = new ConnectDB();
                var _dbAgencyAdmin = connectDbAgencyAdmin.GetDB(ConnectionStringAgencyAdmin);
                var contract = new UpdateStatusAgencyAdmin(_dbAgencyAdmin);
                var result = contract.UpdateStatus();
                Console.WriteLine($"Update status done : match : {result.MatchedCount} - Modified : {result.ModifiedCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update status contract agency admin exception : {ex.Message}");
            }
            
            #endregion

            #region ContractRemind
            
            string ConnectionStringVimeAdmin = ConfigurationSettings.AppSettings["ConnectStringVimeAdmin"];
            var connectDbVimeAdmin = new ConnectDB();
            var _dbVimeAdmin = connectDbVimeAdmin.GetDB(ConnectionStringVimeAdmin);
            try
            {
                var schedule = new ScheduleRepository(_dbVimeAdmin);
                Console.WriteLine("mail remind");
                var task = schedule.RemindContract();
                Task.WaitAll(task);
                Console.WriteLine("ContractRemind done");
            }
            catch(Exception ex)
            {
                new RunCommandRepository(_dbVimeAdmin).WriteLogAsync(ex.Message, "ReadTemplate");
            }

            #endregion

            #region Update status request
            try
            {
                string ConnectionStringVime = ConfigurationSettings.AppSettings["ConnectStringVime"];
                var connectDbVime = new ConnectDB();
                var _dbVime = connectDbVime.GetDB(ConnectionStringVime);
                var contract = new UpdateStatusRequestVime(_dbVime);
                var result = contract.UpdateStatus();
                Console.WriteLine($"Update status done : match : {result.MatchedCount} - Modified : {result.ModifiedCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update status contract agency admin exception : {ex.Message}");
            }
            #endregion

            //Console.ReadLine();
        }
    }
}
