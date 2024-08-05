using System.Threading.Tasks;

namespace BehaviorTree
{
    public abstract class BTListener
    {
        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
        public INotifiable NotifiedObject { get; set; }

        protected BTListener(INotifiable notifiedObject)
        {
            NotifiedObject = notifiedObject;
        }

        public abstract void Subscribe();
        public abstract void UnSubscribe();
        public void Signal()
        {
            _tcs.TrySetResult(true);
            _tcs = new TaskCompletionSource<bool>();
        }
        public Task<bool> NotifyAsync()
        {
            return _tcs.Task;
        }
    }
    //public interface BTDecoratorListener : IBTListener
    //{
    //    public BTDecorator Decorator { get; set; }
    //}

    public interface INotifiable
    {
        public BehaviorTree NotifiedTree { get; }
        public void Notify();
    }
}
