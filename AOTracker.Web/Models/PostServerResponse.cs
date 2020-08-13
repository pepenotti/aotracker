namespace AOTracker.Web.Models
{
    public class PostServerResponse
    {
        public bool IsNameRepeated { get; set; }

        public bool IsWebRepeated { get; set; }

        public bool IsUsersEndpointRepeated { get; set; }

        public bool HasError { get; set; }

        public ServerData ServerData { get; set; }

        public bool WebIsNotValid { get; set; }

        public bool UsersEndpointIsNotValid { get; set; }
    }
}
