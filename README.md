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
    "url": "https://upm.pckgs.io",
    "scopes": [
      "com.vuzmir"
    ]
  }
],
```

Then add the package dependency under the "dependencies" section:

```json
"dependencies" : {
  "com.vuzmir.unity-web-request-interface": "1.0.0"
}
```

### By Git Url
You can install this plugin via Git URL using Unity Package Manager.

```
https://github.com/yvz-dmr/unity-web-request-interface.git
```

## Usage

### Create Request

#### Simple Get
```csharp
var request = await WebRequest.Get("https://postman-echo.com/get");
```

#### Post File Inside Form
```csharp
var request = await WebRequest
  .Post("https://postman-echo.com/post")
  .AttachFormPayload(
    new WebRequestForm()
    .Set("field1", "foo")
    .Set("field2", "bar")
    .Set("file", new WebFile(new byte[1024], "fileName", "mimeType"))
);
```
#### Post Json Object As Body
```csharp
var request = await WebRequest
  .Post("https://postman-echo.com/post")
  .AttachJsonPayload(new { id = Guid.NewGuid().ToString(), title = "title" });
```

#### Post File As Body
```csharp
var request = await WebRequest
  .Post("https://postman-echo.com/post")
  .AttachRawPayload(new byte[2048])
  .SetHeader("content-type", "image/png");
```

### Get Response

#### Get Web Response

```csharp
WebRequest request; // Any web request object
var response = await request.GetResponse();
```

#### Get Web Response with Json Payload

```csharp
WebRequest request; // Any web request object
WebResponse<MyDataObject> response = await request.GetResponse<MyDataObject>();
```

#### Get Server Response as Json Payload

```csharp
WebRequest request; // Any web request object
MyDataObject response = await request.GetJsonOrThrow<MyDataObject>();
```

#### Get Response as byte[]

```csharp
WebRequest request; // Any web request object
var bytes = await request.GetBytesOrThrow();
```

#### Get Response as string

```csharp
WebRequest request; // Any web request object
var str = await request.GetStringOrThrow();
```

### Handle Response

```csharp
WebResponse response; // Any web response object
if (response.IsSuccess)
{
  //Handle Response

  //Response payload as raw byte[]
  var payload = response.Payload;

  //Response payload as string
  var str = response.StringPayload;

  //Response as parsed json object
  var jsonObj = response.ToJson<MyDataObject>();
}
else
{
  if (response.IsNetworkAvailable)
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

### Handle Exception

```csharp
WebRequest request; // Any web request object
try{
  await request.Send();
  //or
  var bytes = await request.GetBytesOrThrow();
  //or
  var str = await request.GetStringOrThrow();
  //or
  var obj = await request.GetJsonOrThrow<MyDataObject>();
}
catch(WebRequestException e){
  // HTTP status code of the response (e.g., 200, 404). If the response couldn't be parsed (e.g., invalid JSON), this will be 0.
  Debug.Log(e.StatusCode);

  // The URL that was requested
  Debug.Log(e.Url);

  // Error message
  Debug.Log(e.Error);

  // Body content as string returned by the server, if any (e.g., JSON string, HTML, plain text)
  Debug.Log(e.Body);

  // Indicates whether the device had an active internet connection during the request
  Debug.Log(e.IsNetworkAvailable);
}
```

### Configure

#### Set Json Serializer Settings

You can set JsonSerializerSettings for Json Deserialization.

```csharp
WebRequest.JsonSerializerSettings = new JsonSerializerSettings();
```
