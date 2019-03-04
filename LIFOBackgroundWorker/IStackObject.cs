namespace LIFOBackgroundWorker
{
    public interface IStackItem
    {
        void Process();
        bool IsProcessed { get; set; }
    }
}