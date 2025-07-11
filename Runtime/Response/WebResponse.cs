using System;
using System.Text;
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

        public string StringPayload
        {
            get => Payload == null ? null : WebRequest.Encoding.GetString(Payload);
        }

        public WebResponse(long statusCode, byte[] payload, string error)
        {
            StatusCode = statusCode;
            Payload = payload;
            IsSuccess = statusCode >= 200 && statusCode < 300;
            Error = error;
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
}
