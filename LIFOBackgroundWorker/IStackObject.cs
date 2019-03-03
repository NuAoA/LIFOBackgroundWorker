namespace LIFOBackgroundWorker
{
    public interface IStackObject
    {
        void Process();
        bool IsProcessed { get; set; }
    }
}