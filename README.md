# SysPayServer
微信 + 支付宝 支付。使用得是 .net core 2.1。代码用得最简。

## 1.Alipay 支付宝支付
### 1.1. 支付
#### 1.1.1. 构造函数
> AlipayPagePayServer(string privateKey, string publicKey)

privateKey : 私钥
publicKey  : 公钥

> AlipayPagePayServer(string privateKey, string publicKey, string appid, string returnUrl, string NotifyUrl, model.ModelPayBizContent modelPayBizContent)

privateKey : 私钥
publicKey  : 公钥
appid      : 商店帐号
returnUrl  : 返回链接【明流】
NotifyUrl  : 返回链接【暗流】
modelPayBizContent   :  支付模板

#### 1.1.2. 支付调用
> bool BuildBridge()

建立支付 URL
返回得是执行结果。

如果是 true。则找下面属性得到 URL：

> public string Url { get => _url; }

之后调用 Redirect 去调 URL 对应的 H5 支付页面

如果是 false。则找下面属性得到错误：

> public string Error { get => _Error; }


### 1.2 交易查询
#### 1.2.1. 构造函数
> AlipayPaySearchServer(string privateKey, string publicKey)

privateKey : 私钥
publicKey  : 公钥

> AlipayPaySearchServer(string privateKey, string publicKey, string appid, model.ModelPayBizContent modelPayBizContent)

privateKey : 私钥
publicKey  : 公钥
appid      : 商店帐号
modelPayBizContent   :  支付模板

#### 1.2.2. 查询操作
> bool PayStatic()

直接给结果

> JsonSearchResult OrderStatus()

直接调接口查看支付宝服务器返回的源值



## 2. WeiXin 微信支付
### 2.0. 准备工作
SysPayServer/WeiXin/AppBin/WxPayConfig.cs 这里修改一下设置。
具体大家看吧！^o^

### 2.1. 统一下单
#### 2.1.1. 构造函数
> Unifiedorder(string sn, string totalFee, string snbody, string ip, string tradeType)

sn        : 订单号
totalFee  : 金额（单位：分）
snbody    : 说明
ip        : 你的 IP
tradeType : 交易类型

#### 2.1.2. 返回数据
> public SortedDictionary<string, string> ReturnValue()

内容与官方文档相近

### 2.2. 交易查询
#### 构造函数
> SearchPayStatus(string orderNo)

orderNo   : 订单号

> string TradeState()

返回交易结果

> string XMLDATA

返回的 XML


PS：最好能看看源代码，写得比较简单，也是方便大家修改，了解支付方式。
