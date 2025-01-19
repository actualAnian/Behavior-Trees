using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BehaviorTrees.Nodes;
using TaleWorlds.Library;

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
        public bool IsSuccessful()
        {
            return !_hasError;
        }
        bool _hasError = false;
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
        private void SetDecorator<DDecorator>(DDecorator decorator) where DDecorator : BTDecorator
        {
            CopyAllInterfacesProperties(decorator);
            decorator.Tree = TreeBeingBuild;
            decorator.NodeBeingDecoracted = _currentNode;
            decorator.CreateListener();
        }
        public BehaviorTreeBuilder<TTree> AddSequence<DDecorator>(string name, DDecorator? decorator = null) where DDecorator : BTDecorator
        {
            if (_hasError) return this;
            Sequence sequence = new(TreeBeingBuild, new(), decorator);
            _currentNode.AddChild(sequence);
            _previousNodes.Push(_currentNode);
            _currentNode = sequence;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddSequence(string name)
        {
            return AddSequence<BTDecorator>(name, null);
        }

        public BehaviorTreeBuilder<TTree> AddSelector<DDecorator>(string name, DDecorator? decorator = null) where DDecorator : BTDecorator
        {
            if (_hasError) return this;
            Selector selector = new(TreeBeingBuild, new(), decorator);
            _currentNode.AddChild(selector); 
            _previousNodes.Push(_currentNode);
            _currentNode = selector;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddSelector(string name)
        {
            return AddSelector<BTDecorator>(name, null);
        }


        //public BehaviorTreeBuilder<TTree> AddTask<TaskTree>(BTTask<TaskTree> task) where TaskTree : BehaviorTree
        //{
        //    TaskTree? treeAsTaskTree = TreeBeingBuild as TaskTree ?? throw new TaskRequiresADifferentTree(task.GetType(), typeof(TTree), typeof(TaskTree));
        //    _currentNode.AddChild(task);
        //    task.Tree = treeAsTaskTree;
        //    return this;
        //}
        public BehaviorTreeBuilder<TTree> Up()
        {
            if (_previousNodes.Count == 0) throw new AlreadyInRootException(typeof(TTree));
            _currentNode = _previousNodes.Pop();
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddTask<TTask>(TTask task) where TTask : BTTask
        {
            if (_hasError) return this;
            //Type[] treeInterfaces = typeof(TTree).GetInterfaces();
            //Type[] taskInterfaces = typeof(TTask).GetInterfaces();

            //foreach (Type? interfaceType in taskInterfaces.Intersect(treeInterfaces))
            //{
            //    if (!typeof(IBTBlackboard).IsAssignableFrom(interfaceType) || interfaceType is IBTBlackboard) continue;
            //    CopyInterfaceProperties(TreeBeingBuild, task, interfaceType);
            //}
            CopyAllInterfacesProperties(task);
            _currentNode.AddChild(task);
            return this;
        }
        private void CopyAllInterfacesProperties<AssignableObject>(AssignableObject node)
        {
            Type[] nodeInterfaces = typeof(AssignableObject).GetInterfaces();
            Type[] treeInterfaces = typeof(TTree).GetInterfaces();
            foreach (Type? interfaceType in nodeInterfaces)
            {
                if (!typeof(IBTBlackboard).IsAssignableFrom(interfaceType) || interfaceType is IBTBlackboard) continue;
                if (!treeInterfaces.Contains(interfaceType))
                {
                    _hasError = true;
                    InformationManager.DisplayMessage(new InformationMessage($"error creating tree {TreeBeingBuild.GetType().Name}. The tree does not implement the interface {interfaceType.Name}, but {node.GetType().Name} does."));
                    return;
                }
                CopyInterfaceProperties(TreeBeingBuild, node, interfaceType);
            }

        }
        private static void CopyInterfaceProperties(BehaviorTree source, object target, Type interfaceType)
        {
            var properties = interfaceType.GetProperties();
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var value = property.GetValue(source);
                    property.SetValue(target, value);
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage($"error creating tree {source.GetType().Name}. The property {property.Name} can not be assigned. Is it missing get or set methods?"));
                }
            }
        }
        public TTree? Finish()
        {
            if (_hasError) return null;
            return TreeBeingBuild;
        }
    }
}
