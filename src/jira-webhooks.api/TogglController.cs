using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace jira_webhooks.api
{
    public class TogglController : ApiController
    {
        // POST api/toggl
        public async Task<string> Post([FromBody]JiraMessage jiraMessage)
        {
            var jiraUsername = ConfigurationManager.AppSettings["JiraUsername"];
            var jiraPassword = ConfigurationManager.AppSettings["JiraPassword"];

            var togglApiToken = ConfigurationManager.AppSettings["TogglAPIToken"];

            var response = "";

            if (jiraMessage.User.Name != jiraUsername)
                return "";

            var clientToggl = new HttpClient
            {
                BaseAddress = new Uri("https://www.toggl.com")
            };


            clientToggl.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Base64Encode($"{togglApiToken}:api_token"));

            var startDate = DateTime.UtcNow.AddDays(-1).ToString("o");
            var endDate = DateTime.UtcNow.ToString("o");

            var responseToggl = await clientToggl.GetAsync($"api/v8/time_entries?start_date={startDate}&end_date={endDate}");

            var entries = responseToggl.Content.ReadAsAsync<IList<TogglEntry>>().Result;

            foreach (var entry in entries)
            {
                if (entry.description != jiraMessage.Issue.Key)
                    continue;

                var clientJira = new HttpClient
                {
                    BaseAddress = new Uri("http://jira.princiweb.net.br:7070")
                };

                clientJira.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Base64Encode($"{jiraUsername}:${jiraPassword}"));

                var started = DateTime.Now.AddSeconds(-entry.duration);

                var worklog = new Worklog
                {
                    timeSpentSeconds = entry.duration,
                    started = FormatJIRADate(started),
                    comment = "Teste"
                };

                var responseJira = await clientJira.PostAsJsonAsync($"rest/api/2/issue/{jiraMessage.Issue.Key}/worklog", worklog);

                response = responseJira.ToString();
            }

            return response;
        }

        public class TogglEntry
        {
            public int duration { get; set; }
            public string description { get; set; }
        }

        public class Worklog
        {
            public string timeSpent { get; set; }
            public int timeSpentSeconds { get; set; }
            public string started { get; set; }
            public string comment { get; set; }
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string FormatJIRADate(DateTime date)
        {
            return date.ToString("o");
        }
    }
}