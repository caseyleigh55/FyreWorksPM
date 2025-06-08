namespace FyreWorksPM.Configuration;

public static class ApiConfig
{
#if ANDROID
    public const string BaseUrl = "https://10.0.2.2:7139/";
#else
    public const string BaseUrl = "https://localhost:7139/";
#endif

}
