using MongoDB.Driver;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace backupDBv1.Repository
{
    public class ScheduleRepository
    {
        private IMongoDatabase _db;

        public ScheduleRepository(IMongoDatabase db)
        {
            _db = db;
        }

        public string ReadTemplate()
        {
            try
            {
                string line = string.Empty;
                string path = $"{ConfigurationSettings.AppSettings["pathTemplate"]}\\{ConfigurationSettings.AppSettings["template"]}";
                FileStream fileStream = new FileStream(path, FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    line = reader.ReadToEnd();
                }
                return line;
            }
            catch (Exception ex)
            {
                new RunCommandRepository(_db).WriteLogAsync(ex.Message, "ReadTemplate");
                return string.Empty;
            }
        }

        public async Task RemindContract()
        {
            try
            {
                var runCommandRepository = new RunCommandRepository(_db);
                var contract = await runCommandRepository.GetContractRemind();
                if (contract.status == false)
                {
                    return;
                }

                string content = ReadTemplate();
                if (string.IsNullOrEmpty(content) == true)
                {
                    return;
                }

                var fromAddress = ConfigurationSettings.AppSettings["email"];
                var subject = ConfigurationSettings.AppSettings["subject"];


                StringBuilder body = new StringBuilder();
                body.Append(content);
                body.Replace("{{SENDDATE}}", DateTime.Now.ToLocalTime().ToString("dd/MM/yyyy"));

                for (int i = 0; i < contract.output.Length; i++)
                {
                    var body_content = new StringBuilder();
                    body_content.Append(body);
                    body_content.Replace("{{VOCATIVE}}", contract.output[i].vocative);
                    body_content.Replace("{{FULLNAME}}", contract.output[i].customer_name);
                    body_content.Replace("{{DAYLEFT}}", contract.output[i].preview_notification_date_number.ToString());
                    body_content.Replace("{{PHONE}}", contract.output[i].phone_number);
                    body_content.Replace("{{DOMAIN}}", $"{contract.output[i].domain}/{contract.output[i].sub_domain}");
                    body_content.Replace("{{EFFECTIVEDATE}}", contract.output[i].start.ToLocalTime().ToString("dd/MM/yyyy"));
                    body_content.Replace("{{EXPIREDATE}}", contract.output[i].end.ToLocalTime().ToString("dd/MM/yyyy"));

                    FluentMail.CreateEmailFrom(fromAddress)
                            .Subject(subject)
                            .To(contract.output[i].email)
                            .Body(body_content.ToString())
                            .SendAsync().Wait();
                }
                runCommandRepository.HistoryAsync(contract.output);
            }
            catch (Exception ex)
            {
                new RunCommandRepository(_db).WriteLogAsync(ex.Message, "GetContractRemind");
                return;
            }

        }
    }
}
