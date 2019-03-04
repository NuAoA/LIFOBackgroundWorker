using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace LIFOBackgroundWorker.Tests
{
    [TestClass]
    public class LifoBackgroundWorkerTests
    {      

        [TestMethod]
        public void StartExecution()
        {
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();
            worker.StartExecution();

        }

        [TestMethod]
        public void AddItemToStackAddsItemToList()
        {
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();
            worker.StartExecution();
            worker.Add(new StackItem() { ID = 1 });
            Assert.AreEqual(1, worker.StackItemCollection.Count);
            worker.FinishExecution();
        }
        [TestMethod]
        public void AddItemToStack_isProcessed()
        {
            ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim();
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();
            worker.ItemProcessed += (s, item) =>
            {
                Assert.AreEqual(1, (item as StackItem).ID);
                manualResetEventSlim.Set();
            };

            worker.StartExecution();
            worker.Add(new StackItem() { ID = 1 });

            if (!manualResetEventSlim.Wait(10000))
            {
                Assert.Fail();
            }
            worker.FinishExecution();
        }

        [TestMethod]
        public void AddItemToStackThenStartProcess_isProcessed()
        {
            ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim();
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();
            worker.ItemProcessed += (s, item) =>
            {
                Assert.AreEqual(1, (item as StackItem).ID);
                manualResetEventSlim.Set();
            };

            worker.Add(new StackItem() { ID = 1 });
            worker.StartExecution();
            if (!manualResetEventSlim.Wait(10000))
            {
                Assert.Fail();
            }
            worker.FinishExecution();
        }

        [TestMethod]
        public void finishExecution_EventIsFired()
        {
            ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim();
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();
            worker.StackProcessed += (s, items) =>
            {                
                manualResetEventSlim.Set();
            };

            worker.StartExecution();
            worker.FinishExecution();

            if (!manualResetEventSlim.Wait(10000))
            {
                Assert.Fail(); // Event was not fired
            }
        }

        [TestMethod]
        public void finishExecution_EventIsFiredWithData()
        {
            ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim();
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();
            worker.StackProcessed += (s, items) =>
            {
                Assert.AreEqual(6, items.Count);
                foreach(var item in items)
                {
                    Assert.AreEqual(true, item.IsProcessed);
                }
                manualResetEventSlim.Set();
            };

            worker.StartExecution();
            worker.Add(new StackItem() { ID = 1 });
            worker.Add(new StackItem() { ID = 2 });
            worker.Add(new StackItem() { ID = 3 });
            worker.Add(new StackItem() { ID = 4 });
            worker.Add(new StackItem() { ID = 5 });
            worker.Add(new StackItem() { ID = 6 });
            worker.FinishExecution();

            if (!manualResetEventSlim.Wait(10000))
            {
                Assert.Fail(); // Event was not fired
            }
        }

        [TestMethod]
        public void abortExecution_aborts()
        {
            ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim();
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();

            worker.StackProcessed += (s, items) =>
            {
                manualResetEventSlim.Set();
            };

            worker.StartExecution();
            worker.AbortExecution();

            if (manualResetEventSlim.Wait(2000))
            {
                Assert.Fail(); // Event was fired, it shouldnt be on abort
            }
        }

        [TestMethod]
        public void ClearStackClearsStack()
        {
            ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim();
            LIFOBackgroundWorker worker = new LIFOBackgroundWorker();
            worker.StackProcessed += (s, items) =>
            {
                Assert.AreEqual(2, items.Count);
                foreach (var item in items)
                {
                    Assert.AreEqual(true, item.IsProcessed);
                }
                manualResetEventSlim.Set();
            };

            worker.StartExecution();
            worker.Add(new StackItem() { ID = 1 });
            worker.Add(new StackItem() { ID = 2 });
            worker.Add(new StackItem() { ID = 3 });
            worker.Add(new StackItem() { ID = 4 });
            worker.Add(new StackItem() { ID = 5 });
            worker.Add(new StackItem() { ID = 6 });
            worker.ClearStack();
            worker.Add(new StackItem() { ID = 1 });
            worker.Add(new StackItem() { ID = 2 });
            worker.FinishExecution();

            if (!manualResetEventSlim.Wait(10000))
            {
                Assert.Fail(); // Event was not fired
            }
        }
    }
}
