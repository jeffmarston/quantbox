using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eze.Quantbox
{
    public class AlgoMaster
    {
        private readonly string _folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eze", "Quantbox");
        private readonly string _filename = "config.json";
        private IClientProxy _publisher;
        private ITradingSystemAdapter _adapter; 

        public List<AbstractAlgoModel> Algos { get; internal set; }
        public EmsSettings EmsSettings
        {
            get { return _adapter.Settings; }
            set { _adapter.Settings = value; }
        }
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

        public AlgoMaster()
        {
            this.Algos = new List<AbstractAlgoModel>();
            Init();
        }

        private void Init()
        {
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
                        _adapter = new EmsAdapter(config.EmsSettings);
                    }
                    catch (DllNotFoundException e)
                    {
                        Console.WriteLine("Error loading EMS Toolkit: " + e.Message);
                        throw;
                    }
                }
                else
                {
                    _adapter = new CsvAdapter(new EmsSettings());
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
                _adapter = new CsvAdapter(new EmsSettings());
                Console.WriteLine("No EMS configuration, using CSV Adapter");
                // No config, create an initial one just for ease of demo

                CreateAlgo(new AlgoMetadata("Algorithm One"));
            }
        }

        public AbstractAlgoModel CreateAlgo(AlgoMetadata metadata)
        {
            var newOne = new RapidAlgo(metadata, _adapter);
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
            config.EmsSettings = _adapter.Settings;

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);

            // If directory doesn't exist, create it
            System.IO.Directory.CreateDirectory(_folderPath);
            System.IO.File.WriteAllText(System.IO.Path.Combine(_folderPath, _filename), json);
        }

    }
}