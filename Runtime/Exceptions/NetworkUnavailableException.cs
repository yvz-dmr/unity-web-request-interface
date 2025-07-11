namespace Vuzmir.UnityWebRequestInterface
{
    public class NetworkUnavailableException : WebRequestException
    {
        public NetworkUnavailableException(string url)
            : base(0, url, "Network is unavailable. Please check your internet connection.", null)
        { }
    }
}
