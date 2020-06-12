using System;
using System.Text;
using SysPayServer.Alipay.model;
using SysPayServer.Tools;

namespace SysPayServer.Alipay
{
    public class AlipayPagePayServer
    {
        public AlipayPagePayServer()
        {

        }

        public AlipayPagePayServer(string privateKey, string publicKey)
        {
            _privateKey = privateKey;
            _publicKey = publicKey;
        }


        public AlipayPagePayServer(string privateKey, string publicKey, string appid, string returnUrl, string NotifyUrl, model.ModelPayBizContent modelPayBizContent)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;

            _appid = appid;
            _returnUrl = returnUrl;
            _notifyUrl = NotifyUrl;
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

            if (0 == _returnUrl.Length)
            {
                _Error = "别忘记填写明流的返回页";
                return false;
            }

            if (0 == _notifyUrl.Length)
            {
                _Error = "别忘记填写暗流的返回页";
                return false;
            }


            if (null == _modelPayBizContent)
            {
                _Error = "呃……您的货品信息别忘记写";
                return false;
            }


            if (0 == _modelPayBizContent.out_trade_no.Length)
            {
                _Error = "帐单号别忘记写呀！";
                return false;
            }


            if (0 == _modelPayBizContent.subject.Length)
            {
                _Error = "支付标题别忘记写！";
                return false;
            }


            if (0 == _modelPayBizContent.product_code.Length)
            {
                _Error = "怎么忘记写 Product_Code，可不应该呀！";
                return false;
            }


            if ("FAST_INSTANT_TRADE_PAY" != _modelPayBizContent.product_code && "QUICK_WAP_WAY" != _modelPayBizContent.product_code && "FACE_TO_FACE_PAYMENT" != _modelPayBizContent.product_code && "QUICK_MSECURITY_PAY" != _modelPayBizContent.product_code)
            {
                _Error = "Product_Code错啦！【FAST_INSTANT_TRADE_PAY: 电脑网站支付产品; QUICK_WAP_WAY: 手机网站支付; FACE_TO_FACE_PAYMENT: 当面付条码支付; QUICK_MSECURITY_PAY: APP支付产品】";
                return false;
            }


            Run();
            return true;
        }





        private string _privateKey = "";
        private string _publicKey = "";
        private string _appid = "";
        private string _method = "";
        private string _charset = "utf-8";
        private string _signType = "RSA2";
        private string _sign = "";
        private string _timestamp = "";
        private string _version = "1.0";
        private string _returnUrl = "";
        private string _notifyUrl = "";
        private string _bizContent = "";
        private string _Error = "";
        private model.ModelPayBizContent _modelPayBizContent = null;

        private string _url = "";




        public string PrivateKey { get => _privateKey; set => _privateKey = value; }
        public string PublicKey { get => _publicKey; set => _publicKey = value; }
        public string Appid { get => _appid; set => _appid = value; }
        public string Method { get => _method; set => _method = value; }
        public string Charset { get => _charset; set => _charset = value; }
        public string ReturnUrl { get => _returnUrl; set => _returnUrl = value; }
        public string NotifyUrl { get => _notifyUrl; set => _notifyUrl = value; }
        public ModelPayBizContent ModelPayBizContent { get => _modelPayBizContent; set => _modelPayBizContent = value; }


        public string SignType { get => _signType; }
        public string Sign { get => _sign; }
        public string Timestamp { get => _timestamp; }
        public string Version { get => _version; }
        public string Url { get => _url; }
        public string Error { get => _Error; }


        private void Run()
        {
            switch (_modelPayBizContent.product_code)
            {
                case "FAST_INSTANT_TRADE_PAY":
                    _method = "alipay.trade.page.pay";
                    break;

                case "QUICK_WAP_WAY":
                    _method = "alipay.trade.wap.pay";
                    break;

                case "FACE_TO_FACE_PAYMENT":
                    _method = "alipay.trade.pay";
                    break;

                case "QUICK_MSECURITY_PAY":
                    _method = "alipay.trade.app.pay";
                    break;
            }


            _timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");


            _bizContent = "{\"out_trade_no\":\"" + _modelPayBizContent.out_trade_no + "\",\"product_code\":\"" + _modelPayBizContent.product_code + "\",\"total_amount\":" + _modelPayBizContent.total_amount + ",\"subject\":\"" + _modelPayBizContent.subject + "\",\"body\":\"" + _modelPayBizContent.body + "\"}";
            string norParam = "app_id=" + _appid + "&biz_content=" + _bizContent + "&charset=" + _charset + "&method=" + _method + "&notify_url=" + _notifyUrl + "&return_url=" + _returnUrl + "&sign_type=" + _signType + "&timestamp=" + _timestamp + "&version=" + _version;

            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, _privateKey, _publicKey);
            _sign = rsa.Sign(norParam);

            _url = "https://openapi.alipay.com/gateway.do";
            _url += "?app_id=" + System.Web.HttpUtility.UrlEncode(_appid);
            _url += "&method=" + System.Web.HttpUtility.UrlEncode(_method);
            _url += "&return_url=" + System.Web.HttpUtility.UrlEncode(_returnUrl);
            _url += "&charset=" + System.Web.HttpUtility.UrlEncode(_charset);
            _url += "&sign_type=" + _signType;
            _url += "&sign=" + System.Web.HttpUtility.UrlEncode(_sign);
            _url += "&timestamp=" + System.Web.HttpUtility.UrlEncode(_timestamp);
            _url += "&version=" + System.Web.HttpUtility.UrlEncode(_version);
            _url += "&notify_url=" + System.Web.HttpUtility.UrlEncode(_notifyUrl);
            _url += "&biz_content=" + System.Web.HttpUtility.UrlEncode(_bizContent);

            return;
        }
    }
}
