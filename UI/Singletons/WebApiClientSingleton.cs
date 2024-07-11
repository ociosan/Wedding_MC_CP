using Wedding.Api;

namespace UI.Singletons
{
    public sealed class WebApiClientSingleton
    {
        private static FamilyClient? _instance;
        private static readonly object padlock = new object();
        private static string _baseApiUrl = "https://fns-family-check.azurewebsites.net/";

        public WebApiClientSingleton()
        { }

        public static FamilyClient GetInstance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                        _instance = new FamilyClient(new HttpClient() { BaseAddress = new Uri(_baseApiUrl) });

                    return _instance;
                }
            }
        }

    }
}
