using AutoMapper;
using Bingo.Common;
using Bingo.Common.Fileter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.IO;


namespace Bingo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AutoMapperHelper.Init();

            Util.Init();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddControllersWithViews();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews().AddNewtonsoftJson(); ;

            //services.AddEntityFrameworkMySql().
            //    AddDbContext<OrderContext>(options =>
            //        options.UseMySQL(Configuration["ConnectionStrings:SqlConnection"]), ServiceLifetime.Scoped);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddControllersWithViews().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddControllersWithViews(opt =>
            {
                opt.Filters.Add(typeof(ValidatorActionFilter));
                //opt.Filters.Add<ApiResponseFilterAttribute>();
                //opt.Filters.Add<ApiExceptionFilterAttribute>();
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.AddControllers().AddNewtonsoftJson();
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages().AddNewtonsoftJson();


           

            //添加对AutoMapper的支持
            services.AddAutoMapper();
            //注册Swagger生成器，定义一个和多个Swagger 文档

            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NPC业务接口", Version = "v1" });
                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "Bingo.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseErrorHandling();

           
            app.UseStaticHttpContext();

            // 启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "pic")),
                //RequestPath = "/" //重写了一个虚拟路径。
            });
            //app.UseSpaStaticFiles();
            //app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthorization();

            app.UseCors();

            // 端点中间件，对端点进行注册
            app.UseEndpoints(endpoints =>
{
                endpoints.MapGet(pattern: "/", requestDelegate: async context =>
                {
                    await context.Response.WriteAsync(text: "Hello World!");
                });
                // MVC
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllers();
            });

            //
        }
    }
}
