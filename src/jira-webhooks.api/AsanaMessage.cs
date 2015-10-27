namespace jira_webhooks.api
{
    public class AsanaMessage
    {
        public AsanaFields data { get; set; }
    }

    public class AsanaFields
    {
        public string assignee { get; set; }
        public string notes { get; set; }
        public string name { get; set; }
        public string workspace { get; set; }
    }
}