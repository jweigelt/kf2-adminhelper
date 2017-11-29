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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KF2Admin.Admin.Commands
{
    public class CmdLength : Command
    {
        [XmlIgnore]
        public const string FILE_NAME = "./cfg/cmd/length.xml";

        [XmlIgnore]
        public const string RESOURCE_NAME = "KF2Admin.Resources.cfg.cmd.length.xml";

        public string OnNoGameLengthMatch { get; set; } = "Unkown game length '{0}'";
        public string OnChangeGameLength { get; set; } = "Changing game length to '{0}'";

        public List<GameLength> GameLengthPresets { get; set; } 

        public CmdLength()
        {
            CommandAlias = "length";
            CommandName = "length";
            CommandPermission = "length";
            CommandDescrption = "Changes the game length.";
            ParameterSyntax = "!length <length>";

            GameLengthPresets = new List<GameLength>();
            GameLengthPresets.Add(new GameLength("0", "Short", "short"));
            GameLengthPresets.Add(new GameLength("1", "Normal", "normal"));
            GameLengthPresets.Add(new GameLength("2", "Long", "long"));
            GameLengthPresets.Add(new GameLength("3", "Custom", "custom"));
        }

        public override bool CheckSyntax(string[] command, Player player)
        {
            if (command.Length < 2) return false;
            return true;
        }

        public override bool RunCommand(string[] command, Player player)
        {
            foreach (GameLength l in GameLengthPresets)
            {
                if (l.Alias.ToLower().Equals(command[1].ToLower()))
                {
                    Say(OnChangeGameLength, l.Name);
                    Tool.Web.ChangeGameLength(l);
                    return true;
                }
            }
            Say(OnNoGameLengthMatch, command[1]);

            return false;
        }

    }
}