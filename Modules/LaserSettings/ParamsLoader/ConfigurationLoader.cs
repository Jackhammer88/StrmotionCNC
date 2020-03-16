using Infrastructure.Interfaces.Logger;
using LaserSettings.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LaserSettings.ParamsLoader
{
    public class ConfigurationLoader
    {
        private static string DEFAULT_PATH = @"D:\Sources\Cnc_prism\CNC\bin\Debug\laser-settings\default.json";
        private readonly ILoggerExtended _loggerExtended;

        public ConfigurationLoader()
        {
        }

        public ConfigurationLoader(ILoggerExtended loggerExtended)
        {
            _loggerExtended = loggerExtended;
        }
        public bool LoadFromDisk(string path, out LaserConfiguration laserConfiguration)
        {
            laserConfiguration = null;
            try
            {
                var rawData = File.ReadAllText(path);
                laserConfiguration = JsonConvert.DeserializeObject<LaserConfiguration>(rawData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                return true;
            }
            catch (JsonSerializationException ex)
            {
                _loggerExtended.Exception($"Error during opening a file. {path}", ex);
                return false;
            }
            catch (JsonReaderException ex)
            {
                _loggerExtended.Exception($"Error during opening a file. {path}", ex);
                return false;
            }
            catch (IOException ex)
            {
                _loggerExtended.Exception($"Error during opening a file. {path}", ex);
                return false;
            }
        }
        public LaserConfiguration LoadLastOrEmpty()
        {
            LaserConfiguration result = null;
            if (File.Exists(DEFAULT_PATH))
            {
                try
                {
                    var rawData = File.ReadAllText(DEFAULT_PATH);
                    result = JsonConvert.DeserializeObject<LaserConfiguration>(rawData, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                }
                catch (JsonSerializationException ex)
                {
                    _loggerExtended.Exception("LoadLastOrEmpty", ex);
                }
                catch (IOException ex)
                {
                    _loggerExtended.Exception("LoadLastOrEmpty", ex);
                }
            }

            return result ?? new LaserConfiguration
            {
                MaterialType = Material.Steel,
                MaterialThickness = 2,
                Tables = GenerateDefaultTables()
            };
        }
        public bool Save(LaserConfiguration configuration, string path)
        {
            try
            {
                var data = JsonConvert.SerializeObject(configuration, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                File.WriteAllText(path, data);
                return true;
            }
            catch (IOException ex)
            {
                _loggerExtended.Exception($"Error during saving a file. {path}", ex);
                return false;
            }
            catch(JsonSerializationException ex)
            {
                _loggerExtended.Exception($"Error during saving a file. {path}", ex);
                return false;
            }
            catch(JsonWriterException ex)
            {
                _loggerExtended.Exception($"Error during saving a file. {path}", ex);
                return false;
            }
        }


        private List<LaserParameterTableBase> GenerateDefaultTables()
        {
            return new List<LaserParameterTableBase>
                {
                    new Punching(), new BurnNormal(), new BurnSoft(),
                    new Evaporation(), new MicroweldSoft(), new SmallContour(),
                    new ApproachNormal(), new ApproachPrecut(), new Engraving(),
                    new Cooling(), new ApproachRedAcc(), new MiddleContour(),
                    new ApproachRedSpeed(), new PreBurn(), new ApproachRedSpeed(),
                    new MicroweldHard(), new LargeContour()
                };
        }
    }
}
