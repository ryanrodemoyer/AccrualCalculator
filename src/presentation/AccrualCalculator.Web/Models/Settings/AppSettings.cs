namespace AppName.Web.Models
{
    public class AppSettings
    {
        public Application Application { get; set; }

        public Messages Messages { get; set; }
    }

    public class Application
    {
        public string AppName { get; set; }

        public string Copyright { get; set; }
    }

    public class Messages
    {
        public string AccessDenied { get; set; }
    }
}
