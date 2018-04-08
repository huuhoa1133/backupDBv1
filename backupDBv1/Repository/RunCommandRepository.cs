using backupDBv1.Entity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace backupDBv1.Repository
{
    public class RunCommandRepository
    {
        private IMongoDatabase _db;

        public RunCommandRepository(IMongoDatabase db)
        {
            _db = db;
        }

        public async Task<DataResultDto> GetContractRemind()
        {
            try
            {
                string content = $"{{ eval: \"contract_get_remind()\" }}";
                var result = await RunCommandAsync<DataRunCommand<DataResultDto>>(content);
                return result.retval;
            }
            catch (Exception ex)
            {
                WriteLogAsync(ex.Message, "GetContractRemind");
                return null;
            }
        }

        public Task<DataRunCommand<int>> UpdateRequestStatusVime()
        {
            try
            {
                var updateRequestAfterDay = int.Parse(ConfigurationSettings.AppSettings["UpdateRequestAfterDay"]);
                string content = $"{{ eval: \"system_closed_request({updateRequestAfterDay})\" }}";
                return RunCommandAsync<DataRunCommand<int>>(content);
            }
            catch (Exception ex)
            {
                WriteLogAsync(ex.Message, "system_closed_request");
                return null;
            }
        }

        public async Task<T> RunCommandAsync<T>(string eval)
        {
            try
            {
                var command = new JsonCommand<BsonDocument>(eval);
                var result = await _db.RunCommandAsync(command);
                return BsonSerializer.Deserialize<T>(result);
            }
            catch (Exception ex)
            {
                WriteLogAsync(ex.Message, "RunCommandAsync");
                throw;
            }
        }

        public void WriteLogAsync(string message, string function)
        {
            _db.GetCollection<object>(CollectionNames.log).InsertOne(new { function = function, message = message, date = DateTime.Now });
        }

        public void HistoryAsync(Contract[] contract)
        {
            var now = DateTime.Now;
            var model = new
            {
                date = new
                {
                    Year = now.Year,
                    Month = now.Month,
                    Day = now.Day,
                    Hour = now.Hour,
                    Minute = now.Minute,
                    Second = now.Second
                },
                contract = contract
            };

            //var a = model.GetType().GetProperty("Year").GetValue(model);

            _db.GetCollection<object>(CollectionNames.contract_remind).InsertOne(model);
        }
    }
}
