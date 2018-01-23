using backupDBv1.Repository;
using System;
using System.Configuration;
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

            Console.ReadLine();
        }
    }
}
