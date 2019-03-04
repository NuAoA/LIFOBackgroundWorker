using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LIFOBackgroundWorker.Tests
{
    class StackItem : IStackItem
    {

        public bool IsProcessed { get; set; } = false;

        public int ID { get; set; }

        public void Process()
        {
            System.Threading.Thread.Sleep(100);
            IsProcessed = true;
            System.Diagnostics.Debug.WriteLine($"{ID} has been processed on Thread : {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
