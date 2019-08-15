using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Eze.SchemaCompare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));


            var hostingPort = "5000";
            if (args.Contains("--port")) {
                var idx = args.ToList().IndexOf("--port");
                hostingPort = args[idx + 1];
                if (Int32.Parse(hostingPort) < 1024 || Int32.Parse(hostingPort) > 65535)
                {
                    Console.WriteLine("Invalid port. Please choose a port between 1024 and 65535");
                    throw new InvalidDataException();
                }
            }

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            var staticContentPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..\\client\\dist"));
            Console.WriteLine("Static content served from: " + staticContentPath);
            var host = new WebHostBuilder()
                .UseUrls("http://0.0.0.0:" + hostingPort)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseWebRoot(staticContentPath)
                .ConfigureKestrel((context, options) =>
                {
                    // Set properties and call methods on options
                })
                .Build();


            if (isService)
            {
                // To run the app without the CustomWebHostService change the
                // next line to host.RunAsService();
                host.RunAsService();
            }
            else
            {
                host.Run();
            }
        }
    }
}
