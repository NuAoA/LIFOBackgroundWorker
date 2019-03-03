using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIFOBackgroundWorker.Tests
{
    class StackObject : IStackObject
    {
        public bool IsProcessed { get; set; } = false;

        public int ID { get; set; }

        public void Process()
        {
            System.Threading.Thread.Sleep(1000);
            IsProcessed = true;
            System.Diagnostics.Debug.WriteLine($"{ID} has been processed");
        }
    }
}
