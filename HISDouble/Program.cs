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
            string txt = "";
            string desresult = DESCryptoService.DESEncrypt(txt, "Core_H_N");
            string jmres = DESCryptoService.DESDecrypt(desresult, "Core_H_N");
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:7500");
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
