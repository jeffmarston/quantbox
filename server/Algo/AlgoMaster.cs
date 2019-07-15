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

        //        private ITradingSystemAdapter _adapter = new EmsAdapter(EmsSettings.CreateDefault());
        private ITradingSystemAdapter _adapter = new CsvAdapter(EmsSettings.CreateDefault());

        public List<AbstractAlgoModel> Algos { get; internal set; }
        public EmsSettings EmsSettings
        {
            get
            {
                return _adapter.Settings;
            }
            set
            {
                _adapter = new CsvAdapter(value);
                foreach (var algo in Algos)
                {
                    algo.Adapter = _adapter;
                }
            }
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
                if (config.EmsSettings != null)
                {
                    _adapter = new CsvAdapter(config.EmsSettings);
                }

                // Algo Metadata
                foreach (var metadata in config.Metadata)
                {
                    CreateAlgo(metadata);
                }
            }
            else
            {
                // No config, create an initial one just for ease of demo
                CreateAlgo(new AlgoMetadata("Algorithm One"));
            }
        }

        public AbstractAlgoModel CreateAlgo(AlgoMetadata metadata)
        {
            var newOne = new RapidAlgo(metadata, _adapter);
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