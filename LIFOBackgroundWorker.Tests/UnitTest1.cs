using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LIFOBackgroundWorker.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            LIFOBackgroundWorker lbw = new LIFOBackgroundWorker();
            ManualResetEvent finsihed = new ManualResetEvent(false);
            lbw.StackProcessed += (S, args) =>
            {
                Assert.AreEqual(6, args.Count);
                finsihed.Set();
            };
            lbw.StartExecution();
            System.Threading.Thread.Sleep(10);
            lbw.Add(new StackObject() { ID = 1 });
            lbw.Add(new StackObject() { ID = 2 });
            lbw.Add(new StackObject() { ID = 3 });
            System.Threading.Thread.Sleep(1000);
            lbw.Add(new StackObject() { ID = 4 });
            System.Threading.Thread.Sleep(1000);
            lbw.Add(new StackObject() { ID = 5 });
            lbw.Add(new StackObject() { ID = 6 });
            lbw.FinishExecution();

            finsihed.WaitOne();
        }
    }
}
