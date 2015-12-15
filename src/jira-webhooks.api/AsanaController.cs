using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace jira_webhooks.api
{
    public class AsanaController : ApiController
    {
        // POST api/asana
        public async Task<string> Post([FromBody]JiraMessage jiraMessage)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://app.asana.com")
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

            var asanaMessage = new AsanaMessage
            {
                data = new AsanaFields
                {
                    notes = jiraMessage.Issue.Fields.Description,
                    name = jiraMessage.Issue.Fields.Summary,
                    workspace = "",
                    assignee = ""
                }
            };

            var response = await client.PostAsJsonAsync("api/1.0/tasks", asanaMessage);

            return response.ToString();
        }
    }
}