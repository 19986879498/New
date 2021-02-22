using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using HISDouble.DapperConfig;
using HISDouble.Factory;
using HISDouble.Interfaces;
using HISDouble.Jwt;
using HISDouble.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Oracle.ManagedDataAccess.Client;
using ServiceReference1;
using ServiceReference2;
using ServiceReference3;

namespace HISDouble
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.services.AddHttpClient();
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Latest).AddNewtonsoftJson();
            #region Jwt加密
            //注册jwt
            services.Configure<JWTConfig>(Configuration.GetSection("JWTConfig"));
           
            services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
            AddJwtBearer();
            #endregion
            #region 加载dapper配置
            DapperConfig.ColumnMapper.SetMapper();
            #endregion
            #region 跨域

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("any", builder =>
            //    {
            //        builder.WithMethods("GET", "POST", "HEAD", "PUT", "DELETE")
            //        //.AllowCredentials()
            //    .AllowAnyOrigin(); 
            //    });
            //});
            ////跨域
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("cors",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod().SetIsOriginAllowed(_ => true)
            //    );
            //});
            services.AddCors(options =>
            {
                options.AddPolicy("cors", policy =>
                {
                    // 跨域请求
                    policy.WithOrigins("http://192.168.2.68:8888", "http://192.168.188.129:8080", "http://192.168.188.119:8080", "http://localhost:8080", "http://127.0.0.1:8080", "http://127.0.0.1:8020", "http://localhost:8020", "http://120.202.68.96:8888", "http://192.168.1.100:8080","http://0.0.0.0:8080", "http://172.22.158.2:9002")//
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();

                });
                //
                //options.AddPolicy("all", builder =>
                //{
                //    builder.WithOrigins("*")
                //    .AllowAnyMethod()
                //    .AllowAnyHeader();
                //});
            });
            #endregion

            #region 配置swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HISDouble", Version = "v1" });
            });
            #endregion
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //services.AddHttpClient();

        }

        /// <summary>
        /// Autofacr容器  支持AOP
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<OracleConnection>().As<IDbConnection>();
            builder.RegisterType<DapperContext>().As<IDapperContext>().As<IDapperContext>().UsingConstructor(typeof(IDbConnection));
            builder.RegisterType<AnnexService>().As<IAnnexService>();
            builder.RegisterType<HospitaldicService>().As<IHospitaldicService>();
            builder.RegisterType<ReferralService>().As<IReferralService>();
            builder.RegisterType<View_BedsService>().As<IView_BedsService>();
            builder.RegisterType<PatientsInfoService>().As<IPatientsInfoService>();
            builder.RegisterType<DiagnosisService>().As<IDiagnosisService>();
            builder.RegisterType<DiagnosisdicService>().As<IDiagnosisdicService>();
            builder.RegisterType<MedicaldicService>().As<IMedicaldicService>();
            builder.RegisterType<OrderService>().As<IOrderService>();
            builder.RegisterType<UserinfoService>().As<IUserinfoService>();
            builder.RegisterType<View_BedsInfoService>().As<IView_BedsInfoService>();
            builder.RegisterType<View_contractunitService>().As<IView_contractunitService>();
            builder.RegisterType<View_DiagnosisService>().As<IView_DiagnosisService>();
            builder.RegisterType<View_GetCheckindataService>().As<IView_GetCheckindataService>();
            builder.RegisterType<View_OrderService>().As<IView_OrderService>();
            builder.RegisterType<View_PatientsInfoService>().As<IView_PatientsInfoService>();
            builder.RegisterType<View_PreRegisterService>().As<IView_PreRegisterService>();
            builder.RegisterType<TokenHelper>().As<ITokenHelper>();
            builder.RegisterType<BaseServices>().As<IBaseService>();
            builder.RegisterType<OutpatientService>().As<IOutpatientService>();
            builder.RegisterType<View_PreInspectionService>().As<IView_PreInspectionService>();
            builder.RegisterType<View_PacsService>().As<IView_PacsService>();
            builder.RegisterType<CheckInDataListService>().As<ICheckInDataListService>();
            builder.RegisterType<View_QueryAduitService>().As<IView_QueryAuditService>();
            builder.RegisterType<View_QueryByAduitService>().As<IView_QueryByAuditService>();
            builder.RegisterType<View_QueryAllService>().As<IView_QueryAllService>();
            builder.RegisterType<WorkflowServicePortTypeClient>().As<WorkflowServicePortType>();
            builder.RegisterType<HrmServicePortTypeClient>().As<HrmServicePortType>();
            builder.RegisterType<SqlServiceSoapClient>().As<SqlServiceSoap>();
            builder.Register(c => new SqlServiceSoapClient(SqlServiceSoapClient.EndpointConfiguration.SqlServiceSoap)).As<SqlServiceSoap>();
            //builder.RegisterType<OracleFactory>().As<IOracleFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HISDouble v1"));
            // app.UseHttpsRedirection();
          
            app.UseRouting();
            app.UseCors("cors");
            //app.UseCors("all");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors("cors");
            });
        }
    }
}
