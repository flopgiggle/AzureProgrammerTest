using System;

namespace Bingo.Models
{
    public class BaseModel
    {
        /// <summary>
        /// 不传，实操中应该在界面使用dto进行转换，到数据库模型不需要暴露此信息
        /// </summary>
        public long Id { get; set; }

        //public DateTime CreateTime { get; set; }

        //public DateTime UpdateTime { get; set; }

        //还可以有很多操作数据
    }
}
