using Microsoft.AspNetCore.Razor.Language;
using Newtonsoft.Json.Linq;

namespace Bingo.Common
{
    /// <summary>
    /// 微信CKD基础方法
    /// </summary>
    public class BaseProcess
    {
        public static HttpItem GetHttpItem()
        {
            var item = new HttpItem
            {
                Encoding = null, //编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
                //Encoding = Encoding.Default,
                Method = "get", //URL     可选项 默认为Get
                Timeout = 100000, //连接超时时间     可选项默认为100000
                ReadWriteTimeout = 30000, //写入Post数据超时时间     可选项默认为30000
                IsToLower = false, //得到的HTML代码是否转成小写     可选项默认转小写
                //Cookie = "",//字符串Cookie     可选项
                UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; WOW64)AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36", //用户的浏览器类型，版本，操作系统     可选项有默认值
                Accept = "text/html, application/xhtml+xml, */*", //    可选项有默认值
                ContentType = "text/html", //返回类型    可选项有默认值
                //Referer = "http://www.jd.com",//来源URL     可选项
                Allowautoredirect = true, //是否根据３０１跳转     可选项
                //CerPath = "d:\\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数
                Connectionlimit = 1024, //最大连接数     可选项 默认为1024
                //Postdata = "C:\\PERKYSU_20121129150608_ScrubLog.txt",//Post数据     可选项GET时不需要写
                //PostDataType = PostDataType.FilePath,//默认为传入String类型，也可以设置PostDataType.Byte传入Byte类型数据
                //ProxyPwd = "123456",//代理服务器密码     可选项
                //ProxyUserName = "administrator",//代理服务器账户名     可选项
                ResultType = ResultType.String //返回数据类型，是Byte还是String
                //PostdataByte = System.Text.Encoding.Default.GetBytes("测试一下"),//如果PostDataType为Byte时要设置本属性的值
                //CookieCollection = new System.Net.CookieCollection(),//可以直接传一个Cookie集合进来
            };
            return item;
        }

        /// <summary>
        ///     获取OpenId
        /// </summary>
        /// <returns></returns>
        public static string GetOpenIdByCode(string code, string appId, string secret)
        {
            var openId = "";
            var http = new Bingo.Common.HttpHelper();
            var item = GetHttpItem();
            item.URL = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appId + "&secret=" + secret +
                       "&js_code=" + code + "&grant_type=authorization_code";
            item.Method = "get";
            item.Accept = "image/webp,image/*,*/*;q=0.8";
            item.ResultType = ResultType.Byte;
            var result = http.GetHtml(item).Html;
            //Util.AddLog(new LogInfo() { Describle = "GetUserInfo" + result });
            var jsonResult = JObject.Parse(result);
            if (jsonResult["openid"] != null)
                openId = jsonResult["openid"].Value<string>();
            return openId;
        }

        /// <summary>
        ///     获取Oauth2OpenId,公众号用
        /// </summary>
        /// <returns></returns>
        public static string GetOauth2OpenIdByCode(string code, string appId, string secret)
        {
            var openId = "";
            var http = new Bingo.Common.HttpHelper();
            var item = GetHttpItem();
            //item.URL = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appId + "&secret=" + secret +
            //           "&js_code=" + code + "&grant_type=authorization_code";

            item.URL = "https://api.weixin.qq.com/sns/oauth2/access_token?appid="+appId+"&secret="+secret+"&code="+code+"&grant_type=authorization_code";
            item.Method = "get";
            item.Accept = "image/webp,image/*,*/*;q=0.8";
            item.ResultType = ResultType.Byte;
            var result = http.GetHtml(item).Html;
            //Util.AddLog(new LogInfo() { Describle = "GetUserInfo" + result });
            var jsonResult = JObject.Parse(result);
            if (jsonResult["openid"] != null)
                openId = jsonResult["openid"].Value<string>();
            return openId;  
        }
    }

    /// <summary>
    /// JSSDK签名包
    /// </summary>
    public class JsSdkPackage
    {
        /// <summary>
        /// APPID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// NonceStr
        /// </summary>
        public string NonceStr { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// Signature
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Sha1Value
        /// </summary>
        public string Sha1Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Tiket { get; set; }

    }
}