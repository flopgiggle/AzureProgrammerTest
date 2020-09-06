using System;
using Bingo.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bingo.Models
{
    public partial class Db : DbContext
    {
        public Db()
        {
        }

        public Db(DbContextOptions<Db> options)
            : base(options)
        {
        }

        public virtual DbSet<Order> Order { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //配置文件读取连接字符串
                optionsBuilder.UseMySql(MyConfigurationManager.AppSetting["Db:Conn"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
