using BehaviorTrees.Nodes;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BehaviorTrees
{
    public enum BTStatus
    {
        Running,
        FinishedWithFalse,
        FinishedWithTrue,
        WaitingForEvent,
        ReceivedEvent,
        NotExecuted
    }
    public enum BTTaskStatus
    {
        Running,
        FinishedWithFalse,
        FinishedWithTrue,
    }
    public abstract class BehaviorTree
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;
        public readonly int rootEvaluationDelay = 2000; // miliseconds

        public bool ShouldRunNextTick { get; set; }
        internal BTNode CurrentNode { get; set; }
        internal BTNode RootNode { get; set; }
        internal BTNode? NodeReceivingEvent { get; set; }
        internal List<BTControlNode> ControlNodes { get; } = new();
        public BehaviorTree(int rootEvaluationDelay = 2000)
        {
            this.rootEvaluationDelay = rootEvaluationDelay;
        }
        public void RunTree()
        {
            if (CurrentNode.Status == BTStatus.NotExecuted) // will only be hit when the tree is in root
                CurrentNode.HandleExecute();
            if (CurrentNode.ShouldExitTree())
                return;
            do
            {
                CurrentNode = CurrentNode.HandleExecute();

                if (CurrentNode.ShouldExitTree())
                    return;

            } while (CurrentNode != RootNode);

            if (CurrentNode == RootNode && RootNode is BTControlNode controlRoot)
            {
                controlRoot.ResetChildren();
                CurrentNode.Status = BTStatus.NotExecuted;
            }
        }
        //public void Dispose()
        //{
        //    if (!_disposed)
        //    {
        //        _disposed = true;
        //    }
        //}
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
