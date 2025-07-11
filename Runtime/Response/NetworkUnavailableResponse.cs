using System;
using System.Text;

namespace Vuzmir.UnityWebRequestInterface
{
    public class NetworkUnavailableResponse : WebResponse
    {
        public NetworkUnavailableResponse()
            : base(0, null, "Network is unavailable. Please check your internet connection.") { }
    }
}
