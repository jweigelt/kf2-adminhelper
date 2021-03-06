﻿/* 
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
namespace KF2Admin.Admin.Commands
{
    public class CmdUnknown : Command
    {
        [XmlIgnore]
        public const string FILE_NAME = "./cfg/cmd/unknown.xml";

        [XmlIgnore]
        public const string RESOURCE_NAME = "KF2Admin.Resources.cfg.cmd.unknown.xml";

        public string OnInvalidUserInput { get; set; } = "Unknown command.";

        public CmdUnknown()
        {
            CommandAlias = "unknown";
            CommandName = "unknown";
            CommandPermission = "unknown";
            CommandDescrption = "Dummy-command for unkown command input";
            ParameterSyntax = "";
            IsPublic = true;
        }

        public override bool RunCommand(string[] command, Player player)
        {
            Say(OnInvalidUserInput);
            return true;
        }
    }
}