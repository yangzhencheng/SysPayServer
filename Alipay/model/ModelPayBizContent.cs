namespace SysPayServer.Alipay.model
{
    public class ModelPayBizContent
    {
        public string out_trade_no { get; set; }
        public string product_code { get; set; }
        public string total_amount { get; set; }
        public string subject { get; set; }
        public string body { get; set; }

        /// <summary>
        /// 查询时用的“支付宝交易号” 在查询时订单号与其二选一
        /// </summary>
        /// <value>The trade no.</value>
        public string trade_no { get; set; }
    }
}
