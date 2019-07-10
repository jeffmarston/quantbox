using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Management;
using Eze.AdminConsole.Machines;
using System.IO;

namespace Eze.AdminConsole.Services
{
    public static class ServiceUtils
    {
        public static void StartService(string svcName)
        {
            var serviceController = new ServiceController(svcName);
            if (serviceController.Status.ToString() == "Stopped")
            {
                serviceController.Start();
            }
        }

        public static void StopService(string svcName)
        {
            var serviceController = new ServiceController(svcName);
            if (serviceController.Status.ToString() == "Running")
            {
                serviceController.Stop();
            }
        }

        public static Service GetServiceDetails(ServiceController svcTemp)
        {
            var svc = new Service()
            {
                name = svcTemp.ServiceName,
                status = svcTemp.Status.ToString()
            };
            return svc;
        }

        public static Service GetService(string svcName)
        {
            var svcContollers = ServiceController.GetServices().Where(o => o.ServiceName.ToLower() == svcName.ToLower()).ToList();
            return (svcContollers.Count() == 0) ? null : GetServiceDetails(svcContollers[0]);
        }

        public static List<Service> GetAllEzeServices()
        {
            try
            {
                ManagementPath path = new ManagementPath("Win32_Service");
                ManagementClass services;
                services = new ManagementClass(path, null);

                var serviceData = new List<Service>();
                foreach (ManagementObject service in services.GetInstances())
                {
                    foreach (var prop in service.Properties)
                    {
                        Console.WriteLine(prop.Name + ": " + prop.Value);
                    }
                    var fileName = Path.GetFileNameWithoutExtension(service.GetPropertyValue("PathName").ToString()).ToLower();

                    if (fileName.StartsWith("eze."))
                    {
                        var svc = new Service()
                        {
                            name = service.GetPropertyValue("Name").ToString(),
                            filename = fileName,
                            status = service.GetPropertyValue("State").ToString(),
                            pid = Int32.Parse(service.GetPropertyValue("ProcessId").ToString()),
                            path = service.GetPropertyValue("PathName").ToString(),
                            startMode = service.GetPropertyValue("StartMode").ToString(),
                            startName = service.GetPropertyValue("StartName").ToString()
                        };
                        serviceData.Add(svc);
                    }
                }
                return serviceData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<Service>();
            }
        }
    }
}