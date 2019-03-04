using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIFOBackgroundWorker;

namespace LIFOBackgroundWorkerWPF
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _interval;

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; InvokePropertyChanged(); }
        }

        private bool IsProcessing;

        public bool _isProcessing
        {
            get { return IsProcessing; }
            set { IsProcessing = value; InvokePropertyChanged(); }
        }

        private bool? _isthreaded;

        public bool? Isthreaded
        {
            get { if (Worker != null) return Worker.UseMultithreading; else return true; }
            set { _isthreaded = value; if (Worker != null) Worker.UseMultithreading = (bool)value; InvokePropertyChanged(); }
        }

        public ObservableCollection<StackItem> ListBoxData
        {
            get
            {
                if (Worker==null)
                {
                    return new ObservableCollection<StackItem>();
                }
                return new ObservableCollection<StackItem>(Worker.StackItemCollection.Cast<StackItem>());
            }

        }

        private LIFOBackgroundWorker.LIFOBackgroundWorker Worker { get; set; }

        public ViewModel()
        {
            Worker = new LIFOBackgroundWorker.LIFOBackgroundWorker();
            Interval = 2500;
        }

        public void AddStackItem()
        {
            Worker.Add(new StackItem(Interval));
            InvokePropertyChanged("ListBoxData");
        }

        internal void start()
        {
            Worker.StartExecution();
            IsProcessing = true;
        }

        internal void stop()
        {
            Worker.FinishExecution();
            IsProcessing = false;
        }

        private void InvokePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void clear()
        {
            Worker.ClearStack();
            InvokePropertyChanged("ListBoxData");
        }
    }
}
