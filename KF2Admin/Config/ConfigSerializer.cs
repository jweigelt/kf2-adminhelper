/* 
 * This file is part of kf2 adminhelper.
 * 
 * SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * kf2 adminhelper is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with kf2 adminhelper.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.IO;
using System.Xml.Serialization;
using KF2Admin.Utility;

namespace KF2Admin.Config
{
    public class ConfigSerializer
    {
        private XmlSerializer serializer;
        private Type cfgType;

        public virtual Type CfgType { get { return this.cfgType; } }

        public ConfigSerializer(Type cfgType)
        {
            this.cfgType = cfgType;
            serializer = new XmlSerializer(cfgType);
        }

        ~ConfigSerializer()
        {
            serializer = null;
        }

        public void SaveToFile(string fileName, string directoryName, object config, bool relative = true)
        {
            if(relative) directoryName = Directory.GetCurrentDirectory() + directoryName;
            try
            {
                if (!System.IO.Directory.Exists(directoryName))
                {
                    System.IO.Directory.CreateDirectory(directoryName);
                }
                fileName = directoryName + fileName;
                StreamWriter writer = new StreamWriter(fileName);
                serializer.Serialize(writer, config);
                writer.Close();
                Logger.Log("[XML] Stored '{0}'.", LogLevel.Info, fileName);
            }
            catch (Exception ex)
            {
                Logger.Log("[XML] Couldn't write to '{0}'. \n {1}", LogLevel.Error, fileName, ex.ToString());
            }
        }

        public object LoadFromFile(string fileName, string directoryName, bool relative = true)
        {
            if (relative) directoryName = Directory.GetCurrentDirectory() + directoryName;
            string filePath = directoryName + fileName;
            object config = null;
            if (File.Exists(filePath))
            {
                try
                {
                    StreamReader reader = new StreamReader(filePath);
                    config = serializer.Deserialize(reader);
                    reader.Close();
                    Logger.Log("[XML] XML-File '{0}' parse OK.", LogLevel.Info, fileName);
                }
                catch (Exception ex)
                {
                    config = Activator.CreateInstance(cfgType);
                    Logger.Log("[XML] Couldn't read '{0}'. Using default. \n {1}", LogLevel.Error, fileName, ex.ToString());
                }
            }
            else
            {
                Logger.Log("[XML] Generating default Config for '{0}'.", LogLevel.Info, fileName);
                config = Activator.CreateInstance(cfgType);
                SaveToFile(fileName, directoryName, config, false);
            }
            return config;
        }

    }
}