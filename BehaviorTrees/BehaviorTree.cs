using BehaviorTrees;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public interface IBTTask<out TaskTree>
    {
        void Execute();
    }
    public abstract class BehaviorTree : IDisposable
    {

        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;

        public List<BTListener> currentListeners;
        public BTControlNode CurrentControlNode { get; set; }
        public BTControlNode RootNode { get; set; }
        public BehaviorTree()
        { 
        }
        public void StartTree()
        {
            CurrentControlNode = RootNode;
            _cancellationTokenSource = new CancellationTokenSource();
            ExecuteNode(_cancellationTokenSource.Token);
        }
        public async void ExecuteNode(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RootNode.Execute(cancellationToken);
                    await Task.Delay(2000);
                }
                catch (OperationCanceledException)
                {

                }
            }
        }
        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource?.Cancel();
                //CurrentControlNode.ClearTasks();
                //CurrentControlNode.RemoveDecorators();
                _disposed = true;
            }
        }
        public static BehaviorTreeBuilder<TTree> BuildTree<TTree>(TTree tree) where TTree : BehaviorTree
        {
            BehaviorTreeBuilder<TTree> newBuilder = new(tree);

            return newBuilder;
        }
    }
}
