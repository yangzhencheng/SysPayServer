namespace SysPayServer.Tools
{
    public class PostXmltoUrl
    {
        public PostXmltoUrl(string url, string postData)
        {
            _url = url;
            _postData = postData;
        }


        public string ReturnValue
        {
            get
            {
                Run();
                return _r;
            }
        }


        private string _r = "";
        private string _url = "";
        private string _postData = "";

        private void Run()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                _r = wc.UploadString(_url, "POST", _postData);
            }
        }
    }
}
