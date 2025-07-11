using System;

namespace Vuzmir.UnityWebRequestInterface
{
    public class WebRequestException : Exception
    {
        public long StatusCode { get; private set; }
        public string Url { get; private set; }
        public string Error { get; private set; }
        public string Body { get; private set; }

        public WebRequestException(long statusCode, string url, string error, string body = null)
            : base(FormatMessage(statusCode, url, error, body))
        {
            StatusCode = statusCode;
            Url = url;
            Error = error;
            Body = body;
        }

        private static string FormatMessage(long statusCode, string url, string error, string body)
        {
            return $"Web request failed\n"
                + $"Status Code: {statusCode}\n"
                + $"URL: {url}\n"
                + $"Error: {error}"
                + (string.IsNullOrWhiteSpace(body) ? "" : $"\nBody: {body}");
        }
    }
}
