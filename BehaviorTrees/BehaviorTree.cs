using BehaviorTrees.Nodes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public enum BTStatus
    {
        Running,
        FinishedWithFalse,
        FinishedWithTrue,
    }
    public abstract class BehaviorTree : IDisposable
    {

        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;
        private readonly int rootEvaluationDelay = 2000; // miliseconds

        internal BTNode CurrentNode { get; set; }
        internal BTNode RootNode { get; set; }
        public BehaviorTree(int rootEvaluationDelay = 2000)
        {
            this.rootEvaluationDelay = rootEvaluationDelay;
        }
        public void RunTree()
        {
            if (CurrentNode.Decorator != null && !CurrentNode.Decorator.Evaluate()) return;
            CurrentNode = CurrentNode.Execute();

            while (CurrentNode != RootNode)
            {
                CurrentNode = CurrentNode.Execute();
            }
            // if running, do nothing
        }
        //public void StartTree()
        //{
        //    CurrentControlNode = RootNode;
        //    _cancellationTokenSource = new CancellationTokenSource();
        //    ExecuteNode(_cancellationTokenSource.Token);
        //}
        //private async void ExecuteNode(CancellationToken cancellationToken)
        //{
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            await RootNode.Execute(cancellationToken);
        //            await Task.Delay(rootEvaluationDelay);
        //        }
        //        catch (OperationCanceledException) { }
        //    }
        //}
        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource?.Cancel();
                _disposed = true;
            }
        }
        protected static BehaviorTreeBuilder<TTree> StartBuildingTree<TTree>(TTree tree) where TTree : BehaviorTree
        {
            BehaviorTreeBuilder<TTree> newBuilder = new(tree);
            return newBuilder;
        }
        /// <summary>
        /// Should start with StartBuildingTree, afterwards build nodes, tasks, up, end with Finish
        /// </summary>
        /// <param name="objects"></param>
        /// <returns>Returns the built tree.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static BehaviorTree? BuildTree(object[] objects)
        {
            throw new NotImplementedException("Derived classes must implement the BuildTree method.");
        }
    }
}
