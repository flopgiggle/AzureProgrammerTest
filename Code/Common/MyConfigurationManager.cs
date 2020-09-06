using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bingo.Common
{
    /// <summary>
    /// 获取自定义的 json 配置文件
    /// </summary>
    static class MyConfigurationManager

    {

        public static IConfiguration AppSetting { get; }



        static MyConfigurationManager()
        {
            // 注意：2.2版本的这个路径不对 会输出 xxx/IIS Express...类似这种路径，
            // 等3.0再看有没其他变化
            string directory = Directory.GetCurrentDirectory();

            AppSetting = new ConfigurationBuilder()
                     .SetBasePath(directory)
                     .AddJsonFile("appsettings.json")        
                     .Build();
        }
    }
}
