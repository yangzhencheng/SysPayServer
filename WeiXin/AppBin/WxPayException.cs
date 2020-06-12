using System;

namespace SysPayServer.WeiXin.AppBin
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg) : base(msg)
        {

        }
    }
}
