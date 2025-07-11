namespace Vuzmir.UnityWebRequestInterface
{
    public class InvalidUrlException : WebRequestException
    {
        public InvalidUrlException(string url)
            : base(0, url, "Invalid or malformed URL. Please verify the format and try again.") { }
    }
}
