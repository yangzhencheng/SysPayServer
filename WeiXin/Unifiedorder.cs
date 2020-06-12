using System;
using System.Collections.Generic;
using System.Text;
using SysPayServer.Tools;
using SysPayServer.WeiXin.AppBin;

namespace SysPayServer.WeiXin
{
    public class Unifiedorder
    {
        public Unifiedorder(string sn, string totalFee, string snbody, string ip, string tradeType)
        {
            out_trade_no = sn;
            total_fee = totalFee;
            spbill_create_ip = ip;
            body = snbody;
            trade_type = tradeType;
        }



        public SortedDictionary<string, string> ReturnValue()
        {
            Run();
            return returnResult;
        }







        #region 静态区
        private string appid = WxPayConfig.APPID;               // 小程序ID
        private string mch_id = WxPayConfig.MCHID;              // 商户号
        private string out_trade_no = "";                       // 商户订单号
        private string total_fee = "";                          // 标价金额，单位为分
        private string body = "";                               // 商品描述
        private string spbill_create_ip = "";                   // 终端IP
        private string notify_url = WxPayConfig.NOTIFY_URL;     // 通知地址
        private string trade_type = "";                   // 交易类型
        private string nonce_str = "";                          // 32位随机字符串
        private string openid = "";                             // 用户标识 - trade_type=JSAPI时必传
        private string _2codeUrl = "";                            // 返回的二维码链接
        private readonly string apikey = WxPayConfig.KEY;
        private SortedDictionary<string, string> returnResult = null;




        private void Run()
        {
            // 获取 32位字符
            nonce_str = (new RandomStringLine(32)).Value;

            // 建立 XML
            XMLOperator _xml = new XMLOperator();
            _xml.SetValue("appid", appid);
            _xml.SetValue("mch_id", mch_id);
            _xml.SetValue("nonce_str", nonce_str);
            _xml.SetValue("body", body);
            _xml.SetValue("out_trade_no", out_trade_no);
            _xml.SetValue("total_fee", total_fee);
            _xml.SetValue("spbill_create_ip", spbill_create_ip);
            _xml.SetValue("notify_url", notify_url);
            _xml.SetValue("trade_type", trade_type);
            _xml.SetValue("sign", Getsign());                   // 获取签名

            string _strXML = _xml.ToXml();
            string _unifiedorderUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string _xmlResult = (new PostXmltoUrl(_unifiedorderUrl, _strXML)).ReturnValue;

            WxPayData data = new WxPayData();
            data.FromXml(_xmlResult);


            string return_code = data.GetValue("return_code").ToString().Trim();


            if ("SUCCESS" != return_code)
            {
                // 支付功能关闭
                returnResult = new SortedDictionary<string, string>
                {
                    {"sign", "0x10320002"},
                    {"info", "支付功能关闭！"}
                };

                return;
            }

            if (0 <= _xmlResult.IndexOf("err_code_des", StringComparison.Ordinal))
            {
                returnResult = new SortedDictionary<string, string>
                {
                    {"sign", "0x10320002"},
                    {"info", data.GetValue("err_code_des").ToString().Trim()}
                };

                return;
            }


            string _rTimeStamp = (new DateTimeOffset(DateTime.Now)).ToUnixTimeSeconds().ToString();
            string _rNonceStr = (new RandomStringLine(32)).Value.ToUpper();
            string _rPackAge = "prepay_id=" + data.GetValue("prepay_id").ToString().Trim();
            string _paySign = "";
            _2codeUrl = null == data.GetValue("code_url") ? "" : data.GetValue("code_url").ToString().Trim();


            _paySign += "appId=" + WxPayConfig.APPID;
            _paySign += "&nonceStr=" + _rNonceStr;
            _paySign += "&package=" + _rPackAge;
            _paySign += "&signType=MD5";
            _paySign += "&timeStamp=" + _rTimeStamp;
            _paySign += "&key=" + WxPayConfig.KEY;


            _paySign = MD5Util.GetMD5(_paySign, "utf-8").ToUpper();      // 进行 MD5 加密


            returnResult = new SortedDictionary<string, string>
            {
                {"sign", "0x10320000"},
                {"appId", WxPayConfig.APPID},       // 小程序ID
                {"timeStamp", _rTimeStamp},         // 时间戳
                {"nonceStr", _rNonceStr},           // 随机串
                {"package", _rPackAge},             // 数据包
                {"signType", "MD5"},                // 签名方式
                {"paySign", _paySign},              // 签名
                {"2codeUrl", _2codeUrl}                // 二维码链接
            };
        }



        /// <summary>
        /// 获取微信签名
        /// </summary>
        /// <returns></returns>
        private string Getsign()
        {
            SortedDictionary<string, string> sParams = new SortedDictionary<string, string>
            {
                { "appid", appid },
                { "mch_id", mch_id },
                { "nonce_str", nonce_str },
                { "body", body },
                { "out_trade_no", out_trade_no },
                { "total_fee", total_fee },
                { "spbill_create_ip", spbill_create_ip },
                { "notify_url", notify_url },
                { "trade_type", trade_type },
                { "openid", openid}
            };



            int i = 0;
            string _sign = string.Empty;
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> temp in sParams)
            {
                if (string.IsNullOrEmpty(temp.Value) || temp.Key.ToLower() == "sign") continue;
                if (temp.Key.ToLower() == "openid" && "JSAPI" != trade_type && 0 == openid.Length) continue;
                i++;
                sb.Append(temp.Key.Trim() + "=" + temp.Value.Trim() + "&");
            }
            sb.Append("key=" + apikey);
            int _l = nonce_str.Length;
            string signkey = sb.ToString();


            return MD5Util.GetMD5(signkey, "utf-8");
        }
        #endregion 静态区
    }
}
