using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public class BehaviorTreeBuilder<TTree> where TTree : BehaviorTree
    {
        public class AlreadyInRootException : Exception
        {
            public AlreadyInRootException(Type type) : base($"Error building the tree {type.Name} in method, can not go Up, already in root.") { }
        }
        public class TaskRequiresADifferentTree : Exception
        {
            public TaskRequiresADifferentTree(Type taskType, Type treeType, Type taskTreeType) : base($"The task {taskType} requires a tree of type {taskTreeType.Name}, while {treeType.Name} was received") { }
        }
        public TTree TreeBeingBuild { get; set; }
        private Stack<BTControlNode> _previousNodes;
        private BTControlNode _currentNode;
        internal BehaviorTreeBuilder(TTree tree)
        {
            TreeBeingBuild = tree;
            _currentNode = new Sequence(TreeBeingBuild);
            _previousNodes = new();
            TreeBeingBuild.RootNode = _currentNode;
        }
        private void SetDecorator<DecoratorTree>(BTDecorator<DecoratorTree> decorator) where DecoratorTree : BehaviorTree
        {
            DecoratorTree? treeAsDecoratorTree = TreeBeingBuild as DecoratorTree ?? throw new TaskRequiresADifferentTree(decorator.GetType(), typeof(TTree), typeof(DecoratorTree));
            decorator.Tree = treeAsDecoratorTree;
            decorator.NodeBeingDecoracted = _currentNode;
            decorator.CreateListener();
        }
        public BehaviorTreeBuilder<TTree> AddSequence<DecoratorTree>(string name, BTDecorator<DecoratorTree>? decorator = null) where DecoratorTree : BehaviorTree
        {
            Sequence sequence = new(TreeBeingBuild, new(), decorator);
            _currentNode.AddChild(sequence);
            _previousNodes.Push(_currentNode);
            _currentNode = sequence;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddSequence(string name)
        {
            return AddSequence<TTree>(name);
        }
        public BehaviorTreeBuilder<TTree> AddSelector<DecoratorTree>(string name, BTDecorator<DecoratorTree>? decorator = null) where DecoratorTree : BehaviorTree
        {
            Selector selector = new(TreeBeingBuild, new(), decorator);
            _currentNode.AddChild(selector); 
            _previousNodes.Push(_currentNode);
            _currentNode = selector;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddSelector(string name)
        {
            return AddSelector<TTree>(name);
        }
        public BehaviorTreeBuilder<TTree> AddTask<TaskTree>(BTTask<TaskTree> task) where TaskTree : BehaviorTree
        {
            TaskTree? treeAsTaskTree = TreeBeingBuild as TaskTree ?? throw new TaskRequiresADifferentTree(task.GetType(), typeof(TTree), typeof(TaskTree));
            _currentNode.AddChild(task);
            task.Tree = treeAsTaskTree;
            return this;
        }
        public BehaviorTreeBuilder<TTree> Up()
        {
            if (_previousNodes.Count == 0) throw new AlreadyInRootException(typeof(TTree));
            _currentNode = _previousNodes.Pop();
            return this;
        }
        public TTree Finish()
        {
            return TreeBeingBuild;
        }
    }
}
