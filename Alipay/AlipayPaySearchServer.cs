using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using SysPayServer.Alipay.model;
using SysPayServer.Tools;
using SysPayServer.TypeLib;

namespace SysPayServer.Alipay
{
    public class AlipayPaySearchServer
    {
        public AlipayPaySearchServer()
        {
        }


        public AlipayPaySearchServer(string privateKey, string publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }


        public AlipayPaySearchServer(string privateKey, string publicKey, string appid, model.ModelPayBizContent modelPayBizContent)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;

            _appid = appid;
            ModelPayBizContent = modelPayBizContent;
        }



        public bool BuildBridge()
        {
            _Error = "";

            if (0 == _privateKey.Length)
            {
                _Error = "请填写私钥";
                return false;
            }

            if (0 == _publicKey.Length)
            {
                _Error = "请填写公钥";
                return false;
            }

            if (0 == _appid.Length)
            {
                _Error = "请填写商店帐号";
                return false;
            }


            if (0 == _appid.Length)
            {
                _Error = "请填写商店帐号";
                return false;
            }


            if (null == _modelPayBizContent)
            {
                _Error = "呃……您的货品信息别忘记写";
                return false;
            }


            if (0 == _modelPayBizContent.out_trade_no.Length && 0 == _modelPayBizContent.trade_no.Length)
            {
                _Error = "订单号【out_trade_no】与支付宝交易号【trade_no】至少选择一个";
                return false;
            }


            Run();
            return true;
        }



        public JsonSearchResult OrderStatus()
        {
            JsonSearchResult _json = new JsonSearchResult();

            Run();


            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_url);
            httpWebRequest.Method = "Get";
            httpWebRequest.Accept = "*/*";

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string text = streamReader.ReadToEnd();

            streamReader.Close();
            responseStream.Close();
            httpWebResponse.Close();

            _json = JsonConvert.DeserializeObject<JsonSearchResult>(text);
            return _json;
        }



        public bool PayStatic()
        {
            JsonSearchResult _json = OrderStatus();

            if (null == _json) return false;
            if (null == _json.alipay_trade_query_response.msg) return false;
            if ("Success" != _json.alipay_trade_query_response.msg) return false;

            return true;
        }




        private string _privateKey = "";
        private string _publicKey = "";
        private string _appid = "";
        private string _method = "alipay.trade.query";
        private string _charset = "utf-8";
        private string _signType = "RSA2";
        private string _sign = "";
        private string _timestamp = "";
        private string _version = "1.0";
        private string _bizContent = "";
        private string _url = "";
        private string _Error = "";
        private model.ModelPayBizContent _modelPayBizContent = null;



        public string PrivateKey { get => _privateKey; set => _privateKey = value; }
        public string PublicKey { get => _publicKey; set => _publicKey = value; }
        public string Appid { get => _appid; set => _appid = value; }
        public string Method { get => _method; set => _method = value; }
        public string Charset { get => _charset; set => _charset = value; }
        public ModelPayBizContent ModelPayBizContent { get => _modelPayBizContent; set => _modelPayBizContent = value; }


        public string SignType { get => _signType; }
        public string Sign { get => _sign; }
        public string Timestamp { get => _timestamp; }
        public string Version { get => _version; }
        public string Error { get => _Error; }
        public string Url { get => _url; }



        private void Run()
        {
            _timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            if (0 < _modelPayBizContent.out_trade_no.Length)
            {
                _bizContent = "{\"out_trade_no\":\"" + _modelPayBizContent.out_trade_no + "\"}";
            }
            else
            {
                _bizContent = "{\"trade_no\":\"" + _modelPayBizContent.trade_no + "\"}";
            }

            string norParam = "app_id=" + _appid + "&biz_content=" + _bizContent + "&charset=" + _charset + "&method=" + _method + "&sign_type=" + _signType + "&timestamp=" + _timestamp + "&version=" + _version;

            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, _privateKey, _publicKey);
            _sign = rsa.Sign(norParam);


            _url = "https://openapi.alipay.com/gateway.do";
            _url += "?app_id=" + System.Web.HttpUtility.UrlEncode(_appid);
            _url += "&method=" + System.Web.HttpUtility.UrlEncode(_method);
            _url += "&charset=" + System.Web.HttpUtility.UrlEncode(_charset);
            _url += "&sign_type=" + _signType;
            _url += "&sign=" + System.Web.HttpUtility.UrlEncode(_sign);
            _url += "&timestamp=" + System.Web.HttpUtility.UrlEncode(_timestamp);
            _url += "&version=" + System.Web.HttpUtility.UrlEncode(_version);
            _url += "&biz_content=" + System.Web.HttpUtility.UrlEncode(_bizContent);
        }
    }
}
