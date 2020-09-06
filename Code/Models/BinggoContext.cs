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

        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Collectionactivity> Collectionactivity { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Distribution> Distribution { get; set; }
        public virtual DbSet<Exception> Exception { get; set; }
        public virtual DbSet<Messageinfo> Messageinfo { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<Reply> Reply { get; set; }
        public virtual DbSet<Sku> Sku { get; set; }
        public virtual DbSet<Town> Town { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Useractivity> Useractivity { get; set; }
        public virtual DbSet<Userfollow> Userfollow { get; set; }
        public virtual DbSet<Userthumbsup> Userthumbsup { get; set; }

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
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("activity");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.HasIndex(e => e.Publisher)
                    .HasName("FK_activity_user_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActivityType)
                    .HasColumnName("activityType")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Area)
                    .HasColumnName("area")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AreaCode)
                    .HasColumnName("areaCode")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Auth)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BeginningTime).HasColumnType("datetime");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CityCode)
                    .HasColumnName("cityCode")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CommentNum)
                    .HasColumnName("commentNum")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Details).HasColumnType("mediumtext");

                entity.Property(e => e.EndingTime).HasColumnType("datetime");

                entity.Property(e => e.Fee)
                    .HasColumnName("fee")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FeeText)
                    .HasColumnName("feeText")
                    .HasColumnType("varchar(5000)");

                entity.Property(e => e.Image).HasColumnType("varchar(500)");

                entity.Property(e => e.InsureType)
                    .HasColumnName("insureType")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.InsureTypeName)
                    .HasColumnName("insureTypeName")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("isDelete")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.IsForbidCacelSign)
                    .HasColumnName("isForbidCacelSign")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.IsNeedInsurance)
                    .HasColumnName("isNeedInsurance")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.IsNeedNameAndId)
                    .HasColumnName("isNeedNameAndID")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.IsNeedPhone)
                    .HasColumnName("isNeedPhone")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.IsNeedSignOut)
                    .HasColumnName("isNeedSignOut")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.IsOpenSignList)
                    .HasColumnName("isOpenSignList")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.IsShowMainPage)
                    .HasColumnName("isShowMainPage")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.LimitOfPeople)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Location).HasColumnType("varchar(500)");

                entity.Property(e => e.ParticipateNum)
                    .HasColumnName("participateNum")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Province)
                    .HasColumnName("province")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Publisher)
                    .HasColumnName("publisher")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.RangOfAge).HasColumnType("varchar(50)");

                entity.Property(e => e.SignEndTime)
                    .HasColumnName("signEndTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tag).HasColumnType("varchar(500)");

                entity.Property(e => e.Telphone)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.TotalThumbsUp)
                    .HasColumnName("totalThumbsUp")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Town)
                    .HasColumnName("town")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.TownCode)
                    .HasColumnName("townCode")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Type)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.VerifyPeople)
                    .HasColumnName("verifyPeople")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.PublisherNavigation)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.Publisher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_activity_user_Id");
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("area");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.AreaCode)
                    .HasColumnName("areaCode")
                    .HasColumnType("varchar(12)");

                entity.Property(e => e.CityCode)
                    .HasColumnName("cityCode")
                    .HasColumnType("varchar(12)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("chat");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.FromUser)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsRead).HasColumnType("bit(1)");

                entity.Property(e => e.Message).HasColumnType("varchar(500)");

                entity.Property(e => e.ReadTime).HasColumnType("datetime");

                entity.Property(e => e.ToUser)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("city");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CityCode)
                    .HasColumnName("cityCode")
                    .HasColumnType("varchar(12)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasColumnType("varchar(12)");
            });

            modelBuilder.Entity<Collectionactivity>(entity =>
            {
                entity.ToTable("collectionactivity");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("FK_collectionactivity_activity_Id");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("activityId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Collectionactivity)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_collectionactivity_activity_Id");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("activityId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FromUserId)
                    .HasColumnName("fromUserId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsAuthor)
                    .HasColumnName("isAuthor")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.Msg)
                    .HasColumnName("msg")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<Distribution>(entity =>
            {
                entity.ToTable("distribution");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("FK_distribution_activity_Id");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActivityId).HasColumnType("int(11)");

                entity.Property(e => e.Amount).HasColumnType("decimal(8,2)");

                entity.Property(e => e.IsShowInMainPage).HasColumnType("bit(1)");

                entity.Property(e => e.Phone).HasColumnType("varchar(50)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Distribution)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_distribution_activity_Id");
            });

            modelBuilder.Entity<Exception>(entity =>
            {
                entity.ToTable("exception");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Detail)
                    .IsRequired()
                    .HasColumnType("varchar(10000)");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("varchar(5000)");
            });

            modelBuilder.Entity<Messageinfo>(entity =>
            {
                entity.ToTable("messageinfo");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BusinessId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsRead)
                    .HasColumnName("isRead")
                    .HasColumnType("bit(1)");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.ReadTime)
                    .HasColumnName("readTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.ToUserId)
                    .HasColumnName("toUserId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TypeName)
                    .HasColumnName("typeName")
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("province");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasColumnType("varchar(12)");
            });

            modelBuilder.Entity<Reply>(entity =>
            {
                entity.ToTable("reply");

                entity.HasIndex(e => e.CommentId)
                    .HasName("FK_reply_comment_id");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CommentId)
                    .HasColumnName("commentId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FromUserId)
                    .HasColumnName("fromUserId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Msg)
                    .HasColumnName("msg")
                    .HasColumnType("varchar(500)");

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.Reply)
                    .HasForeignKey(d => d.CommentId)
                    .HasConstraintName("FK_reply_comment_id");
            });

            modelBuilder.Entity<Sku>(entity =>
            {
                entity.ToTable("sku");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("FK_sku_activity_Id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("activityId")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatePerson)
                    .HasColumnName("createPerson")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.IsDelete).HasColumnType("bit(1)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(19,2)");

                entity.Property(e => e.Stock)
                    .HasColumnName("stock")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatePerson)
                    .HasColumnName("updatePerson")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Sku)
                    .HasForeignKey(d => d.ActivityId)
                    .HasConstraintName("FK_sku_activity_Id");
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.ToTable("town");

                entity.Property(e => e.Id)
                    .HasColumnName("_id")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CountryId)
                    .HasColumnName("country_id")
                    .HasColumnType("varchar(12)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.TownId)
                    .HasColumnName("town_id")
                    .HasColumnType("varchar(12)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.HasIndex(e => e.Phone)
                    .HasName("Phone")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AgeFavor)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.AuthenticationStatus).HasColumnType("int(11)");

                entity.Property(e => e.Avatar).HasColumnType("varchar(500)");

                entity.Property(e => e.BusinessLicenseUrl).HasColumnType("varchar(2550)");

                entity.Property(e => e.Class)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.DisFavor)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.FirstLetter).HasColumnType("varchar(100)");

                entity.Property(e => e.Idcard)
                    .HasColumnName("IDCard")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.IdcardOtherSideUrl)
                    .HasColumnName("IDCardOtherSideUrl")
                    .HasColumnType("varchar(2550)");

                entity.Property(e => e.IdcardPositiveUrl)
                    .HasColumnName("IDCardPositiveUrl")
                    .HasColumnType("varchar(2550)");

                entity.Property(e => e.LegalPerson).HasColumnType("varchar(255)");

                entity.Property(e => e.Name).HasColumnType("varchar(50)");

                entity.Property(e => e.Nickname).HasColumnType("varchar(500)");

                entity.Property(e => e.OpenId).HasColumnType("varchar(255)");

                entity.Property(e => e.Password).HasColumnType("varchar(255)");

                entity.Property(e => e.Phone).HasColumnType("varchar(50)");

                entity.Property(e => e.Signature).HasColumnType("varchar(50)");

                entity.Property(e => e.TypeFavor).HasColumnType("varchar(500)");

                entity.Property(e => e.UserType).HasColumnType("int(11)");

                entity.Property(e => e.Wxid).HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Useractivity>(entity =>
            {
                entity.ToTable("useractivity");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("FK_useractivity");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.HasIndex(e => e.UserId)
                    .HasName("FK_useractivity_user_Id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("activityId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Useractivity)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_useractivity");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Useractivity)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_useractivity_user_Id");
            });

            modelBuilder.Entity<Userfollow>(entity =>
            {
                entity.ToTable("userfollow");

                entity.HasIndex(e => e.FollowUserId)
                    .HasName("FK_userfollow_user_Id");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FollowUserId)
                    .HasColumnName("followUserId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.FollowUser)
                    .WithMany(p => p.Userfollow)
                    .HasForeignKey(d => d.FollowUserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_userfollow_user_Id");
            });

            modelBuilder.Entity<Userthumbsup>(entity =>
            {
                entity.ToTable("userthumbsup");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("FK_userthumbsup_activity_Id");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_AUTOFIELD");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("activityId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Userthumbsup)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_userthumbsup_activity_Id");
            });
        }
    }
}
