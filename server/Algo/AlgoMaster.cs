using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RealTick.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Eze.Quantbox
{
    public class AlgoMaster
    {
        private readonly string _folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eze", "Quantbox");
        private readonly string _filename = "config.json";
        private IClientProxy _publisher;
        private CsvAdapter _csvAdapter;
        private EmsAdapter _emsAdapter;
        private ITradingSystemAdapter _activeAdapter;
        private EmsSettings _emsSettings;

        public List<AbstractAlgoModel> Algos { get; internal set; }
        public EmsSettings EmsSettings {
            get => _emsSettings;
            set => _emsSettings = value; }
        public IClientProxy Publisher
        {
            get => _publisher;
            set
            {
                _publisher = value;
                foreach (var algo in Algos)
                {
                    algo.Publisher = _publisher;
                }
            }
        }

        public string ActiveAdapter
        {
            get
            {
                return (_activeAdapter is EmsAdapter) ? "EMS" : "CSV";
            }
            set
            {
                _activeAdapter = (value == "EMS") ? (ITradingSystemAdapter)_emsAdapter : (ITradingSystemAdapter)_csvAdapter;
                foreach (var algo in Algos)
                {
                    algo.Adapter = _activeAdapter;
                }
            }
        }

        public AlgoMaster()
        {
            this.Algos = new List<AbstractAlgoModel>();
            Init();
        }

        private void Init()
        {
            _activeAdapter = _csvAdapter = new CsvAdapter();
            string filePath = System.IO.Path.Combine(_folderPath, _filename);
            if (System.IO.File.Exists(filePath))
            {
                string txt = System.IO.File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<QuantBoxConfig>(txt);

                // EMS Settings
                if (config.EmsSettings != null && config.EmsSettings.Gateway != null && config.EmsSettings.Gateway.Length > 0)
                {
                    Console.WriteLine("Using EMS Adapter at gateway: " + config.EmsSettings.Gateway);
                    try
                    {
                        EmsSettings = config.EmsSettings;
                        _activeAdapter = _emsAdapter = new EmsAdapter(config.EmsSettings);
                    }
                    catch (DllNotFoundException e)
                    {
                        Console.WriteLine("Error loading EMS Toolkit: " + e.Message);
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry("Error loading EMS Toolkit: " + e.Message, EventLogEntryType.Error, 101, 1);
                        }
                        throw;
                    }
                    catch (ToolkitPermsException tkEx)
                    {
                        Console.WriteLine("Error authenticating with EMS Toolkit: " + tkEx.Message);
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry("Error authenticating with EMS Toolkit: " + tkEx.Message, EventLogEntryType.Error, 101, 1);
                        }
                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("No EMS configuration, using CSV Adapter");
                }

                // Algo Metadata
                foreach (var metadata in config.Metadata)
                {
                    CreateAlgo(metadata);
                }
            }
            else
            {
                Console.WriteLine("No configuration found, using all defaults");
                // No config, create an initial one just for ease of demo

                CreateAlgo(new AlgoMetadata("Algorithm One"));
            }
        }

        public AbstractAlgoModel CreateAlgo(AlgoMetadata metadata)
        {
            var newOne = new RapidAlgo(metadata, _activeAdapter);
            var date = DateTime.Now;
            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
            for (int i = -120; i < 0; i += 2)
            {
                newOne.History.Add(new AlgoHistory() { Date = date.AddSeconds(i), Value = 0 });
            }
            newOne.Publisher = _publisher;
            Algos.Add(newOne);
            return newOne;
        }

        public void Save()
        {
            var config = new QuantBoxConfig();
            config.Metadata = from algo in Algos select algo.Metadata;
            config.EmsSettings = EmsSettings;
            config.ActiveAdapter = (_activeAdapter is EmsAdapter) ? "EMS" : "CSV";

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);

            try
            {
                Console.WriteLine("Saving to: " + System.IO.Path.Combine(_folderPath, _filename));

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Saving to: " + System.IO.Path.Combine(_folderPath, _filename), EventLogEntryType.Information, 101, 1);
                }

                // If directory doesn't exist, create it
                System.IO.Directory.CreateDirectory(_folderPath);
                System.IO.File.WriteAllText(System.IO.Path.Combine(_folderPath, _filename), json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to save: " + e);

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "BlackBoxSim";
                    eventLog.WriteEntry("Failed to save: " + e, EventLogEntryType.Information, 101, 1);
                }
            }
        }
    }
}