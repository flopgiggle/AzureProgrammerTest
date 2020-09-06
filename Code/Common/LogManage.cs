//using System;
//using System.Collections.Generic;
//using Bingo.Dto;
//using Bingo.Models;
//using KidneyCareApi.Common;

//namespace Bingo.Common
//{
//    public class LogManage
//    {
//        static readonly ICacheHelper CacheHelper = new RedisCacheHelper();
        
//        public bool CreateLog(OperationLogDto log)
//        {
//            Db db = new Db();
//            var newlog = log.MapTo<OperationLog>();
//            if (newlog.CreatePerson != -1) {
//                newlog.CreatePerson = SSOManager.GetUser().Id;
//            }
//            newlog.CreateTime = DateTime.Now;
//            db.OperationLogs.Add(newlog);
//            db.SaveChanges();
//            return true;
//        }

//        public ResultPakage<List<OperationLogDto>> GetLogList(QueryLogCondition condtion)
//        {
//            Db db = new Db();
//            var dmapper = Util.GetDynamicMap();
//            var logtype = (int)Enum.Parse(typeof(BusinessEnum.LogType), condtion.Type);
//            //全部记录
//            var totalResult = db.OperationLogs.Where(a => a.LogType == logtype &&a.BusinessKey == condtion.BusinessKey);

//            var joinReuslt = totalResult.Join(db.Users, a => a.CreatePerson, a => a.Id, (a, b) =>new 
//            {
//                a.Id,
//                a.LogType,
//                a.Data,
//                a.Summary,
//                a.Title,
//                CreatePersonName = b.UserName,
//                a.CreateTime,
//                a.CreatePerson
//            });

//            //按分页进行查询
//            var pageReuslt = joinReuslt.OrderByDescending(a=>a.CreateTime).Skip((condtion.CurrentPage - 1) * condtion.PageSize).Take(condtion.PageSize)
//                .ToList().Select(dmapper.Map<OperationLogDto>).ToList(); 

//            pageReuslt.ForEach(a =>
//            {
//                a.LogTypeName = ((BusinessEnum.LogType)a.LogType).GetEnumDes();
//            });

//            var returnResult = Util.ReturnOkResult(pageReuslt);
//            //设置总行数,用于前端展示
//            returnResult.PageCount = totalResult.Count();
//            return returnResult;
//        }

//        public class LogInfo
//        {
//            public BusinessEnum.LogType Type { set; get; }
//            public string Title { set; get; }
//            public string Content { set; get; }
//            public string ModuleName { set; get; }
//            public string Describle { set; get; }
//            public string Parameter { set; get; }
//            public Exception Exception { set; get; }
//            public string RequestInfo { set; get; }
//            public string RequestUrl { set; get; }
//        }

//        /// <summary>
//        /// 日志查询条件
//        /// </summary>
//        public class QueryLogCondition: PageDto
//        {
//            public string Type { set; get; }
//            public string BusinessKey { set; get; }
//        }

//    }
//}