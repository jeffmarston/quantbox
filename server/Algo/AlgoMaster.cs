using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eze.Quantbox
{
    public class AlgoMaster
    {
        private static string _folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eze", "Quantbox");
        private const string _filename = "config.json";
        private IClientProxy _publisher;

        public List<AbstractAlgoModel> Algos { get; internal set; }
        public IClientProxy Publisher
        {
            get => _publisher;
            set {
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
            List<AbstractAlgoModel> algos = new List<AbstractAlgoModel>();
            var adapter = new CsvAdapter();
            // var adapter = new EmsAdapter();

            string filePath = System.IO.Path.Combine(_folderPath, _filename);
            if (System.IO.File.Exists(filePath))
            {
                string txt = System.IO.File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<QuantBoxConfig>(txt);
                foreach (var metadata in config.Metadata)
                {
                    algos.Add(new RapidAlgo(metadata, adapter));
                }
            } else
            {
                // No config, create an initial one just for ease of demo
                algos.Add(new RapidAlgo(new AlgoMetadata("Algorithm One"), adapter));
                algos.Add(new RapidAlgo(new AlgoMetadata("Algorithm Two"), adapter));

            }
            Algos = algos;
        }

        public void Save()
        {
            var config = new QuantBoxConfig();
            config.Metadata = from algo in Algos select algo.Metadata;
            
            string json = JsonConvert.SerializeObject(config);

            // If directory doesn't exist, create it
            System.IO.Directory.CreateDirectory(_folderPath);
            System.IO.File.WriteAllText(System.IO.Path.Combine(_folderPath, _filename), json);
        }
    }
}