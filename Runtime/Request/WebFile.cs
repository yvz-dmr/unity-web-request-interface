namespace Vuzmir.UnityWebRequestInterface
{
    public class WebFile
    {
        public byte[] Bytes { get; private set; }
        public string FileName { get; private set; }
        public string MimeType { get; private set; }

        public WebFile(byte[] bytes, string fileName, string mimeType)
        {
            Bytes = bytes;
            FileName = fileName;
            MimeType = mimeType;
        }
    }
}
