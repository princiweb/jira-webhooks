namespace jira_webhooks.api
{
    public class JiraMessage
    {
        public Issue Issue { get; set; }
        public User User { get; set; }
    }

    public class Issue
    {
        public Field Fields { get; set; }
    }

    public class Field
    {
        public string Summary { get; set; }
        public string Description { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
    }
}