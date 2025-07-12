using System;
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

        public async Task<WebResponse<T>> GetResponse<T>(WebRequest request)
        {
            var response = await GetResponse(request);
            if (response.IsSuccess)
            {
                try
                {
                    var data = await response.ToJsonAsync<T>();
                    return new WebResponse<T>(response.StatusCode,
                                                  response.Payload,
                                                  null,
                                                  response.IsNetworkAvailable,
                                                  data);
                }
                catch (Exception e)
                {
                    return new WebResponse<T>(0,
                                                  response.Payload,
                                                  e.Message,
                                                  response.IsNetworkAvailable,
                                                  default);
                }
            }

            return new WebResponse<T>(0,
                                      response.Payload,
                                      response.Error,
                                      response.IsNetworkAvailable,
                                      default);
        }


        public async Task<T> GetJsonOrThrow<T>(WebRequest request)
        {
            var response = await GetResponse<T>(request);
            if (response.IsSuccess)
                return response.Data;

            throw GetException(request.Url, response);
        }
        public async Task<string> GetStringOrThrow(WebRequest request)
        {
            var bytes = await GetBytesOrThrow(request);
            if (bytes == null || bytes.Length == 0)
                return null;

            return WebRequest.Encoding.GetString(bytes);
        }

        public async Task<byte[]> GetBytesOrThrow(WebRequest request)
        {
            var response = await GetResponse(request);
            if (response.IsSuccess)
                return response.Payload;
            throw GetException(request.Url, response);
        }

        private WebRequestException GetException(string url, WebResponse response)
        {
            return new WebRequestException(
                    response.StatusCode,
                    url,
                    response.Error,
                    response.StringPayload,
                    response.IsNetworkAvailable
                );
        }
    }
}
