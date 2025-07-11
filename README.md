# Unity Web Request Interface

## Overview
A simple and easy to use web request interface around Unity's UnityWebRequest API.
Provides easy-to-use methods for GET, POST, PATCH, sending JSON, form data, raw payloads, and automatic json response deserialization.

## Installation


### By [pckgs.io](https://pckgs.io)

Add the following **scoped registry** to your project's `Packages/manifest.json` file:

```json
"scopedRegistries" : [
  {
    "name": "pckgs.io",
    "url": "https://registry.pckgs.io/upm",
    "scopes": [
      "pckgs.io"
    ]
  }
],
```

Then add the package dependency under the "dependencies" section:

```json
"dependencies" : {
  "pckgs.io.com.vuzmir.unity-web-request-interface": "1.0.0"
}

```

### By Git Url
You can install this plugin via Git URL using Unity Package Manager.

```
https://github.com/yvz-dmr/unity-web-request-interface.git
```

## Usage

### Simple Get Request
```csharp
var getRequest = await WebRequest.Get("https://postman-echo.com/get").GetResponse();
```

### Post File Inside Form
```csharp
var postFile = await WebRequest
  .Post("https://postman-echo.com/post")
  .AttachFormPayload(
    new WebRequestForm()
    .Set("field1", "foo")
    .Set("field2", "bar")
    .Set("file", new WebFile(new byte[1024], "fileName", "mimeType"))
).GetResponse();
```
### Post Json Object As Body
```csharp
var postBodyAsJson = await WebRequest
  .Post("https://postman-echo.com/post")
  .AttachJsonPayload(new { id = Guid.NewGuid().ToString(), title = "title" })
  .GetResponse();
```

### Post File As Body
```csharp
var postFileAsBody = await WebRequest
  .Post("https://postman-echo.com/post")
  .AttachRawPayload(new byte[2048])
  .SetHeader("content-type", "image/png")
  .GetResponse();
```

### Handle Web Response

```csharp
var response = await WebRequest.Get("https://postman-echo.com/get").GetResponse();
if (response.IsSuccess)
{
  //Handle Response

  //Response payload as raw byte[]
  var payload = response.Payload;

  //Response payload as string
  var str = response.StringPayload;

  //Response as parsed json object
  var jsonObj = response.ToJson<MyDataType>();
}
else
{
  if (response is NetworkUnavailableResponse)
  {
    //No internet connection
  }
  else
  {
    //Handle Error
    Debug.LogError(response.Error);
  }
}
```

### Get Json Object As Response
```csharp
var jsonObject = await WebRequest.Get("https://postman-echo.com/post").GetJsonResponse<MyDataType>();
```

This method throws WebRequestException if request fails.

```csharp

try
{
  var jsonObject = await WebRequest.Get("https://postman-echo.com/post").GetJsonResponse<MyDataType>();
}
catch (NetworkUnavailableException)
{
  //No internet connection
}
catch (WebRequestException e)
{
  Debug.Log(e.StatusCode);
  Debug.Log(e.Url);
  Debug.Log(e.Body);
  Debug.Log(e.Error);
}
```

### Set Json Serializer Settings

You can set JsonSerializerSettings for Json Deserialization.

```csharp
WebRequest.JsonSerializerSettings = new JsonSerializerSettings();
```
