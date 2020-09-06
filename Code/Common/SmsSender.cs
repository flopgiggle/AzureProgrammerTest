using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Microsoft.Extensions.Configuration.Json;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

// newtonsoft json 模块请自行到 http://www.newtonsoft.com/json 下载，或者使用以下链接下载
// https://share.weiyun.com/abc6bd33ae2ca5bb8c83c830413c9c26
// 注意 json 库中有 .net 的多个版本，请开发者集成自己项目相应 .net 版本的 json 库

namespace Bingo.Common
{

    /// <summary>
    /// 阿里云短信发送帮助类
    /// </summary>
    public class SendSMSAliHelper
    {
        static String product = "Dysmsapi";//短信API产品名称
        static String domain = "dysmsapi.aliyuncs.com";//短信API产品域名
        static String regionIdForPop = "cn-hangzhou";
        public static string SendSMS(String accessId, String accessSecret, string PhoneNumbers, string SignName, string TemplateCode, string TemplateParam)
        {
            string result = "{}";
            SMSAliResponse response = new SMSAliResponse();
            IClientProfile profile = DefaultProfile.GetProfile(regionIdForPop, accessId, accessSecret);
            DefaultProfile.AddEndpoint(regionIdForPop, regionIdForPop, product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                //request.SignName = "上云预发测试";//"管理控制台中配置的短信签名（状态必须是验证通过）"
                //request.TemplateCode = "SMS_71130001";//管理控制台中配置的审核通过的短信模板的模板CODE（状态必须是验证通过）"
                //request.RecNum = "13567939485";//"接收号码，多个号码可以逗号分隔"
                //request.ParamString = "{\"name\":\"123\"}";//短信模板中的变量；数字需要转换为字符串；个人用户每个变量长度必须小于15个字符。"
                //SingleSendSmsResponse httpResponse = client.GetAcsResponse(request);
                //request.PhoneNumbers = "1350000000";
                //request.SignName = "xxxxxx";
                //request.TemplateCode = "SMS_xxxxxxx";
                //request.TemplateParam = "{\"code\":\"123\"}";
                //request.OutId = "xxxxxxxx";
                //请求失败这里会抛ClientException异常

                request.PhoneNumbers = PhoneNumbers;
                request.SignName = "唐人世界NPC";
                request.TemplateCode = TemplateCode;
                request.TemplateParam = TemplateParam;
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                if (sendSmsResponse.Message == "OK")
                {
                    response.code = 200;
                    response.msg = sendSmsResponse.Message;
                }
                else
                {
                    response.code = 500;
                    response.msg = sendSmsResponse.Message;
                }
            }
            catch (Exception ex)
            {
                response.code = 500;
                response.msg = ex.ToString();
            }
            result = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            return result;

        }
    }

    public class SMSAliResponse
    {
        public int code { get; set; }
        public string msg { get; set; }
    }



    class Demo
    {
        public int result { get; set; }
        public string errMsg { get; set; }
        public string ext { get; set; }
    }

    public class AliyunSmsSender
    {
        private string RegionId = "cn-hangzhou";
        private string Version = "2017-05-25";
        private string Action = "SendSms";
        private string Format = "JSON";
        private string Domain = "dysmsapi.aliyuncs.com";

        private int MaxRetryNumber = 3;
        private bool AutoRetry = true;
        private const string SEPARATOR = "&";
        private int TimeoutInMilliSeconds = 100000;

        private string AccessKeyId;
        private string AccessKeySecret;

        public AliyunSmsSender(string accessKeyId, string accessKeySecret)
        {
            this.AccessKeyId = accessKeyId;
            this.AccessKeySecret = accessKeySecret;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        public async Task<(bool success, string response)> Send(SmsObject sms)
        {
            var paramers = new Dictionary<string, string>();
            paramers.Add("PhoneNumbers", sms.Mobile);
            paramers.Add("SignName", sms.Signature);
            paramers.Add("TemplateCode", sms.TempletKey);
            paramers.Add("TemplateParam", JsonConvert.SerializeObject(sms.Data));
            paramers.Add("OutId", sms.OutId);
            paramers.Add("AccessKeyId", AccessKeyId);

            try
            {
                string url = GetSignUrl(paramers, AccessKeySecret);

                int retryTimes = 1;
                var reply = await HttpGetAsync(url);
                while (500 <= reply.StatusCode && AutoRetry && retryTimes < MaxRetryNumber)
                {
                    url = GetSignUrl(paramers, AccessKeySecret);
                    reply = await HttpGetAsync(url);
                    retryTimes++;
                }

                if (!string.IsNullOrEmpty(reply.response))
                {
                    var res = JsonConvert.DeserializeObject<Dictionary<string, string>>(reply.response);
                    if (res != null && res.ContainsKey("Code") && "OK".Equals(res["Code"]))
                    {
                        return (true, response: reply.response);
                    }
                }

                return (false, response: reply.response);
            }
            catch (Exception ex)
            {
                return (false, response: ex.Message);
            }
        }

        private string GetSignUrl(Dictionary<string, string> parameters, string accessSecret)
        {
            var imutableMap = new Dictionary<string, string>(parameters);
            imutableMap.Add("Timestamp", FormatIso8601Date(DateTime.Now));
            imutableMap.Add("SignatureMethod", "HMAC-SHA1");
            imutableMap.Add("SignatureVersion", "1.0");
            imutableMap.Add("SignatureNonce", Guid.NewGuid().ToString());
            imutableMap.Add("Action", Action);
            imutableMap.Add("Version", Version);
            imutableMap.Add("Format", Format);
            imutableMap.Add("RegionId", RegionId);

            IDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(imutableMap, StringComparer.Ordinal);
            StringBuilder canonicalizedQueryString = new StringBuilder();
            foreach (var p in sortedDictionary)
            {
                canonicalizedQueryString.Append("&")
                .Append(PercentEncode(p.Key)).Append("=")
                .Append(PercentEncode(p.Value));
            }

            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append("GET");
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode("/"));
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode(canonicalizedQueryString.ToString().Substring(1)));

            string signature = SignString(stringToSign.ToString(), accessSecret + "&");

            imutableMap.Add("Signature", signature);

            return ComposeUrl(Domain, imutableMap);
        }

        private static string FormatIso8601Date(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.CreateSpecificCulture("en-US"));
        }

        /// <summary>
        /// 签名
        /// </summary>
        public static string SignString(string source, string accessSecret)
        {
            using (var algorithm = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret.ToCharArray())))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }

        private static string ComposeUrl(string endpoint, Dictionary<String, String> parameters)
        {
            StringBuilder urlBuilder = new StringBuilder("");
            urlBuilder.Append("http://").Append(endpoint);
            if (-1 == urlBuilder.ToString().IndexOf("?"))
            {
                urlBuilder.Append("/?");
            }
            string query = ConcatQueryString(parameters);
            return urlBuilder.Append(query).ToString();
        }

        private static string ConcatQueryString(Dictionary<string, string> parameters)
        {
            if (null == parameters)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();

            foreach (var entry in parameters)
            {
                String key = entry.Key;
                String val = entry.Value;

                sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
                if (val != null)
                {
                    sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));
                }
                sb.Append("&");
            }

            int strIndex = sb.Length;
            if (parameters.Count > 0)
                sb.Remove(strIndex - 1, 1);

            return sb.ToString();
        }

        public static string PercentEncode(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(
                        string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        private async Task<(int StatusCode, string response)> HttpGetAsync(string url)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = null;
            handler.AutomaticDecompression = DecompressionMethods.GZip;

            using (var http = new HttpClient(handler))
            {
                http.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * TimeoutInMilliSeconds);
                HttpResponseMessage response = await http.GetAsync(url);
                return ((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }

    public class SmsObject
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 模板Key
        /// </summary>
        public string TempletKey { set; get; }

        /// <summary>
        /// 短信数据
        /// </summary>
        public IDictionary<string, string> Data { set; get; }

        /// <summary>
        /// 业务ID
        /// </summary>
        public string OutId { set; get; }
    }


    public class AliSendMessage
    {


        public bool  Send(string phoneNum, List<string> content, int tmplId = 0)
        {
            SendSMSAliHelper.SendSMS("LTAI4FxGHAc7TgPndH9gF21g", "SG2DN4aMWCsmTDoiE595Oz0JzZ8itE", phoneNum, "唐人世界NPC"
                , "SMS_162197307", "{\"code\":\"" + content[0].ToString() + "\"}"
            );
            return true;
        }

        public bool Send2(string phoneNum, List<string> content, int tmplId = 0)
        {

            string accessKeyId = "LTAI4FxGHAc7TgPndH9gF21g";
            string accessKeySecret = "SG2DN4aMWCsmTDoiE595Oz0JzZ8itE";
            IDictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Code", content[0]);
            var sms = new SmsObject
            {
                Mobile = phoneNum,
                Signature = "LTAIBgdZ9URrwgz3",
                TempletKey = "SMS_162197307",
                Data = data,
                //OutId = "OutId"
            };

            var res = new AliyunSmsSender(accessKeyId, accessKeySecret).Send(sms).Result;
            return true;

        }

        public bool Send3(string phoneNum, List<string> content, int tmplId =0)
        {
             IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAIeqJQy3lgjeTA", "653etQc0eUF2x62cfZuvLprZiz1njF");
            DefaultAcsClient client = new DefaultAcsClient(profile);
            CommonRequest request = new CommonRequest("", "2017-05-25", "SendSms");
            request.Method = MethodType.POST;
            request.Url = "dysmsapi.aliyuncs.com";
            //request.Version = "2017-05-25";
            //request.ActionName = "SendSms";
            // request.Protocol = ProtocolType.HTTP;
            request.QueryParameters = new Dictionary<string, string>();
            request.QueryParameters.Add("PhoneNumbers", phoneNum);
            request.QueryParameters.Add("SignName", "LTAIBgdZ9URrwgz3");
            request.QueryParameters.Add("TemplateCode", "SMS_162197307");
            request.QueryParameters.Add("TemplateParam", content[0]);
            try {
               
                CommonResponse response = client.GetAcsResponse(request);
                Console.WriteLine(System.Text.Encoding.Default.GetString(response.HttpResponse.Content));
            }
            catch (ServerException e)
            {
                Console.WriteLine(e);
            }
            catch (ClientException e)
            {
                Console.WriteLine(e);
            }
            return true;
        }

    }

    /// <summary>
    /// 发送短信
    /// </summary>
    public class MobileMessageManager
    {
       // private LogManage log = new LogManage();

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="phoneNum">电话号码</param>
        /// <param name="content">内容</param>
        /// <param name="tmplId">腾讯配置的短信模板Id</param>
        /// <returns></returns>
        public  bool Send(string phoneNum, List<string> content,int tmplId)
        {
            int sdkappid = 1400098474;
            string appkey = "9b1746c5f0bd2eb1f305a1eae501eab9";
            SmsSingleSender singleSender = new SmsSingleSender(sdkappid, appkey);

            // 指定模板单发
            // 假设短信模板内容为：测试短信，{1}，{2}，{3}，上学。
            var singleResult = singleSender.SendWithParam("86", phoneNum, tmplId, content, "", "", "");

            //支付日志
            //log.CreateLog(new OperationLogDto()
            //{
            //    Title = "发送验证短信",
            //    LogType = (int)BusinessEnum.LogType.SendMessage,
            //    Data = phoneNum + " " + JsonConvert.SerializeObject(content) + "\n" + JsonConvert.SerializeObject(singleResult) ,
            //    BusinessKey = phoneNum
            //});
            return true;
        }

    //    static void Main(string[] args)
    //    {
    //        // 请根据实际 appid 和 appkey 进行开发，以下只作为演示 sdk 使用
    //        // appid,appkey,templId申请方式可参考接入指南 https://www.qcloud.com/document/product/382/3785#5-.E7.9F.AD.E4.BF.A1.E5.86.85.E5.AE.B9.E9.85.8D.E7.BD.AE
    //        int sdkappid = 1400098474;
    //        string appkey = "9b1746c5f0bd2eb1f305a1eae501eab9";

    //        string phoneNumber1 = "12345678901";
    //        string phoneNumber2 = "12345678902";
    //        string phoneNumber3 = "12345678903";
    //        int tmplId = 7839;

    //        try
    //        {
    //            SmsSingleSenderResult singleResult;
    //            SmsSingleSender singleSender = new SmsSingleSender(sdkappid, appkey);

    //            singleResult = singleSender.Send(0, "86", phoneNumber2, "测试短信，普通单发，深圳，小明，上学。", "", "");
    //            Console.WriteLine(singleResult);

    //            List<string> templParams = new List<string>();
    //            templParams.Add("指定模板单发");
    //            templParams.Add("深圳");
    //            templParams.Add("小明");
    //            // 指定模板单发
    //            // 假设短信模板内容为：测试短信，{1}，{2}，{3}，上学。
    //            singleResult = singleSender.SendWithParam("86", phoneNumber2, tmplId, templParams, "", "", "");
    //            Console.WriteLine(singleResult);

    //            SmsMultiSenderResult multiResult;
    //            SmsMultiSender multiSender = new SmsMultiSender(sdkappid, appkey);
    //            List<string> phoneNumbers = new List<string>();
    //            phoneNumbers.Add(phoneNumber1);
    //            phoneNumbers.Add(phoneNumber2);
    //            phoneNumbers.Add(phoneNumber3);

    //            // 普通群发
    //            // 下面是 3 个假设的号码
    //            multiResult = multiSender.Send(0, "86", phoneNumbers, "测试短信，普通群发，深圳，小明，上学。", "", "");
    //            Console.WriteLine(multiResult);

    //            // 指定模板群发
    //            // 假设短信模板内容为：测试短信，{1}，{2}，{3}，上学。
    //            templParams.Clear();
    //            templParams.Add("指定模板群发");
    //            templParams.Add("深圳");
    //            templParams.Add("小明");
    //            multiResult = multiSender.SendWithParam("86", phoneNumbers, tmplId, templParams, "", "", "");
    //            Console.WriteLine(multiResult);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e);
    //        }
    //    }
    }


    class SmsSingleSender
    {
        int sdkappid;
        string appkey;
        string url = "https://yun.tim.qq.com/v5/tlssmssvr/sendsms";

        SmsSenderUtil util = new SmsSenderUtil();

        public SmsSingleSender(int sdkappid, string appkey)
        {
            this.sdkappid = sdkappid;
            this.appkey = appkey;
        }

        /**
         * 普通单发短信接口，明确指定内容，如果有多个签名，请在内容中以【】的方式添加到信息内容中，否则系统将使用默认签名
         * @param type 短信类型，0 为普通短信，1 营销短信
         * @param nationCode 国家码，如 86 为中国
         * @param phoneNumber 不带国家码的手机号
         * @param msg 信息内容，必须与申请的模板格式一致，否则将返回错误
         * @param extend 扩展码，可填空
         * @param ext 服务端原样返回的参数，可填空
         * @return SmsSingleSenderResult
         */
        public SmsSingleSenderResult Send(
            int type,
            string nationCode,
            string phoneNumber,
            string msg,
            string extend,
            string ext)
        {
            /*
            请求包体
            {
                "tel": {
                    "nationcode": "86", 
                    "mobile": "13788888888"
                },
                "type": 0, 
                "msg": "你的验证码是1234", 
                "sig": "fdba654e05bc0d15796713a1a1a2318c", 
                "time": 1479888540,
                "extend": "",
                "ext": ""
            }
            应答包体
            {
                "result": 0,
                "errmsg": "OK", 
                "ext": "", 
                "sid": "xxxxxxx", 
                "fee": 1
            }
            */
            if (0 != type && 1 != type)
            {
                throw new Exception("type " + type + " error");
            }
            if (null == extend)
            {
                extend = "";
            }
            if (null == ext)
            {
                ext = "";
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();

            JObject tel = new JObject();
            tel.Add("nationcode", nationCode);
            tel.Add("mobile", phoneNumber);

            data.Add("tel", tel);
            data.Add("msg", msg);
            data.Add("type", type);
            data.Add("sig", util.StrToHash(String.Format(
                "appkey={0}&random={1}&time={2}&mobile={3}",
                appkey, random, curTime, phoneNumber)));
            data.Add("time", curTime);
            data.Add("extend", extend);
            data.Add("ext", ext);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsSingleSenderResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToSingleSenderResult(responseStr);
            }
            else
            {
                result = new SmsSingleSenderResult();
                result.result = -1;
                result.errmsg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }

        /**
         * 指定模板单发
         * @param nationCode 国家码，如 86 为中国
         * @param phoneNumber 不带国家码的手机号
         * @param templId 模板 id
         * @param templParams 模板参数列表，如模板 {1}...{2}...{3}，那么需要带三个参数
         * @param extend 扩展码，可填空
         * @param ext 服务端原样返回的参数，可填空
         * @return SmsSingleSenderResult
         */
        public SmsSingleSenderResult SendWithParam(
            string nationCode,
            string phoneNumber,
            int templId,
            List<string> templParams,
            string sign,
            string extend,
            string ext)
        {
            /*
            请求包体
            {
                "tel": {
                    "nationcode": "86",
                    "mobile": "13788888888"
                },
                "sign": "腾讯云",
                "tpl_id": 19,
                "params": [
                    "验证码", 
                    "1234",
                    "4"
                ],
                "sig": "fdba654e05bc0d15796713a1a1a2318c",
                "time": 1479888540,
                "extend": "",
                "ext": ""
            }
            应答包体
            {
                "result": 0,
                "errmsg": "OK", 
                "ext": "", 
                "sid": "xxxxxxx", 
                "fee": 1
            }
            */
            if (null == sign)
            {
                sign = "";
            }
            if (null == extend)
            {
                extend = "";
            }
            if (null == ext)
            {
                ext = "";
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();

            JObject tel = new JObject();
            tel.Add("nationcode", nationCode);
            tel.Add("mobile", phoneNumber);

            data.Add("tel", tel);
            data.Add("sig", util.CalculateSigForTempl(appkey, random, curTime, phoneNumber));
            data.Add("tpl_id", templId);
            data.Add("params", util.SmsParamsToJSONArray(templParams));
            data.Add("sign", sign);
            data.Add("time", curTime);
            data.Add("extend", extend);
            data.Add("ext", ext);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsSingleSenderResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToSingleSenderResult(responseStr);
            }
            else
            {
                result = new SmsSingleSenderResult();
                result.result = -1;
                result.errmsg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
    }

    class SmsSingleSenderResult
    {
        /*
        {
            "result": 0,
            "errmsg": "OK", 
            "ext": "", 
            "sid": "xxxxxxx", 
            "fee": 1
        }
         */
        public int result { set; get; }
        public string errmsg { set; get; }
        public string ext { set; get; }
        public string sid { set; get; }
        public int fee { set; get; }

        public override string ToString()
        {
            return string.Format(
                "SmsSingleSenderResult\nresult {0}\nerrMsg {1}\next {2}\nsid {3}\nfee {4}",
                result, errmsg, ext, sid, fee);
        }
    }

    class SmsMultiSender
    {
        int sdkappid;
        string appkey;
        string url = "https://yun.tim.qq.com/v5/tlssmssvr/sendmultisms2";

        SmsSenderUtil util = new SmsSenderUtil();

        public SmsMultiSender(int sdkappid, string appkey)
        {
            this.sdkappid = sdkappid;
            this.appkey = appkey;
        }

        /**
         * 普通群发短信接口，明确指定内容，如果有多个签名，请在内容中以【】的方式添加到信息内容中，否则系统将使用默认签名
         * 【注意】海外短信无群发功能
         * @param type 短信类型，0 为普通短信，1 营销短信
         * @param nationCode 国家码，如 86 为中国
         * @param phoneNumbers 不带国家码的手机号列表
         * @param msg 信息内容，必须与申请的模板格式一致，否则将返回错误
         * @param extend 扩展码，可填空
         * @param ext 服务端原样返回的参数，可填空
         * @return SmsMultiSenderResult
         */
        public SmsMultiSenderResult Send(
            int type,
            string nationCode,
            List<string> phoneNumbers,
            string msg,
            string extend,
            string ext)
        {
            /*
            请求包体
            {
                "tel": [
                    {
                        "nationcode": "86", 
                        "mobile": "13788888888"
                    }, 
                    {
                        "nationcode": "86", 
                        "mobile": "13788888889"
                    }
                ], 
                "type": 0, 
                "msg": "你的验证码是1234", 
                "sig": "fdba654e05bc0d15796713a1a1a2318c",
                "time": 1479888540,
                "extend": "", 
                "ext": ""
            }
            应答包体
            {
                "result": 0, 
                "errmsg": "OK", 
                "ext": "", 
                "detail": [
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888888", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }, 
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888889", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }
                ]
            }
            */
            if (0 != type && 1 != type)
            {
                throw new Exception("type " + type + " error");
            }
            if (null == extend)
            {
                extend = "";
            }
            if (null == ext)
            {
                ext = "";
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();
            data.Add("tel", util.PhoneNumbersToJSONArray(nationCode, phoneNumbers));
            data.Add("type", type);
            data.Add("msg", msg);
            data.Add("sig", util.CalculateSig(appkey, random, curTime, phoneNumbers));
            data.Add("time", curTime);
            data.Add("extend", extend);
            data.Add("ext", ext);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsMultiSenderResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToMultiSenderResult(responseStr);
            }
            else
            {
                result = new SmsMultiSenderResult();
                result.result = -1;
                result.errmsg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }

        /**
         * 指定模板群发
         * 【注意】海外短信无群发功能
         * @param nationCode 国家码，如 86 为中国
         * @param phoneNumbers 不带国家码的手机号列表
         * @param templId 模板 id
         * @param params 模板参数列表
         * @param sign 签名，如果填空，系统会使用默认签名
         * @param extend 扩展码，可以填空
         * @param ext 服务端原样返回的参数，可以填空
         * @return SmsMultiSenderResult
         */
        public SmsMultiSenderResult SendWithParam(
            String nationCode,
            List<string> phoneNumbers,
            int templId,
            List<string> templParams,
            string sign,
            string extend,
            string ext)
        {
            /*
            请求包体
            {
                "tel": [
                    {
                        "nationcode": "86", 
                        "mobile": "13788888888"
                    }, 
                    {
                        "nationcode": "86", 
                        "mobile": "13788888889"
                    }
                ], 
                "type": 0, 
                "msg": "你的验证码是1234", 
                "sig": "fdba654e05bc0d15796713a1a1a2318c",
                "time": 1479888540,
                "extend": "", 
                "ext": ""
            }
            应答包体
            {
                "result": 0, 
                "errmsg": "OK", 
                "ext": "", 
                "detail": [
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888888", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }, 
                    {
                        "result": 0, 
                        "errmsg": "OK", 
                        "mobile": "13788888889", 
                        "nationcode": "86", 
                        "sid": "xxxxxxx", 
                        "fee": 1
                    }
                ]
            }
            */
            if (null == sign)
            {
                sign = "";
            }
            if (null == extend)
            {
                extend = "";
            }
            if (null == ext)
            {
                ext = "";
            }

            long random = util.GetRandom();
            long curTime = util.GetCurTime();

            // 按照协议组织 post 请求包体
            JObject data = new JObject();
            data.Add("tel", util.PhoneNumbersToJSONArray(nationCode, phoneNumbers));
            data.Add("sig", util.CalculateSigForTempl(appkey, random, curTime, phoneNumbers));
            data.Add("tpl_id", templId);
            data.Add("params", util.SmsParamsToJSONArray(templParams));
            data.Add("sign", sign);
            data.Add("time", curTime);
            data.Add("extend", extend);
            data.Add("ext", ext);

            string wholeUrl = url + "?sdkappid=" + sdkappid + "&random=" + random;
            HttpWebRequest request = util.GetPostHttpConn(wholeUrl);
            byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            request.ContentLength = requestData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestData, 0, requestData.Length);
            requestStream.Close();

            // 接收返回包
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            string responseStr = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            SmsMultiSenderResult result;
            if (HttpStatusCode.OK == response.StatusCode)
            {
                result = util.ResponseStrToMultiSenderResult(responseStr);
            }
            else
            {
                result = new SmsMultiSenderResult();
                result.result = -1;
                result.errmsg = "http error " + response.StatusCode + " " + responseStr;
            }
            return result;
        }
    }

    class SmsMultiSenderResult
    {
        /*
        {
            "result": 0, 
            "errmsg": "OK", 
            "ext": "", 
            "detail": [
                {
                    "result": 0, 
                    "errmsg": "OK", 
                    "mobile": "13788888888", 
                    "nationcode": "86", 
                    "sid": "xxxxxxx", 
                    "fee": 1
                }, 
                {
                    "result": 0, 
                    "errmsg": "OK", 
                    "mobile": "13788888889", 
                    "nationcode": "86", 
                    "sid": "xxxxxxx", 
                    "fee": 1
                }
            ]
        }
            */
        public class Detail
        {
            public int result { get; set; }
            public string errmsg { get; set; }
            public string mobile { get; set; }
            public string nationcode { get; set; }
            public string sid { get; set; }
            public int fee { get; set; }

            public override string ToString()
            {
                return string.Format(
                        "\tDetail result {0} errmsg {1} mobile {2} nationcode {3} sid {4} fee {5}",
                        result, errmsg, mobile, nationcode, sid, fee);
            }
        }

        public int result;
        public string errmsg = "";
        public string ext = "";
        public IList<Detail> detail;

        public override string ToString()
        {
            if (null != detail)
            {
                return String.Format(
                        "SmsMultiSenderResult\nresult {0}\nerrmsg {1}\next {2}\ndetail:\n{3}",
                        result, errmsg, ext, String.Join("\n", detail));
            }
            else
            {
                return String.Format(
                     "SmsMultiSenderResult\nresult {0}\nerrmsg {1}\next {2}\n",
                     result, errmsg, ext);
            }
        }
    }

    class SmsSenderUtil
    {
        Random random = new Random();

        public HttpWebRequest GetPostHttpConn(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            return request;
        }

        public long GetRandom()
        {
            return random.Next(999999) % 900000 + 100000;
        }

        public long GetCurTime()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        // 将二进制的数值转换为 16 进制字符串，如 "abc" => "616263"
        private static string ByteArrayToHex(byte[] byteArray)
        {
            string returnStr = "";
            if (byteArray != null)
            {
                for (int i = 0; i < byteArray.Length; i++)
                {
                    returnStr += byteArray[i].ToString("x2");
                }
            }
            return returnStr;
        }

        public string StrToHash(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] resultByteArray = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            return ByteArrayToHex(resultByteArray);
        }

        // 将单发回包解析成结果对象
        public SmsSingleSenderResult ResponseStrToSingleSenderResult(string str)
        {
            SmsSingleSenderResult result = JsonConvert.DeserializeObject<SmsSingleSenderResult>(str);
            return result;
        }

        // 将群发回包解析成结果对象
        public SmsMultiSenderResult ResponseStrToMultiSenderResult(string str)
        {
            SmsMultiSenderResult result = JsonConvert.DeserializeObject<SmsMultiSenderResult>(str);
            return result;
        }

        public JArray SmsParamsToJSONArray(List<string> templParams)
        {
            JArray smsParams = new JArray();
            foreach (string templParamsElement in templParams)
            {
                smsParams.Add(templParamsElement);
            }
            return smsParams;
        }

        public JArray PhoneNumbersToJSONArray(string nationCode, List<string> phoneNumbers)
        {
            JArray tel = new JArray();
            int i = 0;
            do
            {
                JObject telElement = new JObject();
                telElement.Add("nationcode", nationCode);
                telElement.Add("mobile", phoneNumbers.ElementAt(i));
                tel.Add(telElement);
            } while (++i < phoneNumbers.Count);

            return tel;
        }

        public string CalculateSigForTempl(
            string appkey,
            long random,
            long curTime,
            List<string> phoneNumbers)
        {
            string phoneNumbersString = phoneNumbers.ElementAt(0);
            for (int i = 1; i < phoneNumbers.Count; i++)
            {
                phoneNumbersString += "," + phoneNumbers.ElementAt(i);
            }
            return StrToHash(String.Format(
                "appkey={0}&random={1}&time={2}&mobile={3}",
                appkey, random, curTime, phoneNumbersString));
        }

        public string CalculateSigForTempl(
            string appkey,
            long random,
            long curTime,
            string phoneNumber)
        {
            List<string> phoneNumbers = new List<string>();
            phoneNumbers.Add(phoneNumber);
            return CalculateSigForTempl(appkey, random, curTime, phoneNumbers);
        }

        public string CalculateSig(
            string appkey,
            long random,
            long curTime,
            List<string> phoneNumbers)
        {
            string phoneNumbersString = phoneNumbers.ElementAt(0);
            for (int i = 1; i < phoneNumbers.Count; i++)
            {
                phoneNumbersString += "," + phoneNumbers.ElementAt(i);
            }
            return StrToHash(String.Format(
                    "appkey={0}&random={1}&time={2}&mobile={3}",
                    appkey, random, curTime, phoneNumbersString));
        }
    }

}
