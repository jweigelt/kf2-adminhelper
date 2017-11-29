/* 
 * This file is part of kf2 adminhelper. (https://github.com/jweigelt/kf2-adminhelper)
 * Copyright (C) 2017 Jan Weigelt (info@janelo.net)
 * 
 * kf2 adminhelper is free software: you can redistribute it and/or modify
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
using System.Reflection;
using KF2Admin.Utility;

namespace KF2Admin.Config
{
    public class FileHandler
    {
        public string ParseFileName(string fileName)
        {
            return fileName.Replace("./", Directory.GetCurrentDirectory() + "/");
        }
        private void CreateDirectoryStructure(string fileName)
        {
            int idx = 0;
            while ((idx = fileName.IndexOfAny(new char[] { '/', '\\' }, ++idx)) > 0)
            {
                string dir = fileName.Substring(0, idx);
                if (!Directory.Exists(dir))
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                        Logger.Log(LogLevel.Verbose, Log.FILE_DIRECTORY_CREATE, dir);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(LogLevel.Error, Log.FILE_DIRECTORY_CREATE_ERROR, dir, e.ToString());
                        throw e;
                    }
                }
            }
        }
        private string GetFileName(Type t)
        {
            string fileName = (string)t.GetField("FILE_NAME").GetValue(null);
            fileName = ParseFileName(fileName);
            return fileName;
        }

        private string GetResourceName(Type t)
        {
            return (string)t.GetField("RESOURCE_NAME").GetValue(null);
        }

        private T ReadXmlFile<T>(string fileName = "")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            if (fileName.Equals("")) fileName = GetFileName(typeof(T));

            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    T obj = (T)serializer.Deserialize(reader);
                    Logger.Log(LogLevel.Verbose, Log.FILE_XML_PARSE, fileName, typeof(T).ToString());
                    return obj;
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, Log.FILE_XML_PARSE_ERROR, fileName, typeof(T).ToString(), e.ToString());
                throw e;
            }
        }
        private void WriteXmlFile<T>(string fileName, T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            if (fileName.Equals("")) fileName = GetFileName(typeof(T));

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    serializer.Serialize(writer, obj);
                    Logger.Log(LogLevel.Verbose, Log.FILE_XML_CREATE, fileName, typeof(T).ToString());
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, Log.FILE_XML_CREATE_ERROR, fileName, typeof(T).ToString(), e.ToString());
                throw e;
            }
        }

        private void UnpackResource(string fileName, string resourceName)
        {
            CreateDirectoryStructure(fileName);
            try
            {
                using (Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        stream.CopyTo(fs);
                    }
                }
                Logger.Log(LogLevel.Info, Log.FILE_DEFAULT_UNPACK, fileName);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, Log.FILE_DEFAULT_UNPACK_ERROR, fileName, e.ToString());
                throw e;
            }
        }

        public void WriteConfigDefault<T>(string fileName = "")
        {
            if (fileName.Equals("")) fileName = GetFileName(typeof(T));
            CreateDirectoryStructure(fileName);
            WriteXmlFile<T>(fileName, (T)Activator.CreateInstance(typeof(T)));
        }

        public void UnpackConfigDefault<T>() where T : new()
        {
            try
            {
                UnpackResource(GetFileName(typeof(T)), GetResourceName(typeof(T)));
            }
            catch
            {
                Logger.Log(LogLevel.Warning, Log.FILE_DEFAULT_USE_AUTO, typeof(T).ToString());
                try
                {
                    WriteXmlFile(GetFileName(typeof(T)), new T());
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, Log.FILE_DEFAULT_USE_AUTO_ERROR, typeof(T).ToString(), e.Message);
                }
            }
        }

        public T ReadConfig<T>() where T: new()
        {
            if (!File.Exists(GetFileName(typeof(T))))
            {
                UnpackConfigDefault<T>();
            }
            return ReadXmlFile<T>();
        }

        public string ReadFileText(string fileName)
        {
            fileName = ParseFileName(fileName);
            return File.ReadAllText(fileName);
        }

        public void WriteFileText(string fileName, string text)
        {
            fileName = ParseFileName(fileName);
            File.WriteAllText(fileName, text);
        }

        public byte[] ReadFileBytes(string fileName)
        {
            fileName = ParseFileName(fileName);
            return File.ReadAllBytes(fileName);
        }
    }
}