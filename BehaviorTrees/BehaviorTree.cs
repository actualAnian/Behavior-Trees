using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public abstract class BehaviorTree : INotifiable, IDisposable
    {

        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;

        public List<BTListener> currentListeners;
        public BTControlNode CurrentControlNode { get; set; }
        public BTControlNode RootNode { get; set; }

        public BehaviorTree NotifiedTree { get { return this; } }

        public BehaviorTree()
        {
        }
        public BehaviorTree BuildTree(BTControlNode rootNode, BTListener listener)
        {
            CurrentControlNode = RootNode = rootNode;
            //this.listener = listener;
            //listener.Subscribe();
            _cancellationTokenSource = new CancellationTokenSource();
            ExecuteNode(_cancellationTokenSource.Token);
            return this;
        }
        public BehaviorTree BuildTree(BTControlNode rootNode)
        {
            CurrentControlNode = RootNode = rootNode;
            _cancellationTokenSource = new CancellationTokenSource();
            ExecuteNode(_cancellationTokenSource.Token);
            return this;
        }
        public async void ExecuteNode(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) //@TODO check if this works
            {
                await RootNode.Execute();
                await Task.Delay(2000);
                int a = 5;
            }
        }
        public void NotifyOnPropertyChange()
        {
            CurrentControlNode.Reevaluate();
        }
        public abstract void Notify();

        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                CurrentControlNode.ClearTasks();
                CurrentControlNode.RemoveDecorators();
                _disposed = true;
            }
        }
    }
}
