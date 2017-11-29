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
    public class CmdDifficulty : Command
    {
        [XmlIgnore]
        public const string FILE_NAME = "./cfg/cmd/difficulty.xml";

        [XmlIgnore]
        public const string RESOURCE_NAME = "KF2Admin.Resources.cfg.cmd.difficulty.xml";

        public string OnNoDifficultyMatch { get; set; } = "Unkown difficulty '{0}'";
        public string OnChangeDifficulty { get; set; } = "Changing game difficulty to '{0}'";

        public List<Difficulty> DifficultyPresets { get; set; } 

        public CmdDifficulty()
        {
            CommandAlias = "difficulty";
            CommandName = "difficulty";
            CommandPermission = "difficulty";
            CommandDescrption = "Changes the game difficulty.";
            ParameterSyntax = "!difficulty <difficulty>";

            DifficultyPresets = new List<Difficulty>();
            DifficultyPresets.Add(new Difficulty("0.0000", "Normal", "normal"));
            DifficultyPresets.Add(new Difficulty("1.0000", "Hard", "hard"));
            DifficultyPresets.Add(new Difficulty("2.0000", "Suicidal", "suicidal"));
            DifficultyPresets.Add(new Difficulty("3.0000", "Hell On Earth", "hell"));
        }

        public override bool CheckSyntax(string[] command, Player player)
        {
            if (command.Length < 2) return false;
            return true;
        }

        public override bool RunCommand(string[] command, Player player)
        {
            foreach (Difficulty d in DifficultyPresets)
            {
                if (d.Alias.ToLower().Equals(command[1].ToLower()))
                {
                    Say(OnChangeDifficulty, d.Name);
                    Tool.Web.ChangeDifficulty(d);
                    return true;
                }
            }
            Say(OnNoDifficultyMatch, command[1]);

            return false;
        }

    }
}