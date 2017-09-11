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
using KF2Admin.Web;
using KF2Admin.Admin;
using KF2Admin.Utility;
using KF2Admin.Scheduler;
using KF2Admin.Database;
using KF2Admin.Config;
using KF2Admin.Admin.Announce;
using KF2Admin.Admin.Commands;

namespace KF2Admin
{
    public class AdminTool
    {
        public WebHelper Web { get; }
        public PlayerHandler Players { get; }
        public CommandDispatcher Commands { get; }
    
        AnnounceScheduler AutoAnnounce { get; set; }

        public TaskScheduler Scheduler { get; }
        public SQLHandler Database { get; }

        public virtual CoreConfiguration Config { get { return config; } }
        public virtual ServerStatus Status { get; }

        private CoreConfiguration config = null;
        private bool running = false;

        public AdminTool()
        {
            Logger.Log("{0} v{1}, {2}", LogLevel.Banner, Constants.PRODUCT_NAME, Constants.PRODUCT_VERSION, Constants.PRODUCT_AUTHOR);
            Web = new WebHelper();
            Players = new PlayerHandler(this);
            Commands = new CommandDispatcher(this);
            Scheduler = new TaskScheduler();
            Database = new SQLHandler();
            AutoAnnounce = new AnnounceScheduler(this);
            Status = new ServerStatus();
        }

        public void Run()
        {
            running = true;
            ConfigSerializer cs = new ConfigSerializer(typeof(CoreConfiguration));
            config = (CoreConfiguration) cs.LoadFromFile(Constants.CONFIG_FILE_CORE, Constants.CONFIG_DIR);

            Database.SQLiteFileName = Config.SQLiteFile;

            Web.UserName = config.WebAdminUserName;
            Web.Password = config.WebAdminPassword;
            Web.WebAdminUrl = config.WebAdminURL;
            Web.UserAgent =  Constants.PRODUCT_NAME + "/" + Constants.PRODUCT_VERSION;

            Commands.RegisterCommand(typeof(CmdTest), "test.xml");
            Commands.RegisterCommand(typeof(CmdMute), "mute.xml");
            Commands.RegisterCommand(typeof(CmdUnmute), "unmute.xml");
            Commands.RegisterCommand(typeof(CmdKick), "kick.xml");
            Commands.RegisterCommand(typeof(CmdBanIP), "ipban.xml");
            Commands.RegisterCommand(typeof(CmdBanSession), "sessionban.xml");
            Commands.RegisterCommand(typeof(CmdBanUqId), "ban.xml");
            Commands.RegisterCommand(typeof(CmdMap), "map.xml");
            Commands.RegisterCommand(typeof(CmdDifficulty), "difficulty.xml");
            Commands.RegisterCommand(typeof(CmdPutGroup), "putgroup.xml");
            Commands.RegisterCommand(typeof(CmdRmGroup), "rmgroup.xml");
            Commands.RegisterCommand(typeof(CmdGimmeAdmin), "gimmeadmin.xml");

            if (!AutoAnnounce.Open()) return;
            if (!Database.Open()) return;

            Scheduler.TickDelay = Config.TickDelay;
            Scheduler.PushRepeatingTask(new RepeatingSchedulerTask(() => Players.Update(), Config.PlayerListUpdateDelay));
            Scheduler.PushRepeatingTask(new RepeatingSchedulerTask(() => Commands.Update(), Config.ChatUpdateDelay));

            if (!Web.Authenticate()) return;

            Status.InstalledMaps = Web.GetInstalledMaps();
            if (Status.InstalledMaps == null) return;
            Status.InstalledGamemodes = Web.GetInstalledGamemodes();
            if (Status.InstalledGamemodes == null) return;

            Scheduler.Start();

            HandleConsole();

            Scheduler.Stop();
            Web.Logout();
            Database.Close();
        }

        private void HandleConsole()
        {
            string command = string.Empty;
            while (running)
            {
                command = Console.ReadLine();
                if (command == "exit") running = false;
            }
        }
    }
}