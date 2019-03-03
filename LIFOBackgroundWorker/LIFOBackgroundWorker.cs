using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace LIFOBackgroundWorker
{
    public class LIFOBackgroundWorker
    {
        public ConcurrentStack<IStackObject> LIFOStack { get; private set; }
        public List<IStackObject> StackObjectCollection { get; private set; }
        private BackgroundWorker backgroundWorker;
        private ManualResetEvent manualResetEvent;
        private bool finishingExecution;
        public event EventHandler<IStackObject> ItemProcessed;
        public event EventHandler<List<IStackObject>> StackProcessed;

        public LIFOBackgroundWorker()
        {
            LIFOStack = new ConcurrentStack<IStackObject>();
            StackObjectCollection = new List<IStackObject>();
            finishingExecution = false;
            manualResetEvent = new ManualResetEvent(false);
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker localBGW = sender as BackgroundWorker;
            while (!localBGW.CancellationPending)
            {
                manualResetEvent.WaitOne(); //wait for a object to be added to the stack. Also set when calling FinishExecution() or AbordExecution();
                manualResetEvent.Reset();
                if (localBGW.CancellationPending)
                {
                    break;
                } else if (finishingExecution)
                {
                    //Process the rest of the stack and break
                    while(LIFOStack.Count > 0)
                    {
                        if (LIFOStack.TryPop(out IStackObject result))
                        {
                            ProcessIStackObject(result);                            
                        }
                    }
                    break;
                } else if (LIFOStack.TryPop(out IStackObject result))
                {
                    ProcessIStackObject(result);
                }
            }
        }

        private void ProcessIStackObject(IStackObject obj)
        {
            obj.Process();
            obj.IsProcessed = true;
            ItemProcessed?.Invoke(this, obj);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StackProcessed?.Invoke(this, StackObjectCollection);
        }

        public void StartExecution()
        {
            backgroundWorker.RunWorkerAsync();
        }

        public void AbortExecution()
        {
            backgroundWorker.CancelAsync();
            manualResetEvent.Set();
        }

        public void FinishExecution()
        {
            finishingExecution = true;
            manualResetEvent.Set();
        }

        public void Add(IStackObject stackObject)
        {
            LIFOStack.Push(stackObject);
            StackObjectCollection.Add(stackObject);
            manualResetEvent.Set();
        }



    }
}
