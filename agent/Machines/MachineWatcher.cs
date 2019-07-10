using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Eze.AdminConsole.Machines;
using Eze.AdminConsole.Environment;
using System.Diagnostics;

namespace Eze.AdminConsole.Machines
{
    public class MachineWatcher
    {
        private Dictionary<string, MachineData> _dataMap = new Dictionary<string, MachineData>();
        private Dictionary<string, List<PerformanceCounter>> _counterMap = new Dictionary<string, List<PerformanceCounter>>();
        private static Timer pollingTimer = null;
        //private static IHubContext<ServiceMgmtHub> _context;
        private static IHubCallerClients signalrClients;

        public static void Init(IHubCallerClients clients, Topology topology)
        {
            if (_instance == null)
            {
                signalrClients = clients;
                _instance = new MachineWatcher(topology);
            }
        }

        private static MachineWatcher _instance = null;
        private const int TimerIntervalMs = 1000;

        private MachineWatcher(Topology topology)
        {
            try{
            foreach (var machine in topology.servers)
            {
                _dataMap[machine.name] = new MachineData();
                _counterMap[machine.name] = new List<PerformanceCounter>() {
                    new PerformanceCounter("Processor", "% Idle Time", "_Total", machine.name),
                    new PerformanceCounter("Memory", "Available MBytes", "_Total", machine.name)
                };
            }
            } catch (Exception e ) {
                Console.WriteLine(e);
            }
           // pollingTimer = new Timer(DoPoll, null, 0, TimerIntervalMs);
        }

        private MachineData GetNextPerfCounters(string machineName)
        {
            var data = new MachineData();
            data.cpuPercent = _counterMap[machineName][0].NextValue();
            data.memoryMb = _counterMap[machineName][1].NextValue();
            return data;
        }

        private void DoPoll(object state)
        {
            Console.WriteLine("--");
            foreach (var machineName in _dataMap.Keys)
            {
                var prevStats = _dataMap[machineName];
                var newStats = GetNextPerfCounters(machineName);
                //var timeDiff = (newStats.idleCpuTime - prevStats.idleCpuTime);
                //newStats.cpuPercent = 100 - timeDiff / (1); // only works with 1 second interval;

                _dataMap[machineName].cpuPercent = newStats.cpuPercent;
                _dataMap[machineName].memoryMb = newStats.memoryMb;

                //if (newStats.cpuPercent >= 0 && newStats.cpuPercent <= 100)
                {
                    PublishChange(machineName, newStats);
                }
            }
        }
        private void PublishChange(string machineName, MachineData machData)
        {
            signalrClients.All.SendAsync("machine", machineName, machData);
            Console.WriteLine($"SignalR [machine] : " + machineName + ", " + machData.cpuPercent + ", " + machData.idleCpuTime);
        }
    }
}
