namespace KF2Admin.Config
{
    public class PlayerHandlerConfiguration
    {
        public bool WelcomeNewPlayer { get; set; } = true;
        public string OnWelcomeNewPlayer { get; set; } = "Welcome '{0}', Player #{1}";

        public bool WelcomeOldPlayer { get; set; } = true;
        public string OnWelcomeOldPlayer { get; set; } = "Welcome back, {0}, played {1} times on this server.";
    }
}
