using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RealTick.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Eze.SchemaCompare
{
    public class DataAccessor
    {
        private readonly string _folderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Eze", "SchemaCompare");
        private readonly string _filename = "config.json";

        public IClientProxy Publisher { get; set; }

        public Dictionary<string, SchemaData> SchemaDictionary { get; internal set; }

        public DataAccessor()
        {
            Init();
        }

        private void Init()
        {
            string filePath = System.IO.Path.Combine(_folderPath, _filename);
            if (System.IO.File.Exists(filePath))
            {
                string txt = System.IO.File.ReadAllText(filePath);
                this.SchemaDictionary = JsonConvert.DeserializeObject<Dictionary<string, SchemaData>>(txt);
            }
            else
            {
                Console.WriteLine("No configuration found, using all defaults");
                this.SchemaDictionary = new Dictionary<string, SchemaData>();
                this.SchemaDictionary.Add("*2019.4", new SchemaData());
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this.SchemaDictionary, Formatting.Indented);

            try
            {
                Console.WriteLine("Saving to: " + System.IO.Path.Combine(_folderPath, _filename));                
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