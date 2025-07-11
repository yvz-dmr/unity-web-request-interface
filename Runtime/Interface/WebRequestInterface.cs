using System.Text;
using System.Threading.Tasks;

namespace Vuzmir.UnityWebRequestInterface
{
    public abstract class WebRequestInterface : IWebRequestInterface
    {
        public abstract Task<WebResponse> GetResponse(WebRequest request);

        public async Task Send(WebRequest request)
        {
            var response = await GetResponse(request);
            if (!response.IsSuccess)
                throw GetException(request.Url, response);
        }

        public async Task<T> GetJsonResponse<T>(WebRequest request)
        {
            var response = await GetResponse(request);
            if (response.IsSuccess)
                return response.ToJson<T>();

            throw GetException(request.Url, response);
        }

        public async Task<string> GetString(WebRequest request)
        {
            var bytes = await GetBytes(request);
            if (bytes == null || bytes.Length == 0)
                return null;

            return WebRequest.Encoding.GetString(bytes);
        }

        public async Task<byte[]> GetBytes(WebRequest request)
        {
            var response = await GetResponse(request);
            if (response.IsSuccess)
                return response.Payload;
            throw GetException(request.Url, response);
        }

        private WebRequestException GetException(string url, WebResponse response)
        {
            return response is NetworkUnavailableResponse
                ? new NetworkUnavailableException(url)
                : new WebRequestException(
                    response.StatusCode,
                    url,
                    response.Error,
                    response.StringPayload
                );
        }
    }
}
