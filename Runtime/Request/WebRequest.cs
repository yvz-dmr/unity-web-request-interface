using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vuzmir.UnityWebRequestInterface
{
    public enum WebRequestMethod
    {
        Get,
        Post,
        Put,
        Patch,
        Delete,
    }

    public enum WebRequestPayloadType
    {
        None,
        Form,
        Raw,
        Json,
    }

    public class WebRequest
    {
        public static WebRequestInterface Provider { get; set; } = new UnityWebRequestProvider();
        public static JsonSerializerSettings JsonSerializerSettings = null;
        public static Encoding Encoding = Encoding.UTF8;

        public string Scheme { get; private set; }
        public string Authority { get; private set; }
        public string Path { get; private set; }
        public string Url
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(Scheme);
                sb.Append("://");
                sb.Append(Authority);
                sb.Append(Path);
                if (Queries.Count() > 0)
                {
                    sb.Append("?");
                    foreach (var query in Queries)
                    {
                        sb.Append(query.Key);
                        sb.Append('=');
                        sb.Append(query.Value);
                        sb.Append('&');
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }
        }
        public WebRequestMethod Method { get; private set; }

        public string Authorization
        {
            get => headerDict?["authorization"];
            set => SetHeader("authorization", value);
        }
        public string ContentType
        {
            get => headerDict?["content-type"];
            set => SetHeader("content-type", value);
        }

        private Dictionary<string, object> queryDict = null;
        private Dictionary<string, string> headerDict = null;

        public WebRequestPayloadType PayloadType { get; private set; } = WebRequestPayloadType.None;

        public object PayloadObject { get; private set; }
        public IEnumerable<KeyValuePair<string, object>> Queries
        {
            get
            {
                if (queryDict == null)
                {
                    return Array.Empty<KeyValuePair<string, object>>();
                }
                return queryDict;
            }
        }
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                if (headerDict == null)
                {
                    return Array.Empty<KeyValuePair<string, string>>();
                }
                return headerDict;
            }
        }

        public WebRequest(string endpoint, WebRequestMethod method)
        {
            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
                throw new InvalidDataException($"The value '{endpoint}' is not a valid URI.");

            var query = uri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                if (query[0] == '?')
                {
                    query = query[1..];
                }
                var queries = query.Split('&').Where(q => !string.IsNullOrEmpty(q));
                foreach (var q in queries)
                {
                    var s = q.Split('=');
                    if (s.Length != 2)
                        continue;
                    SetQuery(s.First(), s.Last());
                }
            }

            Scheme = uri.Scheme;
            Authority = uri.Authority;
            Path = uri.AbsolutePath;

            Method = method;
        }

        public static WebRequest Get(string endpoint) => new(endpoint, WebRequestMethod.Get);

        public static WebRequest Post(string endpoint) => new(endpoint, WebRequestMethod.Post);

        public static WebRequest Put(string endpoint) => new(endpoint, WebRequestMethod.Put);

        public static WebRequest Patch(string endpoint) => new(endpoint, WebRequestMethod.Patch);

        public static WebRequest Delete(string endpoint) => new(endpoint, WebRequestMethod.Delete);

        public WebRequest AttachRawPayload(byte[] payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            ContentType = "application/octet-stream";
            PayloadObject = payload;
            PayloadType = WebRequestPayloadType.Raw;
            return this;
        }

        public WebRequest AttachJsonPayload<T>(T payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            ContentType = "application/json";
            PayloadObject = payload;
            PayloadType = WebRequestPayloadType.Json;
            return this;
        }

        public WebRequest AttachFormPayload(WebRequestForm form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            if (form.Fields.Any(field => field.Value is WebFile || field.Value is byte[]))
                ContentType = "multipart/form-data";
            else
                ContentType = "application/x-www-form-urlencoded";

            PayloadObject = form;
            PayloadType = WebRequestPayloadType.Form;
            return this;
        }

        public WebRequest SetHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            headerDict ??= new Dictionary<string, string>();
            name = name.ToLowerInvariant();
            if (value == null)
            {
                headerDict.Remove(name);
                return this;
            }

            headerDict[name] = value;
            return this;
        }

        public WebRequest SetQuery(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (value == null)
            {
                queryDict.Remove(name);
                return this;
            }

            if (value == null || (value is string str && string.IsNullOrEmpty(str)))
            {
                queryDict.Remove(name);
                return this;
            }

            if (!value.GetType().IsPrimitive && value is not string)
                throw new Exception(
                    $"{value.GetType().Name} can't be set as query parameters, only primitive c# data types like string, int, bool vs is supported as query"
                );

            queryDict ??= new Dictionary<string, object>();
            if (queryDict.ContainsKey(name))
                queryDict[name] = value;
            else
                queryDict.Add(name, value);
            return this;
        }

        /// <summary>
        /// Sends the request and returns a <see cref="WebResponse"/>.
        /// </summary>
        /// <returns>A task that resolves to a <see cref="WebResponse"/>.</returns>
        public Task<WebResponse> GetResponse() => Provider.GetResponse(this);

        /// <summary>
        /// Sends the request and returns a <see cref="WebResponse<T>"/> containing the JSON response.
        /// </summary>
        /// <returns>A task that resolves to a <see cref="WebResponse<T>"/>.</returns>
        public Task<WebResponse<T>> GetResponse<T>() => Provider.GetResponse<T>(this);

        /// <summary>
        /// Sends the web request.
        /// </summary>
        /// <exception cref="WebRequestException">Thrown when the request fails or the network is unavailable.</exception>
        public Task Send() => Provider.Send(this);

        /// <summary>
        /// Sends the request and gets raw bytes.
        /// </summary>
        /// <exception cref="WebRequestException">Thrown when the request fails or the network is unavailable.</exception>
        public Task<byte[]> GetBytesOrThrow() => Provider.GetBytesOrThrow(this);

        /// <summary>
        /// Sends the request and gets the response as a string.
        /// </summary>
        /// <exception cref="WebRequestException">Thrown when the request fails or the network is unavailable.</exception>
        public Task<string> GetStringOrThrow() => Provider.GetStringOrThrow(this);

        /// <summary>
        /// Sends the request and deserializes the response to JSON object.
        /// </summary>
        /// <exception cref="WebRequestException">Thrown when the request fails or the network is unavailable.</exception>
        public Task<T> GetJsonOrThrow<T>() => Provider.GetJsonOrThrow<T>(this);

        /// <summary>
        /// Checks if there is an active internet connection by performing a lightweight request.
        /// </summary>
        /// <returns>True if internet is available, otherwise false.</returns>
        public static async Task<bool> CheckInternetConnection()
        {
            try
            {
                var request = Get("https://www.google.com/generate_204");
                await Provider.Send(request);
                return true;
            }
            catch (WebRequestException)
            {
                return false;
            }
        }
    }
}
