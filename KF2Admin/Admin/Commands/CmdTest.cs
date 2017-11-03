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
using KF2Admin.Database;
using System.Collections.Generic;
namespace KF2Admin.Admin.Commands
{
    public class CmdTest : Command
    {
        public string OnTest { get; set; } = "Test OK. Usergroups: {0}";

        public CmdTest()
        {
            CommandAlias = "test";
            CommandName = "test";
            CommandPermission = "test";
            CommandDescrption = "Showns user's details";
            ParameterSyntax = "!test";
        }

        public override bool RunCommand(string[] command, Player player)
        {
            string groups = "";
            List<PlayerGroup> groupList = Tool.Database.GetPlayerGroups(player);
            for(int i = 0;i < groupList.Count;i++)
            {
                if (i > 0) groups += ", ";
                groups += groupList[i].Name;
            }

            Say(OnTest, groups);
            return true;
        }
    }
}