using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Eze.Quantbox
{
    public class AlgoMaster
    {
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
            List<AbstractAlgoModel> algos;
            string folderbase = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string filePath = System.IO.Path.Combine(folderbase, "Eze", "Quantbox", "algos.json");
            if (System.IO.File.Exists(filePath))
            {
                string txt = System.IO.File.ReadAllText(filePath);
                algos = JsonConvert.DeserializeObject<List<AbstractAlgoModel>>(txt);
            }

            var adapter = new EmsAdapter();
            //var adapter = new CsvAdapter();

            algos = new List<AbstractAlgoModel>() {
                new RapidAlgo() { Name = "Algo1", Adapter = adapter, Enabled = true },
                new SlowBatchAlgo() { Name = "Algo2", Adapter = adapter, Enabled = true },
                new SlowBatchAlgo() { Name = "Algo3", Adapter = adapter, Enabled = false }
            };
            foreach (var algo in algos)
            {
                Console.WriteLine("Initialized: " + algo.Name);
            }
            Algos = algos;
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(Algos);
            string folderbase = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string folderPath = System.IO.Path.Combine(folderbase, "Eze", "Quantbox");
            // If directory doesn't exist, create it
            System.IO.Directory.CreateDirectory(folderPath);
            System.IO.File.WriteAllText(System.IO.Path.Combine(folderPath, "algos.json"), json);
        }
    }
}