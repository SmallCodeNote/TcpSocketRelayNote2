using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketSignalServer
{
    class FailoverWorker
    {
        private Task worker;
        private bool _WorkerRun = true;
        public int IntervalSec = 1;

        public FailoverWorker()
        {
            
        }

        private async void Worker()
        {
            while (_WorkerRun)
            {/*
                await Task.Run(() =>
                {
                    List<Task> taskList = new List<Task>();

                    foreach (FailoverActiveView ctrl in panel_FailoverSystemView.Controls)
                    {
                        ctrl.askAlive();
                    }

                    bool activeAlive = false;

                    foreach (FailoverActiveView ctrl in panel_FailoverSystemView.Controls)
                    {
                        activeAlive = activeAlive || ctrl.Alive;
                    }

                    noticeTransmitter.isActive = !activeAlive;

                    statusStripUpdate(activeAlive);

                });
                */
                Thread.Sleep(IntervalSec * 1000);
            }
        }

        public void Start()
        {
            if (!_WorkerRun || worker == null || worker.IsCompleted)
            {
                _WorkerRun = true;
                worker = Task.Run(() => Worker());
            }
        }

        public void Stop()
        {
            _WorkerRun = false;
            Task.WaitAll(new Task[] { worker });
        }


    }
}
