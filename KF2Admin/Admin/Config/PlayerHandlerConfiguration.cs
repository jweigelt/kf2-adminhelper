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
using System.Xml.Serialization;
namespace KF2Admin.Config
{
    public class PlayerHandlerConfiguration
    {
        [XmlIgnore]
        public const string FILE_NAME = "./cfg/playerhandler.xml";

        [XmlIgnore]
        public const string RESOURCE_NAME = "KF2Admin.Resources.cfg.playerhandler.xml";

        public int NewPlayerTimeout { get; set; } = 120;

        public bool WelcomeNewPlayer { get; set; } = true;
        public string OnWelcomeNewPlayer { get; set; } = "Welcome '{0}', Player #{1}";

        public bool WelcomeOldPlayer { get; set; } = true;
        public string OnWelcomeOldPlayer { get; set; } = "Welcome back, {0}, played {1} times on this server.";
    }
}
