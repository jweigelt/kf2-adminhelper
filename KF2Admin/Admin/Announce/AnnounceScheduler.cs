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
using KF2Admin.Scheduler;

namespace KF2Admin.Admin.Announce
{
    class AnnounceScheduler
    {
        private AdminTool tool = null;
        private AnnounceConfiguration config = null;
        private int currentIndex = 0;

        public AnnounceScheduler(AdminTool tool)
        {
            this.tool = tool;
        }

        public bool Open()
        {
            config = tool.FileIO.ReadConfig<AnnounceConfiguration>();

            if (config.Enable && config.AnnounceList.Count > 0)
            {
                tool.Scheduler.PushRepeatingTask(new RepeatingSchedulerTask(() => Update(), config.AnnounceDelay));
            }
            return true;
        }

        private void Update()
        {
            if (config.AnnounceList.Count <= currentIndex) currentIndex = 0;
            string announce = config.AnnounceList[currentIndex++].ParseText();
            Logger.Log(LogLevel.Verbose, "[ANN] Broadcasting Announce '{0}'", announce);
            tool.Web.Say(announce);
        }
    }
}
