using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Vuzmir.UnityWebRequestInterface
{
    public class UnityWebRequestProvider : WebRequestInterface
    {
        public override async Task<WebResponse> GetResponse(WebRequest request)
        {
            var unityWebRequest = await ConvertRequest(request);

            var completion = new TaskCompletionSource<WebResponse>();
            var operation = unityWebRequest.SendWebRequest();
            operation.completed += (r) =>
            {
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    completion.SetResult(
                        new WebResponse(
                            unityWebRequest.responseCode,
                            unityWebRequest.downloadHandler.data,
                            null,
                            true
                        )
                    );
                }
                else if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    completion.SetResult(
                        new WebResponse(
                            0,
                            null,
                            "Network unavailable, please check your internet connection",
                            false
                        )
                    );
                }
                else
                {
                    completion.SetResult(
                        new WebResponse(
                            unityWebRequest.responseCode,
                            unityWebRequest.downloadHandler.data,
                            unityWebRequest.error,
                            true
                        )
                    );
                }
            };
            return await completion.Task;
        }

        private async Task<UnityWebRequest> ConvertRequest(WebRequest request)
        {
            var unityWebRequest =
                request.PayloadType == WebRequestPayloadType.Form
                    ? UnityWebRequest.Post(
                        request.Url,
                        CreateForm(request.PayloadObject as WebRequestForm)
                    )
                    : new UnityWebRequest(
                        request.Url,
                        request.Method.ToString().ToUpperInvariant(),
                        new DownloadHandlerBuffer(),
                        await GetUploadHandler(request)
                    );

            unityWebRequest.method = request.Method.ToString().ToUpperInvariant();
            foreach (var header in request.Headers)
            {
                if (!header.Value.StartsWith("multipart"))
                    unityWebRequest.SetRequestHeader(header.Key, header.Value);
            }

            return unityWebRequest;
        }

        private WWWForm CreateForm(WebRequestForm requestForm)
        {
            var form = new WWWForm();
            foreach (var field in requestForm.Fields)
            {
                if (field.Value is WebFile webFile)
                    form.AddBinaryData(
                        field.Key,
                        webFile.Bytes,
                        webFile.FileName,
                        webFile.MimeType
                    );
                else if (field.Value is byte[] bytes)
                    form.AddBinaryData(field.Key, bytes);
                else if (field.Value is DateTime date)
                    form.AddField(field.Key, date.ToString("s", CultureInfo.InvariantCulture));
                else if (field.Value is IFormattable formattable)
                    form.AddField(
                        field.Key,
                        formattable.ToString("F", CultureInfo.InvariantCulture)
                    );
                else
                    form.AddField(field.Key, field.Value.ToString());
            }
            return form;
        }

        private async Task<UploadHandler> GetUploadHandler(WebRequest request)
        {
            switch (request.PayloadType)
            {
                case WebRequestPayloadType.Raw:
                    return new UploadHandlerRaw(request.PayloadObject as byte[]);
                case WebRequestPayloadType.Form:
                    return new UploadHandlerRaw(
                        CreateForm(request.PayloadObject as WebRequestForm).data
                    );
                case WebRequestPayloadType.Json:
                    if (request.PayloadObject == null)
                        return null;

                    try
                    {
                        if (request.PayloadObject is not string json)
                            json = await Task.Run(() =>
                                JsonConvert.SerializeObject(
                                    request.PayloadObject,
                                    WebRequest.JsonSerializerSettings
                                )
                            );
                        return new UploadHandlerRaw(WebRequest.Encoding.GetBytes(json));
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Unable to parse body payload, {e.Message}");
                    }
            }
            return null;
        }
    }
}
