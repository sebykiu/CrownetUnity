/*using System;
using System.Collections.Generic;

namespace Logging
{
    public class LoggingController
    {
        private const string Token = "secret-token";
        private const string Bucket = "";
        private const string Organisation = "";
        private const string Url = "http://localhost:8086";
        private InfluxDBClient _client;

        public LoggingController()
        {
            _client = InfluxDBClientFactory.Create(Url, Token);
        }



         public void WriteDataToDatabase(Message message)
         {
             using var writeApi = _client.GetWriteApi();
             var id = PointData.Measurement("crownet").Field("id", message.Id)
                 .Timestamp(DateTime.UtcNow, WritePrecision.Ns);
             var instruction = PointData.Measurement("crownet").Field("instruction", message.Instruction)
                 .Timestamp(DateTime.UtcNow, WritePrecision.Ns);
                
                
                
             writeApi.WritePoints(bucket:Bucket, org: Organisation, points: new List<PointData>
             {
                 id, instruction
             } );
         }
    }
}*/