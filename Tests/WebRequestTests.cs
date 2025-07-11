using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Vuzmir.UnityWebRequestInterface;

public class WebRequestTests
{
    public const string TEST_IMAGE_FILE =
        "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";
    public const string GET_URL = "https://postman-echo.com/get";
    public const string POST_URL = "https://postman-echo.com/post";
    public const string PATCH_URL = "https://postman-echo.com/patch";
    public const string PUT_URL = "https://postman-echo.com/put";
    public const string DELETE_URL = "https://postman-echo.com/delete";

    public static IEnumerable<object[]> GetTestRequests =>
        new[]
        {
            new object[] { WebRequest.Get(GET_URL), 200, null },
            new object[] { WebRequest.Get(GET_URL), 200, null },
        };

    public static IEnumerable<object[]> PostTestRequests =>
        new[]
        {
            new object[]
            {
                WebRequest
                    .Post(POST_URL)
                    .AttachJsonPayload(new { title = "test-title", body = "body" }),
                200,
                "application/json",
            },
            new object[]
            {
                WebRequest
                    .Post(POST_URL)
                    .AttachRawPayload(Convert.FromBase64String(TEST_IMAGE_FILE))
                    .SetHeader("Content-Type", "image/png"),
                200,
                "image/png",
            },
            new object[]
            {
                WebRequest
                    .Post(POST_URL)
                    .AttachFormPayload(
                        new WebRequestForm()
                            .Set("test", 1)
                            .Set("test-date", DateTime.UtcNow)
                            .Set("test-float", 55)
                    ),
                200,
                "application/x-www-form-urlencoded",
            },
            new object[]
            {
                WebRequest
                    .Post(POST_URL)
                    .AttachFormPayload(
                        new WebRequestForm()
                            .Set("test", 1)
                            .Set("test-date", DateTime.UtcNow)
                            .Set("test-float", 55)
                            .Set(
                                "file",
                                new WebFile(
                                    Convert.FromBase64String(TEST_IMAGE_FILE),
                                    "test-file.png",
                                    "image/png"
                                )
                            )
                    ),
                200,
                "multipart/form-data",
            },
        };
    public static IEnumerable<object[]> PutTestRequests =>
        new[]
        {
            new object[]
            {
                WebRequest.Put(PUT_URL).AttachJsonPayload(new { title = "test-title" }),
                200,
                "application/json",
            },
        };
    public static IEnumerable<object[]> PatchTestRequests =>
        new[]
        {
            new object[]
            {
                WebRequest.Patch(PATCH_URL).AttachJsonPayload(new { title = "test-title" }),
                200,
                "application/json",
            },
        };

    public static IEnumerable<object[]> DeleteTestRequests =>
        new[] { new object[] { WebRequest.Delete(DELETE_URL), 200, null } };

    public static IEnumerable<object[]> TestRequests =>
        new List<object[]>()
            .Concat(GetTestRequests)
            .Concat(PostTestRequests)
            .Concat(PutTestRequests)
            .Concat(PatchTestRequests)
            .Concat(DeleteTestRequests);

    [Test, TestCaseSource(nameof(TestRequests))]
    public async Task Gets_Response_Correctly(
        WebRequest request,
        int expectedStatusCode,
        string contentType
    )
    {
        // Act
        var response = await request.GetResponse();

        // Assert
        Assert.AreEqual(contentType, request.ContentType);
        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccess);
        Assert.IsNull(response.Error);
        Assert.IsNotNull(response.Payload);
        Assert.IsNotEmpty(response.Payload);
        Assert.AreEqual(expectedStatusCode, response.StatusCode);
    }

    [Test, TestCaseSource(nameof(TestRequests))]
    public async Task Gets_Json_Correctly(
        WebRequest request,
        int expectedStatusCode,
        string contentType
    )
    {
        // Act
        var response = await request.GetJsonResponse<Dictionary<string, object>>();

        // Assert
        Assert.AreEqual(contentType, request.ContentType);
        Assert.IsNotNull(response);
    }

    [Test, TestCaseSource(nameof(TestRequests))]
    public async Task Gets_Bytes_Correctly(
        WebRequest request,
        int expectedStatusCode,
        string contentType
    )
    {
        // Act
        var response = await request.GetBytes();

        // Assert
        Assert.AreEqual(contentType, request.ContentType);
        Assert.IsNotNull(response);
        Assert.IsNotEmpty(response);
    }

    [Test, TestCaseSource(nameof(TestRequests))]
    public async Task Gets_String_Correctly(
        WebRequest request,
        int expectedStatusCode,
        string contentType
    )
    {
        // Act
        var response = await request.GetString();

        // Assert
        Assert.AreEqual(contentType, request.ContentType);
        Assert.IsNotNull(response);
        Assert.IsNotEmpty(response);
    }
}
