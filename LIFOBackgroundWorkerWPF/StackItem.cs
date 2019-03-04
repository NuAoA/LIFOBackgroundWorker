using LIFOBackgroundWorker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIFOBackgroundWorkerWPF
{
    public class StackItem : IStackItem,INotifyPropertyChanged
    {
        public bool IsProcessed { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private double _progress;

        public double Progress
        {
            get { return _progress; }
            set { _progress = value; InvokePropertyChanged(); }
        }


        public int ProcessingTime;

        public StackItem(int millisecondProcessingTime)
        {
            ProcessingTime = millisecondProcessingTime;
        }
        public void Process()
        {
            for(int i=0;i<ProcessingTime;i+=10)
            {
                Progress = ((double)i / (double)ProcessingTime)*100;
                System.Threading.Thread.Sleep(10);
            }
            Progress = 100;
        }

        private void InvokePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
