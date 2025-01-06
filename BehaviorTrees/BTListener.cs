using System.Collections.Generic;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public abstract class BTListener : INotifiable
    {
        private TaskCompletionSource<bool> _tcs = new();
        public BehaviorTree Tree { get; private set; }
        public INotifiable Notifies { get; private set; }
        protected BTListener(BehaviorTree tree, INotifiable notifies)
        {
            Notifies = notifies;
            Tree = tree;
        }

        public virtual void Subscribe() //@TODO make this obligatory
        {
            _tcs = new();
        }
        public abstract void UnSubscribe();
        public abstract void Notify(List<object> data);
        public void Signal(bool success)
        {
            if (success)
                _tcs.TrySetResult(true);
            else
                _tcs.TrySetCanceled();
            //_tcs = new TaskCompletionSource<bool>();
        }
        public Task<bool> NotifyAsync()
        {
            return _tcs.Task;
        }
    }
    public interface INotifiable
    {
        public void Notify(List<object> data);
    }
}
