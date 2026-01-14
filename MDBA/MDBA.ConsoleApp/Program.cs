using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDBA.ConsoleApp
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            try
            {
                string connectionString = "AuthType=Office365;Url=https://contoso.crm.dynamics.com;Username=admin@contoso.onmicrosoft.com;Password=password123;";
                CrmServiceClient serviceClient = new CrmServiceClient(connectionString);

                var startDate = DateTime.Now.AddMonths(-2).ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string url = "http://mdbaprdrws01.prod.local:8080/FewsWebServices/rest/fewspiservice/v1/timeseries?filterId=Env_Watering_MDBA_Qenv&locationIds=R_401027&startTime=" + startDate + "&endTime=" + endDate + "&documentFormat=PI_JSON&documentVersion=1.34";
                //string url = "http://mdbaprdrws01.prod.local:8080/FewsWebServices/rest/fewspiservice/v1/timeseries?filterId=Env_Watering_MDBA_Qenv&locationIds=R_401027&startTime=2024-12-01T08%3A00%3A00Z&endTime=2024-12-10T08%3A00%3A00Z&documentFormat=PI_JSON&documentVersion=1.34";

                // Send the GET request
                HttpResponseMessage response = await client.GetAsync(url);

                // Ensure we got a 2xx status code
                response.EnsureSuccessStatusCode();

                // Read the content as a string
                string responseBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                FewsPiRoot data = JsonSerializer.Deserialize<FewsPiRoot>(responseBody, options);

                

                CreateHEWNSWRecords(serviceClient,data.TimeSeries.Where(ts => ts.Header.QualifierId.Contains("hew") && ts.Header.QualifierId.Contains("nsw")).FirstOrDefault());
                //CreateHEWVICRecords();
                //CreateRMIFNSWRecords();
                //CreateRMIFVICRecords();
                //CreateEWANSWRecords();
                //CreateEWAVICRecords();

                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        private static void CreateHEWNSWRecords(CrmServiceClient serviceClient, TimeSeriesItem hewNSW)
        {
            hewNSW.Events.ForEach(day =>
            { 
                
                var date = day.Date;
                var state = new OptionSetValue(593620000);
                var qualifier = new OptionSetValue(593620000);

                var query = new QueryExpression("mdba_rowsdata");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition("mdba_date", ConditionOperator.Equal, DateTime.Parse(date));
                query.Criteria.AddCondition("mdba_qualifier", ConditionOperator.Equal, qualifier);
                var existingRecords = serviceClient.RetrieveMultiple(query);

                if(existingRecords.Entities.Count>0 && existingRecords.Entities.First().GetAttributeValue<decimal>("msba_value") != decimal.Parse(day.Value))
                {
                    //Update existing record
                    var recordToUpdate = new Entity(existingRecords.Entities.First().LogicalName, existingRecords.Entities.First().Id);
                    recordToUpdate.Attributes["mdba_value"] = decimal.Parse(day.Value);
                    serviceClient.Update(recordToUpdate);
                }
                else 
                {
                    //Create new record
                    var newRecord = new Entity("mdba_rowsdata");
                    newRecord.Attributes["mdba_date"] = DateTime.Parse(date);
                    newRecord.Attributes["mdba_value"] = decimal.Parse(day.Value);
                    newRecord.Attributes["mdba_qualifier"] = qualifier;
                    newRecord.Attributes["mdba_state"] = state;
                    serviceClient.Create(newRecord);

                }
            });
        }
    }
}
