﻿using BehaviorTrees.Nodes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public abstract class BehaviorTree : IDisposable
    {

        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;
        private readonly int rootEvaluationDelay = 2000; // miliseconds

        public BTControlNode CurrentControlNode { get; set; }
        public BTControlNode RootNode { get; set; }
        public BehaviorTree(int rootEvaluationDelay = 2000)
        {
            this.rootEvaluationDelay = rootEvaluationDelay;
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
                    await Task.Delay(rootEvaluationDelay);
                }
                catch (OperationCanceledException) { }
            }
        }
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
        public static BehaviorTree? BuildTree(object[] objects)
        {
            throw new NotImplementedException("Derived classes must implement the BuildTree method.");
        }
    }
}
