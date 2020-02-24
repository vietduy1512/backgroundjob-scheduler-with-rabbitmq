using Infrastructure.Scheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AppWithScheduler.Tasks
{
    // uses https://theysaidso.com/api/
    public class QuoteOfTheDayTask : IScheduledTask
    {
        public string Schedule => "*/2 * * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();

            var quoteJson = JObject.Parse(await httpClient.GetStringAsync("http://quotes.rest/qod.json"));

            QuoteOfTheDay.list.Add(JsonConvert.DeserializeObject<QuoteOfTheDay>(quoteJson["contents"]["quotes"][0].ToString()));
        }
    }
    
    public class QuoteOfTheDay
    {
        public static List<QuoteOfTheDay> list { get; set; }

        static QuoteOfTheDay()
        {
            list = new List<QuoteOfTheDay> { new QuoteOfTheDay { Quote = "No quote", Author = "Maarten" } };
        }
        
        public string Quote { get; set; }
        public string Author { get; set; }
    }
}