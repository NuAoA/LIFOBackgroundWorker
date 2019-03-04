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
        #region Public Properties


        /// <summary>
        /// The collection of all items added to the stack. 
        /// </summary>
        public IReadOnlyCollection<IStackItem> StackItemCollection
        {
            get
            {
                return stackItemCollection.AsReadOnly();
            }
        }

        /// <summary>
        /// Set to false to disable multithreading on the FinishExecution() processing calls.
        /// </summary>
        public bool UseMultithreading { get; set; } = true;

        /// <summary>
        /// This event is fired every time a new entry to the stack is processed. It will only fire on sequential entries and not during batch processing once FinishExecution() is called
        /// </summary>
        public event EventHandler<IStackItem> ItemProcessed;

        /// <summary>
        /// This event is fired when all processing is complete for every item added to the stack. Can only fire after FinishExecution() is called.
        /// </summary>
        public event EventHandler<List<IStackItem>> StackProcessed;
        #endregion

        #region Private Properties
        /// <summary>
        /// The current LIFO stack. This object will consumed entries item by item in a single thread, always grabbing the latest item on the stack.
        /// </summary>
        private ConcurrentStack<IStackItem> LIFOStack { get; set; }

        private BackgroundWorker backgroundWorker;
        private ManualResetEventSlim manualResetEvent;
        private List<IStackItem> stackItemCollection { get; set; }
        private bool finishingExecution;
        #endregion

        #region Constructors
        public LIFOBackgroundWorker()
        {
            LIFOStack = new ConcurrentStack<IStackItem>();
            stackItemCollection = new List<IStackItem>();
            finishingExecution = false;
            manualResetEvent = new ManualResetEventSlim(false);
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
        }
        #endregion


        #region Private Methods
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker localBGW = sender as BackgroundWorker;
            while (!localBGW.CancellationPending)
            {
                manualResetEvent.Wait(); //wait for a item to be added to the stack. Also set when calling FinishExecution() or AbordExecution();
                manualResetEvent.Reset();
                if (localBGW.CancellationPending)
                {    
                    break;
                }
                else if (finishingExecution)
                {
                    //Process the rest of the stack and break
                    processRemainingStackItems(UseMultithreading);
                    break;
                }
                else if (LIFOStack.TryPop(out IStackItem result))
                {
                    ProcessIStackItem(result);
                }
            }

            if (localBGW.CancellationPending)
            {
                e.Cancel = true;
            }            
        }

        private void processRemainingStackItems(bool useThreading)
        {
            BlockingCollection<IStackItem> consumer = new BlockingCollection<IStackItem>(LIFOStack);
            consumer.CompleteAdding();
            try
            {
                if (consumer.Count > 0)
                {
                    if (useThreading)
                    {
                        Parallel.ForEach(consumer.GetConsumingEnumerable(), (item) =>
                        {
                            System.Diagnostics.Debug.WriteLine($"Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                            ProcessIStackItem(item, true);                            
                        });
                    }
                    else
                    {
                        foreach (var item in consumer.GetConsumingEnumerable())
                        {
                            ProcessIStackItem(item, true);
                        }
                    }
                }
            } catch (Exception ex)
            {

            }
        }

        private void ProcessIStackItem(IStackItem item, bool silenceEvents = false)
        {
            if (!item.IsProcessed)
            {
                item.Process();
                item.IsProcessed = true;
                if (!silenceEvents)
                {
                    ItemProcessed?.BeginInvoke(this, item, null, null);
                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                StackProcessed?.BeginInvoke(this, stackItemCollection, null, null);
            }
        }
        #endregion

        #region Public Methdos
        /// <summary>
        /// Erase all data in the stack.
        /// </summary>
        public void ClearStack()
        {
            LIFOStack.Clear();
            stackItemCollection.Clear();
        }


        /// <summary>
        /// Start the background thread which will consume the IStackItems as they come in. 
        /// </summary>
        public void StartExecution()
        {
            finishingExecution = false;
            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Abort the background thread and do not process any of the data in the stack
        /// </summary>
        public void AbortExecution()
        {
            backgroundWorker.CancelAsync();
            manualResetEvent.Set();
        }

        /// <summary>
        ///  Call this method to Finish processing the stack of data. This assumes no more items will be added to the stack.
        /// </summary>
        public void FinishExecution()
        {
            if (!finishingExecution)
            {
                finishingExecution = true;
                manualResetEvent.Set();
            }
        }

        /// <summary>
        /// Add a stackItem to the stack.
        /// </summary>
        /// <param name="stackObject"></param>
        public void Add(IStackItem stackItem)
        {
            if (!finishingExecution)
            {
                LIFOStack.Push(stackItem);
                stackItemCollection.Add(stackItem);
                manualResetEvent.Set();
            }
        }
        #endregion
    }
}
