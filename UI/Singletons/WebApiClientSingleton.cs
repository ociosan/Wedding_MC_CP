using Wedding.Api;

namespace UI.Singletons
{
    public sealed class WebApiClientSingleton
    {
        private static FamilyClient? _instance;
        private static readonly object padlock = new object();
        private static string _baseApiUrl = "https://localhost:44310/";

        public WebApiClientSingleton()
        { }

        public static FamilyClient GetInstance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                        _instance = new FamilyClient(_baseApiUrl, new HttpClient() { BaseAddress = new Uri(_baseApiUrl) });

                    return _instance;
                }
            }
        }

    }
}
