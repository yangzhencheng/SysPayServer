using System;
using System.Collections.Generic;
using System.Text;
using SysPayServer.Tools;
using SysPayServer.WeiXin.AppBin;

namespace SysPayServer.WeiXin
{
    public class SearchPayStatus
    {
        public SearchPayStatus(string orderNo)
        {
            _orderNo = orderNo;
            _resultXML = Run();
        }


        public string TradeState()
        {
            if (-1 == _resultXML.IndexOf("<trade_state>", StringComparison.CurrentCulture))
            {
                return "No Information";
            }

            WxPayData data = new WxPayData();
            data.FromXml(_resultXML);

            return data.GetValue("trade_state").ToString().Trim().ToUpper();
        }


        public string XMLDATA
        {
            get
            {
                return _resultXML;
            }
        }




        private string _orderNo = "";
        private string _resultXML = "";
        private string appid = WxPayConfig.APPID;                // 小程序ID
        private string mch_id = WxPayConfig.MCHID;                       // 商户号
        private string nonce_str = "";          // 32位随机字符串
        private string trade_type = "JSAPI";    // 交易类型
        private readonly string apikey = WxPayConfig.KEY;




        private string Run()
        {
            if (0 == _orderNo.Length) return "0";


            // 获取 32位字符
            nonce_str = (new RandomStringLine(32)).Value;




            XMLOperator _xml = new XMLOperator();
            _xml.SetValue("appid", appid);
            _xml.SetValue("mch_id", mch_id);
            _xml.SetValue("out_trade_no", _orderNo);
            _xml.SetValue("nonce_str", nonce_str);
            _xml.SetValue("sign", Getsign());     // 获取签名

            string _strXML = _xml.ToXml();
            string _unifiedorderUrl = "https://api.mch.weixin.qq.com/pay/orderquery";
            string _xmlResult = PostXmltoUrl(_unifiedorderUrl, _strXML);

            return _xmlResult;
        }




        public string PostXmltoUrl(string url, string postData)
        {
            string _r = "";
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                _r = wc.UploadString(url, "POST", postData);
            }

            return _r;
        }



        private string Getsign()
        {
            SortedDictionary<string, string> sParams = new SortedDictionary<string, string>
            {
                { "appid", appid },
                { "mch_id", mch_id },
                { "out_trade_no", _orderNo },
                { "nonce_str", nonce_str },
            };



            int i = 0;
            string _sign = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in sParams)
            {
                if (string.IsNullOrEmpty(temp.Value) || temp.Key.ToLower() == "sign") continue;
                i++;
                sb.Append(temp.Key.Trim() + "=" + temp.Value.Trim() + "&");
            }
            sb.Append("key=" + apikey);
            int _l = nonce_str.Length;
            string signkey = sb.ToString();


            return MD5Util.GetMD5(signkey, "utf-8");
        }
    }
}
