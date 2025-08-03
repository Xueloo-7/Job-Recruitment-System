namespace Demo;

public static class Extension
{
    public static bool isAjax(this HttpRequest request)
    {
        return request.Headers.XRequestedWith == "XMLHttpRequest";
    }
}
