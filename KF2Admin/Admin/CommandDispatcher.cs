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
using System.Collections.Generic;
using KF2Admin.Utility;
using KF2Admin.Config;
using KF2Admin.Admin.Commands;

namespace KF2Admin.Admin
{
    public class CommandDispatcher
    {
        public List<Command> Commands { get; }
        public Command UnknownCommand { get; set; }
        public string CommandKey { get; set; } = "!";

        private AdminTool tool = null;

        public CommandDispatcher(AdminTool tool)
        {
            this.tool = tool;
            Commands = new List<Command>();
            UnknownCommand = LoadCommand(typeof(CmdUnknown), "unknown.xml");
        }

        ~CommandDispatcher()
        {
            Commands.Clear();
        }

        public void Update()
        {
            ChatMessage msg = tool.Web.GetChat();
            if (msg != null)
            {
                if (msg.Message.StartsWith(CommandKey))
                {
                    HandleCommand(msg);
                }
            }
        }

        public void HandleCommand(ChatMessage msg)
        {
            string[] commandArray = msg.Message.Substring(CommandKey.Length).Split(Constants.COMMAND_SPLITKEY);
            Player player = null;

            foreach (Command command in Commands)
            {
                if (command.CommandAlias == commandArray[0])
                {
                    //check permission

                    try
                    {
                        player = tool.Players.GetPlayerByNameMatch(msg.PlayerName, true);
                        if (!command.IsPublic)
                        {
                            if (!tool.Database.HasPermission(player, command.CommandPermission)) throw new Exception("User lacks permissions.");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log("[CMD] Access to {0} denied : {1}", LogLevel.Info, command.CommandAlias, e.Message);
                        return;
                    }


                    //check syntax
                    if (!command.CheckSyntax(commandArray, player))
                    {
                        Logger.Log("[CMD] Syntax error running command {0} [{1}]", LogLevel.Info, msg.Message, msg.PlayerName);
                        command.Say(command.ParameterSyntax);
                        return;
                    }

                    //run
                    Logger.Log("[CMD] Running command {0} [{1}]", LogLevel.Info, msg.Message, msg.PlayerName);
                    command.RunCommand(commandArray, player);
                    return;
                }
            }

            Logger.Log("[CMD] Unknown command '{0}' [{1}]", LogLevel.Info, commandArray[0], msg.PlayerName);
            UnknownCommand.RunCommand(commandArray, player);
        }

        private Command LoadCommand(Type commandType, string xmlConfig)
        {
            ConfigSerializer cs = new ConfigSerializer(commandType);
            Command c = (Command)cs.LoadFromFile("/" + xmlConfig, Constants.CONFIG_DIR_COMMANDS);
            c.Tool = tool;
            return c;
        }

        public void RegisterCommand(Type commandType, string xmlConfig)
        {
            Commands.Add(LoadCommand(commandType, xmlConfig));
        }
    }
}