﻿/* 
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
using System.Threading;
using System.Collections.Generic;

namespace KF2Admin.Scheduler
{
    public class TaskScheduler
    {
        public int TickDelay { get; set; } = 10;
        private Queue<SchedulerTask> taskQueue;
        private List<RepeatingSchedulerTask> repeatingTasks;

        private Thread workThread;
        private bool running;

        public TaskScheduler()
        {
            taskQueue = new Queue<SchedulerTask>();
            repeatingTasks = new List<RepeatingSchedulerTask>(); ;
        }

        public void Start()
        {
            running = true;
            workThread = new Thread(this.DoWork);
            workThread.Start();
        }

        public void Stop()
        {
            running = false;
            if (workThread != null)
            {
                workThread.Join();
                workThread = null;
            }
        }

        public void PushTask(SchedulerTask task)
        {
            taskQueue.Enqueue(task);
        }

        public void PushRepeatingTask(RepeatingSchedulerTask task)
        {
            repeatingTasks.Add(task);
        }

        private void DoWork()
        {
            while (running)
            {
                if (taskQueue.Count > 0)
                {
                    taskQueue.Dequeue().Run();
                }

                foreach (RepeatingSchedulerTask task in this.repeatingTasks)
                {
                    task.Tick();
                }
                Thread.Sleep(TickDelay);
            }
        }

        ~TaskScheduler()
        {
            Stop();
        }

    }
}