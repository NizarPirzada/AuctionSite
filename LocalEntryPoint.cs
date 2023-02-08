using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AuctionSite
{
    /// <summary>
    /// The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
    /// </summary>
    public class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    //options.Listen(IPAddress.Any, 443, listenOptions =>
                    //{
                    //    listenOptions.UseHttps(
                    //        File.Exists(@"C:\Users\andre\Desktop\auctionsite.pfx") ?
                    //            @"C:\Users\andre\Desktop\auctionsite.pfx" :
                    //            @"C:\Users\Administrator\Desktop\auctionsite.pfx"
                    //        , "");
                    //});
                })
                .UseUrls("http://0.0.0.0:443")
                .UseStartup<Startup>()
                .Build();
    }
}
