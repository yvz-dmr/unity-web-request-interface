using System.Threading.Tasks;

namespace Vuzmir.UnityWebRequestInterface
{
    public interface IWebRequestInterface
    {
        public Task<WebResponse> GetResponse(WebRequest request);
        public Task Send(WebRequest request);
    }
}
