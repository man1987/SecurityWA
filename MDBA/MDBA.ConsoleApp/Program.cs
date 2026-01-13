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

                

                CreateHEWNSWRecords(data.TimeSeries.Where(ts => ts.Header.QualifierId.Contains("hew") && ts.Header.QualifierId.Contains("nsw")).FirstOrDefault());
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

        private static void CreateHEWNSWRecords(TimeSeriesItem hewNSW)
        {
            hewNSW.Events.ForEach(day =>
            {
                var date = day.Date;
                var state = "";
                var qualifier = "";

                
            });
            
             


        }
    }
}
