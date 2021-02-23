using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HISDouble
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //string text = "Data Source=192.168.1.85;Initial Catalog=ECAN_ECFP_ZJSRMYY;Persist Security Info=True;User ID=sa;Password=Zj123456;";
            //string result = DESCryptoService.DESEncrypt(text, "Core_H_N");
            //Console.WriteLine(result);
            //string jmresult = DESCryptoService.DESDecrypt(result, "Core_H_N");
            //Console.WriteLine(jmresult);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:7500");
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
