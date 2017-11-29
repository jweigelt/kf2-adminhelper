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
using KF2Admin.Utility;
using KF2Admin.Database;
using System.Xml.Serialization;

namespace KF2Admin.Config
{
    public class CoreConfiguration
    {
        [XmlIgnore]
        public const string FILE_NAME = "./cfg/core.xml";

        [XmlIgnore]
        public const string RESOURCE_NAME = "KF2Admin.Resources.cfg.core.xml";

        public string WebAdminURL { get; set; } = "http://blinky.janelo.net:8080";
        public string WebAdminUserName { get; set; } = "admin";
        public string WebAdminPassword { get; set; } = "m0ngo";

        public int TickDelay { get; set; } = 5;
        public int PlayerListUpdateDelay { get; set; } = 5000;
        public int ChatUpdateDelay { get; set; } = 2000;
        public int GameUpdateDelay { get; set; } = 15000;

        public SQLHandler.DbType SQLType { get; set; } = SQLHandler.DbType.SQLite;
        public string SQLiteFile { get; set; } = "KF2Admin.sqlite";
        public string MySQLHostName { get; set; } = "localhost:3306";
        public string MySQLDbName { get; set; } = "kf2admin";
        public string MySQLUserName { get; set; } = "root";
        public string MySQLPassword { get; set; } = "";

        public string CommandKey { get; set; } = "!";

        public LogLevel LogMinimumLevel { get; set; } = LogLevel.Info;
        public bool LogToFile { get; set; } = true;
        public string LogFile { get; set; } = "log.txt";

    }
}