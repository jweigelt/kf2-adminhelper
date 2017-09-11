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

namespace KF2Admin.Scheduler
{
    public class RepeatingSchedulerTask : SchedulerTask
    {
        private Int64 lastRun;
        private DateTime startTime;
        public int Interval { get; set; }

        public RepeatingSchedulerTask(TaskDelegate task) : base(task)
        {
            startTime = new DateTime(1970, 1, 1);
        }
        public RepeatingSchedulerTask(TaskDelegate task, int interval)
            : base(task)
        {
            Interval = interval;
            startTime = new DateTime(1970, 1, 1);
        }

        public void Tick()
        {
            if (Interval == 0)
            {
                Run();
                return;
            }
            else if (lastRun < GetMillis() - Interval)
            {
                lastRun = GetMillis();
                Run();
            }
        }

        private Int64 GetMillis()
        {
            return (Int64)((DateTime.Now - startTime).TotalMilliseconds);
        }

    }
}