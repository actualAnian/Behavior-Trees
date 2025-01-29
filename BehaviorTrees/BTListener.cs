using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public abstract class BTListener
    {
        private TaskCompletionSource<bool> _tcs = new();
        public Task<bool> Task => _tcs.Task;
        public BehaviorTree Tree { get; private set; }
        public INotifiable Notifies { get; private set; }
        protected BTListener(BehaviorTree tree, INotifiable notifies)
        {
            Notifies = notifies;
            Tree = tree;
        }

        public virtual void Subscribe(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
            {
                if (_tcs != null && !_tcs.Task.IsCompleted)
                {
                    _tcs.SetCanceled();
                }
            });
            _tcs = new();
        }
        public abstract void UnSubscribe();

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
        public void Notify(object[] data)
        {
            Notifies.Notify(data);
            Signal(true);
        }
        public void NotifyWithCancel()
        {
            Signal(false);
        }
    }
    public interface INotifiable
    {
        public void Notify(object[] data);
    }
}
