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
    public class Command
    {
        public string CommandName { get; set; }
        public string CommandAlias { get; set; }
        public string CommandPermission { get; set; }
        public string ParameterSyntax { get; set; }
        public string CommandDescrption { get; set; }
        public bool IsPublic { get; set; } = false;

        [XmlIgnore]
        public AdminTool Tool { get; set; }

        public virtual string Usage
        {
            get { return CommandAlias + " " + ParameterSyntax; }
        }

        public virtual bool RunCommand(string[] command, Player player)
        {
            return true;
        }

        public virtual bool CheckSyntax(string[] command, Player player)
        {
            return true;
        }

        public void Say(string message, params string[] parameters)
        {
            Tool.Say(message, parameters);
        }

        public string StringSearch(List<string> src, string expression, int maxHelp, string onNoMatch, string onAmbiguous, string onAmbogiousHelp, bool ignoreCase = true)
        {
            List<string> found = new List<string>();
            foreach (string s in src)
            {
                if (ignoreCase) { if (s.ToLower().Contains(expression.ToLower())) found.Add(s); }
                else if (s.Contains(expression)) found.Add(s);
            }

            if (found.Count > 1)
            {
                if (found.Count <= maxHelp) throw new StringSearchException(onAmbogiousHelp, expression, string.Join(", ", found.ToArray()));
                else throw new StringSearchException(onAmbiguous, expression, maxHelp.ToString());
            }
            if (found.Count == 0) throw new StringSearchException(onNoMatch, expression);

            return found[0];
        }

    }
}