using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KidneyCareApi.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using AutoMapper;
//using Gma.QrCodeNet.Encoding;
//using Gma.QrCodeNet.Encoding.Windows.Render;

//using Quartz;
//using Quartz.Impl;
//using Quartz.Impl.Triggers;
//using ThinkGeo.MapSuite.Core;

namespace Bingo.Common
{

    public class AppSettings
    {
        public string HttpUrl { get; set; }
        public string Copyright { get; set; }
    }
    public static class Util
    {
        public static Dictionary<string,string> configDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            configDictionary.Add("TencentMapkey", "GB6BZ-4ZZK3-NOY36-YLT42-ZJG3V-KUFXU");

            configDictionary.Add("AppIdForPublic", "wx5d2c11512a9a821d");
            configDictionary.Add("AppSecretForublic", "b4d51d30ecb118b085d56322b685ae34");

            configDictionary.Add("AppIdForDisease", "wx5d2c11512a9a821d");
            configDictionary.Add("AppSecretForDisease", "b4d51d30ecb118b085d56322b685ae34");
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="configName"></param>
        public static string GetConfigByName(string configName)
        {
            return configDictionary[configName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConfig(string name)
        {
            return MyConfigurationManager.AppSetting[name];
        }

        public enum ErrorCode
        {
            /// <summary>
            /// 默认未出错
            /// </summary>
            [Description("默认未出错")]
            NoFail = 0,

            /// <summary>
            ///     授权认证失败
            /// </summary>
            [Description("授权认证失败")]
            AuthorFail = 40001,

            /// <summary>
            ///     系统异常
            /// </summary>
            [Description("自动捕获的系统异常")]
            AutoCatchError = 40002,

            /// <summary>
            ///     普通错误
            /// </summary>
            [Description("普通业务错误")]
            CommonError = 40003,

            /// <summary>
            /// 获取用户信息错误
            /// </summary>
            [Description("根据Token获取用户信息错误")]
            UserInfoGetError = 40004,
            
            /// <summary>
            ///     授权认证失败
            /// </summary>
            [Description("App版本异常")]
            AppVersonError = 40005
        }




        //private static Autofac.IContainer Container { get; set; }



        #region 日志记录

        private static readonly object Ctx = new object(); //日志加锁

        //日志同步锁

        private static string _lastMessage = "";
        private static DateTime _lastMessageTime = DateTime.Now;

        public static void AddLog(string loginfo)
        {
            lock (Ctx)
            {
                try
                {
                    LogInfo ex = new LogInfo();
                    ex.Describle = loginfo;
                    CreateLog(ex);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void AddLog(LogInfo ex)
        {
            lock (Ctx)
            {
                try
                {
                    CreateLog(ex);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }



        /// <summary>
        ///     生成系统日志。
        /// </summary>
        public static void CreateLog(LogInfo logInfo)
        {
            string exceptionString = "";




            try
            {
                if (logInfo.OperaterName == "")
                {
                    //logInfo.operaterName = HttpUtility.UrlDecode(CommonHelper.OperatorName);
                }

                if (logInfo.OperaterNo == "")
                {
                    //logInfo.operaterNo = CommonHelper.StaffNo;
                }
            }
            catch (Exception)
            {
            }
            string ipAdress = logInfo.Ip;
            string userName = "";
            string moduleName = "";
            userName = logInfo.OperaterName;
            moduleName = logInfo.ModuleName;
            IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());
            //if(ipe.AddressList.l)
            //string localIp = ipe.AddressList[1].ToString();
            string localIp = "";
            var sb = new StringBuilder();
            sb.Append("[SYSDATE]: " + DateTime.Now);
            sb.Append("\r\n[LOGTYPE]: ERROR ");
            //sb.Append("\r\n[USERINFO]:");
            //sb.Append("\r\n   [IP]: " + (ipAdress == "" ? CommonHelper.Ip : ipAdress));
            sb.Append("\r\n   [USERNAME]: " + logInfo.OperaterName);
            sb.Append("\r\n   [USERNO]: " + logInfo.OperaterNo);
            sb.Append("\r\n[LOGINFO]:");
            sb.Append("\r\n   [DESCRIBLE]: " + logInfo.Describle);
            sb.Append("\r\n   [MODUELNAME]: " + logInfo.ModuleName);

            sb.Append("\r\n   [URL]: " + logInfo.RequestUrl);
            //sb.Append("\r\n   [SERVICEIP]: " + localIp);

            //打印外部异常
            sb.Append("\r\n   [MESSAGE]: " + logInfo.Exception.Message);
            sb.Append("\r\n   [TARGETSITE]: " + logInfo.Exception.TargetSite);
            sb.Append("\r\n   [STACKTRACE]: ");
            sb.Append("\r\n" + logInfo.Exception.StackTrace);
            exceptionString = logInfo.Exception.Message;

            //打印内部异常
            if (logInfo.Exception.InnerException != null)
            {
                sb.Append("\r\n   [INN_MESSAGE]: " + logInfo.Exception.InnerException.Message);
                sb.Append("\r\n   [INN_TARGETSITE]: " + logInfo.Exception.InnerException.TargetSite);
                sb.Append("\r\n   [INN_STACKTRACE]: ");
                sb.Append("\r\n" + logInfo.Exception.InnerException.StackTrace);
                exceptionString += logInfo.Exception.InnerException.Message;
            }

            //打印内部异常
            if (logInfo.Exception.InnerException?.InnerException != null)
            {
                sb.Append("\r\n   [INN_INN_MESSAGE]: " + logInfo.Exception.InnerException.InnerException.Message);
                sb.Append("\r\n   [INN_INN_TARGETSITE]: " + logInfo.Exception.InnerException.InnerException.TargetSite);
                sb.Append("\r\n   [INN_INN_STACKTRACE]: ");
                sb.Append("\r\n" + logInfo.Exception.InnerException.InnerException.StackTrace);
                exceptionString += logInfo.Exception.InnerException.InnerException.Message;
            }

            //打印内部异常
            if (logInfo.Exception.InnerException?.InnerException?.InnerException != null)
            {
                sb.Append("\r\n   [INN_INN_INN_MESSAGE]: " + logInfo.Exception.InnerException.InnerException.InnerException.Message);
                sb.Append("\r\n   [INN_INN_INN_TARGETSITE]: " + logInfo.Exception.InnerException.InnerException.InnerException.TargetSite);
                sb.Append("\r\n   [INN_INN_INN_STACKTRACE]: ");
                sb.Append("\r\n" + logInfo.Exception.InnerException.InnerException.InnerException.StackTrace);
                exceptionString += logInfo.Exception.InnerException.InnerException.InnerException.Message;
            }



            sb.Append("\r\n   [PARAMETER]: ");
            sb.Append("\r\n" + logInfo.Parameter + " " + logInfo.RequestInfo);


            //正式服务器发邮件
            if (localIp == "10.251.36.217" || localIp == "10.251.142.17" || localIp == "10.251.16.8")
            {
                if (exceptionString.Length > 35)
                {
                    //5分钟内内容重复的异常不重发
                    if (exceptionString.Substring(0, 33) == _lastMessage && (DateTime.Now - _lastMessageTime).Minutes < 10)
                    {
                        return;
                    }
                    _lastMessage = exceptionString.Substring(0, 33);
                    _lastMessageTime = DateTime.Now;
                }

                if (exceptionString.Length > 10 && exceptionString.Length <= 35)
                {
                    //5分钟内内容重复的异常不重发
                    if (exceptionString.Substring(0, 9) == _lastMessage && (DateTime.Now - _lastMessageTime).Minutes < 10)
                    {
                        return;
                    }
                    _lastMessage = exceptionString.Substring(0, 9);
                    _lastMessageTime = DateTime.Now;
                }


                string fromWhere = "正式";
                if (localIp == "10.251.36.217")
                {
                    if (exceptionString.Contains("timeout"))
                    {
                        return;
                    }
                    fromWhere = "Demo";
                }
                //MailHelper.SendMailCrm("API异常-" + fromWhere + "-ExtService", sb.ToString(), "liwenhai@vancl.cn", false);
                //MailHelper.SendMailCrm("API异常-" + fromWhere + "-ExtService", sb.ToString(), "zhangwang@vancl.cn", false);
            }
            //非正式服务器写本地日志
            else
            {
                string FilePath = "";

                string strTmp = "";

                strTmp = DateTime.Now.Year + "年\\" + DateTime.Now.Month + "月\\" + DateTime.Now.Day + "日\\";
                FilePath += strTmp;
                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                } //   文件夹操作完成。   
                FilePath += "togo_log_";
                FilePath += DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day
                    /*+ "_" + DateTime.Now.Hour.ToString()*/+ ".txt";
                StreamWriter sw;
                if (!File.Exists(FilePath))
                {
                    sw = File.CreateText(FilePath);
                }
                else
                {
                    sw = File.AppendText(FilePath);
                }
                sw.WriteLine(
                    "-------------------------------------log start--------------------------------------------------");
                sb.Append(
                    "\r\n-------------------------------------log end---------------------------------------------------- ");
                sw.Write(sb.ToString());
                sw.WriteLine();
                sw.Close();
            }
        }



        #endregion

        public static string GetJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });
        }

        public static T GetJsonObject<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static object GetJsonObject(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static string GetBase64JsonString(object obj)
        {
            string str = GetJsonString(obj);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static CommonActionResult GetError(string errorMessage)
        {
            return new CommonActionResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static CommonActionResult GetSuccess(object result)
        {
            return new CommonActionResult
            {
                IsSuccess = true,
                Result = result
            };
        }

        public static ResultPakage<T> ReturnFailResult<T>(string message, ErrorCode errorCode = ErrorCode.CommonError)
        {
            return new ResultPakage<T> { IsSuccess = false, ErrorMessage = message, ErrorCode = errorCode };
            //return GetJsonString(new CommonActionResult {IsSuccess = false, ErrorMessage = message, ErrorCode = errorCode});
        }

        public static ResultPakage<T> ReturnOkResult<T>(T obj)
        {
            return new ResultPakage<T> { IsSuccess = true, Result = obj };
            //return GetJsonString(new CommonActionResult {IsSuccess = true, Result = obj});
        }


        public static HttpResponseMessage ReturnOrginInfo(string msg)
        {
            return new HttpResponseMessage { Content = new StringContent(msg, Encoding.GetEncoding("UTF-8"), "text/plain") };
        }

        public static HttpResponseMessage ReturnHtml(string msg)
        {
            return new HttpResponseMessage { Content = new StringContent(msg, Encoding.GetEncoding("UTF-8"), "text/html") };
        }

        /// <summary>
        ///     获取base64的短token
        /// </summary>
        /// <returns></returns>

        public static string GetNewToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=');
        }
        /// <summary>
        /// 字符串MD5加密
        /// <remarks>
        /// </remarks>
        /// </summary>
        /// <param name="source">目标字符串</param>
        /// <returns>md5结果字符串</returns>
        public static string ToMd5(this string source)
        {
            try
            {
                MD5 getmd5 = new MD5CryptoServiceProvider();
                byte[] targetStr = getmd5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(source));
                string result = BitConverter.ToString(targetStr).Replace("-", "");
                return result;
            }
            catch (Exception)
            {
                return "0";
            }
        }

        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Encrypted text</returns>
        public static string PasswordEncrypt(string source)
        {
            return ToMd5(source);
        }

        ///// <summary>
        ///// 根据文本生成二维码
        ///// </summary>
        ///// <param name="source"></param>
        ///// <returns>Encrypted text</returns>
        //public static string CreateQrCodeByString(string content)
        //{
        //    var encoder = new QrEncoder();
        //    QrCode qrCode;
        //    encoder.TryEncode(content, out qrCode);

        //    var gRenderer = new GraphicsRenderer(
        //        new FixedModuleSize(2, QuietZoneModules.Two),
        //        Brushes.Black, Brushes.White);
        //    var base64Pic = "";
        //    using (var ms = new MemoryStream())
        //    {
        //        gRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);

        //        var b = ms.GetBuffer();
        //        base64Pic = Convert.ToBase64String(b);
        //    }
        //    return base64Pic;
        //}

        /// <summary>
        /// 集合对集合
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        //public static List<TResult> MapTo<TResult>(IEnumerable obj)
        //{
        //    if (obj == null)
        //        throw new ArgumentNullException();
        //    Mapper.Map(obj.GetType(), typeof(TResult));
        //    return (List<TResult>)Mapper.Map(obj, obj.GetType(), typeof(List<TResult>));
        //}


        /// <summary>
        /// 对象对对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        //public static TResult MapTo<TResult>(object obj)
        //{
        //    if (obj == null)
        //        throw new ArgumentNullException();
        //    Mapper.Map(obj.GetType(), typeof(TResult));
        //    return (TResult)Mapper.Map(obj, obj.GetType(), typeof(TResult));
        //}

        /// <summary>
        /// 对象对对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IMapper GetDynamicMap()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMissingTypeMaps = true);
            return config.CreateMapper();
        }



        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="configName"></param>
        //public static string GetConfigByName(string configName)
        //{
        //    return ConfigurationManager.AppSettings[configName];
        //}

        #region String->Image

        /// <summary>
        /// 图片->base64
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        //public static string ImageToString(string strPath)
        //{
        //    try
        //    {
        //        var ret = string.Empty;
        //        using (MemoryStream m = new MemoryStream())
        //        {
        //            Bitmap bp = new Bitmap(strPath);
        //            bp.Save(m, ImageFormat.Png);
        //            byte[] b = m.GetBuffer();
        //            ret = Convert.ToBase64String(b);
        //        }
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        var err = "Base64StringToImage 转换失败\nException：" + ex.Message;
        //        throw new BusinessException(err);

        //    }
        //}

        /// <summary>
        /// 获取两个坐标点之间的距离
        /// </summary>
        /// <param name="startLongitude">起点经度</param>
        /// <param name="startLatitude">起点维度</param>
        /// <param name="endLongitude">终点经度</param>
        /// <param name="endLatitude">终点维度</param>
        /// <returns></returns>
        //public static double GetDistanceFromTwoPoints(double startLongitude, double startLatitude, double endLongitude, double endLatitude)
        //{
        //    var startPoint = new PointShape(startLongitude, startLatitude);
        //    var endPoint = new PointShape(endLongitude, endLatitude);
        //    var distance = startPoint.GetDistanceTo(endPoint, GeographyUnit.DecimalDegree, DistanceUnit.Meter);
        //    return distance;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        //public static Tuple<Bitmap, string> StringToImage(string str, string savePath = null)
        //{
        //    Bitmap img = null;
        //    if (string.IsNullOrEmpty(str))
        //    {
        //        return null;
        //    }
        //    var filePath = string.Empty;
        //    var folder = Util.GetConfigByName("UploadPath");
        //    var basePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + folder;
        //    byte[] bt = Convert.FromBase64String(str);
        //    using (var stream = new MemoryStream(bt))
        //    {
        //        img = new Bitmap(stream);
        //    }
        //    if (!string.IsNullOrEmpty(savePath))
        //    {
        //        filePath = basePath + savePath;
        //        var dir = Path.GetDirectoryName(filePath);
        //        if (!Directory.Exists(dir))
        //        {
        //            Directory.CreateDirectory(dir);
        //        }
        //        img.Save(filePath);
        //    }
        //    var ret = Tuple.Create(img, savePath);
        //    return ret;

        //}

        //public static string SaveImage(string imagePath,HttpContext context) {
        //    HttpPostedFile file = context.Request.Files.Count > 0 ? context.Request.Files[0] : null;
        //    if (file == null) {
        //        return null;
        //    }
        //    string uploadPath = context.Server.MapPath(GetConfigByName(imagePath) + "\\");
        //    string randomNumber = Guid.NewGuid().ToString();
        //    var fileName = randomNumber + file.FileName;
        //    //判断上传的文件是否为空
        //    if (!Directory.Exists(uploadPath))
        //    {
        //        Directory.CreateDirectory(uploadPath);
        //    }
        //    //保存文件
        //    file.SaveAs(uploadPath + fileName);
        //    return fileName;
        //}


        private static readonly HashSet<char> base64Characters = new HashSet<char>()
        {
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z',
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            '+',
            '/',
            '='
        };

        /// <summary>
        /// 验证字符串是否为base64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsBase64String(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else if (value.Any(c => !base64Characters.Contains(c)))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }


        public static ICacheHelper GetCacheClient()
        {
            ICacheHelper client = new RedisCacheHelper();
            return client;
        }

        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string Description(this Enum en)
        {
            //Type type = en.GetType();
            //MemberInfo[] memInfo = type.GetMember(en.ToString());
            //if (memInfo.Length > 0)
            //{
            //    object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            //    if (attrs.Length > 0)
            //        return ((DescriptionAttribute)attrs[0]).Description;
            //}
            //return en.ToString();

            return Enum.GetName(en.GetType(), en);
        }

        /// <summary>
        /// 获取请求线程内的唯一缓存对象
        /// </summary>
        //public static T GetFromCallContext<T>(ContextKey key)
        //{
        //    return (T)CallContext.GetData(key.Description());
        //}

        ///// <summary>
        ///// 设置对象到请求唯一缓存
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="theObject"></param>
        //public static void SetToCallContext(ContextKey key, object theObject)
        //{
        //    CallContext.SetData(key.Description(), theObject);
        //}


        #endregion
        /// <summary>
        /// 时间比较
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static TimeSpan CompareTimeSpan(DateTime t1, DateTime t2)
        {
            TimeSpan ts = t1 - t2;
            return ts;
        }

        /// <summary>
        /// 转换为unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtNow = DateTime.Parse(dateTime.ToString(CultureInfo.InvariantCulture));
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return timeStamp;
        }

        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <param name="en">枚举</param>
        /// <returns></returns>
        public static string GetEnumDes(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }



        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <param name="en">枚举</param>
        /// <returns></returns>
        public static string GetEnumName(this Enum en)
        {
            Type type = en.GetType();
            return Enum.GetName(type, en);
        }

        /// <summary>
        /// 字符串转换为枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ParseStringToEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value);
        }

        public  static void SetObjectValue(object obj, string fieldname, object value)
        {
            obj.GetType().GetProperty(fieldname).SetValue(obj, value);
        }

        public static int GetAgeByDate(DateTime birthdate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthdate.Year;
            if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
            {
                age--;
            }
            return age < 0 ? 0 : age;
        }

        /// <summary>
        /// 创建数据库日志
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        //public static StreamWriter CreateDbLog(DbContext db)
        //{
        //    var sw = new StreamWriter(@"e:\log.txt") { AutoFlush = true };

        //    db.Database.Log = s => {
        //        sw.Write(s);
        //    };
        //    return sw;
        //}

        /// <summary>
        /// 获取客户端IP地址（无视代理）
        /// </summary>
        /// <returns>若失败则返回回送地址</returns>
        //public static string GetHostAddress()
        //{
        //    try
        //    {
        //        string userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //        if (string.IsNullOrEmpty(userHostAddress))
        //        {
        //            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
        //                userHostAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
        //        }
        //        if (string.IsNullOrEmpty(userHostAddress))
        //        {
        //            userHostAddress = HttpContext.Current.Request.UserHostAddress;
        //        }

        //        //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
        //        if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
        //        {
        //            return userHostAddress;
        //        }
        //        return "127.0.0.1";
        //    }
        //    catch (Exception)
        //    {

        //        return "";
        //    }
            
        //}

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 根据Ip地址获取所在的城市位置
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        //public static string GetAddressByIp(string ip)
        //{
        //    try
        //    {
        //        var openId = "";
        //        var http = new Common.HttpHelper();
        //        var item = HttpHelper.GetHttpItem();
        //        item.URL = "http://opendata.baidu.com/api.php?query=" + ip + "&co=&resource_id=6006&t=1412300361645&ie=utf8&oe=gbk&cb=op_aladdin_callback&format=json&tn=baidu&cb=jQuery1102026811896078288555_1412299994977&_=1412299994981";
        //        item.Method = "get";
        //        var result = http.GetHtml(item).Html;
        //        var stringresult = result.Split('[');
        //        if (stringresult.Length > 1)
        //        {
        //            var cc = "[" + stringresult[1].Replace("});", "");
        //            return JArray.Parse(cc)[0]["location"].Value<string>();
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return "";
        //    }
        //}

        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();


            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public static PagedResult<U> GetPaged<T, U>(this IQueryable<T> query,
            int page, int pageSize) where U : class
        {
            var result = new PagedResult<U>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip)
                .Take(pageSize)
                .ProjectTo<U>()
                .ToList();

            return result;
        }

    }

    #region 通用实体
    public class LogInfo
    {
        public LogInfo()
        {
            Exception = new Exception("");
            Ip = "";
            OperaterName = "";
            OperaterNo = "";
            ModuleName = "";
            Describle = "";
            Parameter = "";
        }

        public LogInfo(Exception e)
        {
            Exception = e;
            Ip = "";
            OperaterName = "";
            OperaterNo = "";
            ModuleName = "";
            Describle = "";
            Parameter = "";
        }

        public string Ip { set; get; }
        public string OperaterName { set; get; }
        public string OperaterNo { set; get; }
        public string ModuleName { set; get; }
        public string Describle { set; get; }
        public string Parameter { set; get; }
        public Exception Exception { set; get; }
        public string RequestInfo { set; get; }
        public string RequestUrl { set; get; }
    }

    public class ActionInfo
    {
        public ActionInfo(int processId, string actionType, string controllerName,
            string actionName, string httpMethod, string paramters, int contentLength)
        {
            ProcessId = processId;
            ActionType = actionType;
            ControllerName = controllerName;
            ActionName = actionName;
            HttpMethod = httpMethod;
            Parameters = paramters;
            ContentLength = contentLength;

            InstanceName = DetermineRawInstanceName();
            //SanitizedInstanceName =
            //  InstanceNameRegistry.GetSanitizedInstanceName(InstanceName);
        }


        /// <summary>
        ///     新能计数分类名称
        /// </summary>
        public string PerformaneCounterCategory { get; private set; }


        /// <summary>
        ///     当前进程ID
        /// </summary>
        public int ProcessId { get; }

        /// <summary>
        ///     anction类型是mvc还是webapi
        /// </summary>
        public string ActionType { get; }

        /// <summary>
        ///     控制器名称
        /// </summary>
        public string ControllerName { get; }

        /// <summary>
        ///     Gets/sets the name of this action
        /// </summary>
        public string ActionName { get; }

        /// <summary>
        ///     http方法
        /// </summary>
        public string HttpMethod { get; }


        /// <summary>
        ///     action参数
        /// </summary>
        /// <remarks></remarks>
        public string Parameters { get; }


        /// <summary>
        /// </summary>
        public string InstanceName { get; }


        /// <summary>
        /// </summary>
        public string SanitizedInstanceName { get; set; }

        /// <summary>
        ///     内容长度
        /// </summary>
        public int ContentLength { get; set; }

        /// <summary>
        /// 调用开始时间
        /// </summary>
        public DateTime CallStartTime { get; set; }

        /// <summary>
        /// 调用结束时间
        /// </summary>
        public DateTime CallEndTime { get; set; }

        /// <summary>
        /// 输出内容
        /// </summary>
        public string OutPutContent { get; set; }

        /// <summary>
        /// 输出内容
        /// </summary>
        public string RequestContent { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 调用总耗时
        /// </summary>
        public double ElapsedTime { get; set; }


        /// <summary>
        ///     测量原生实例信息
        /// </summary>
        /// <returns></returns>
        private string DetermineRawInstanceName()
        {
            var rawInstanceName = string.Format("[{0}]-{1} {2}.{3}[{5}] {4}",
                ProcessId,
                ActionType,
                ControllerName,
                ActionName,
                HttpMethod,
                Parameters);
            return rawInstanceName;
        }

        #region Utility Methods

        public override bool Equals(object obj)
        {
            var other = obj as ActionInfo;
            if (obj == null)
                return false;

            return InstanceName.Equals(other.InstanceName);
        }

        public override int GetHashCode()
        {
            return InstanceName.GetHashCode();
        }

        #endregion
    }

    public static class DistinctByClass
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }

    public class CommonActionResult
    {
        public bool IsSuccess
        { get; set; }

        public string ErrorMessage
        { get; set; }

        public Util.ErrorCode ErrorCode
        { get; set; }

        public object Result
        { get; set; }
    }

    /// <summary>
    /// 返回结果集数据包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultPakage<T>
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess
        { get; set; }

        /// <summary>
        /// 错误描述,出错时此字段有值
        /// </summary>
        public string ErrorMessage
        { get; set; }

        /// <summary>
        /// 服务器当前时间
        /// </summary>
        public string DateTimeNow = DateTime.Now.GetDateTimeFormats('s')[0];

        /// <summary>
        /// 错误代码
        /// </summary>

        public Util.ErrorCode ErrorCode
        { get; set; }

        /// <summary>
        /// 泛型返回类型集合
        /// </summary>
        public T Result
        { get; set; }

        /// <summary>
        /// 返回记录数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 返回记录总数
        /// </summary>
        public int TotalCount { get; set; }
    } 
    #endregion
}