using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vuzmir.UnityWebRequestInterface
{
    public class WebResponse
    {
        public bool IsSuccess { get; }
        public long StatusCode { get; }
        public byte[] Payload { get; }
        public string Error { get; }
        public bool IsNetworkAvailable { get; }

        public string StringPayload
        {
            get => Payload == null ? null : WebRequest.Encoding.GetString(Payload);
        }

        public WebResponse(long statusCode, byte[] payload, string error, bool isNetworkAvailable)
        {
            StatusCode = statusCode;
            Payload = payload;
            IsSuccess = statusCode >= 200 && statusCode < 300;
            Error = error;
            IsNetworkAvailable = isNetworkAvailable;
        }

        public T ToJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(
                StringPayload,
                WebRequest.JsonSerializerSettings
            );
        }

        public Task<T> ToJsonAsync<T>()
        {
            return Task.Run(() => ToJson<T>());
        }
    }
    public class WebResponse<T> : WebResponse
    {
        public T Data { get; }
        public WebResponse(
            long statusCode,
            byte[] payload,
            string error,
            bool isNetworkAvailable,
            T data
        )
            : base(statusCode, payload, error, isNetworkAvailable)
        {

            Data = data;
        }
    }
}
