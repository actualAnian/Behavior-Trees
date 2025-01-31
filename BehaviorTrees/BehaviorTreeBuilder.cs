using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorTrees.Nodes;

namespace BehaviorTrees
{
    public class IncorrectParametersException : Exception
    {
        public IncorrectParametersException(Type tree) : base($"Can not create a tree {tree.Name}, received incorrect parameters") { }
    }

    public class BehaviorTreeBuilder<TTree> where TTree : BehaviorTree
    {
        public class AlreadyInRootException : Exception
        {
            public AlreadyInRootException(Type type) : base($"Error building the tree {type.Name} in method, can not go Up, already in root.") { }
        }
        public class MissingTreeBlackBoardException : Exception
        {
            public MissingTreeBlackBoardException(BehaviorTree TreeBeingBuild, Type interfaceType, object node) : 
                base($"error creating tree {TreeBeingBuild.GetType().Name}. The tree does not implement the interface {interfaceType.Name}, but {node.GetType().Name} does.") { }
        }
        public class IncorrectPropertyException : Exception
        {
            public IncorrectPropertyException(BehaviorTree source, PropertyInfo property) :
                base($"error creating tree {source.GetType().Name}. The property {property.Name} can not be assigned. Is it missing get or set methods?")
            { }
        }
        public class CanNotCreateSubTreeException : Exception
        {
            public CanNotCreateSubTreeException(string treeName) : base($"Can not create a subtree of type {treeName}") { }
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
            _currentNode = new Selector(TreeBeingBuild);
            _previousNodes = new();
            TreeBeingBuild.RootNode = _currentNode;
        }
        private void SetDecorator<DDecorator>(DDecorator decorator) where DDecorator : AbstractDecorator
        {
            CopyAllInterfacesProperties(decorator);
            if (decorator is BTEventDecorator eventDecorator)
            {
                eventDecorator.Tree = TreeBeingBuild;
                eventDecorator.CreateListener();
            }
            decorator.NodeBeingDecoracted = _currentNode;
        }
        public BehaviorTreeBuilder<TTree> AddSequence<DDecorator>(string name, DDecorator? decorator = null, int weight = 100) where DDecorator : AbstractDecorator
        {
            //if (_hasError) return this;
            Sequence sequence = new(TreeBeingBuild, new(), decorator, weight);
            _currentNode.AddChild(sequence);
            _previousNodes.Push(_currentNode);
            _currentNode = sequence;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddSequence(string name, int weight = 100)
        {
            return AddSequence<BTEventDecorator>(name, null, weight);
        }

        public BehaviorTreeBuilder<TTree> AddSelector<DDecorator>(string name, DDecorator? decorator = null, int weight = 100) where DDecorator : BTEventDecorator
        {
            //if (_hasError) return this;
            Selector selector = new(TreeBeingBuild, new(), decorator, weight);
            _currentNode.AddChild(selector); 
            _previousNodes.Push(_currentNode);
            _currentNode = selector;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddSelector(string name, int weight = 100)
        {
            return AddSelector<BTEventDecorator>(name, null, weight);
        }
        public BehaviorTreeBuilder<TTree> AddRandomSelector<DDecorator>(string name, DDecorator? decorator = null, int weight = 100) where DDecorator : BTEventDecorator
        {
            //if (_hasError) return this;
            RandomSelector randomSelector = new(TreeBeingBuild, decorator, new(), weight);
            _currentNode.AddChild(randomSelector);
            _previousNodes.Push(_currentNode);
            _currentNode = randomSelector;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddSubTree(string subTreeName, int weight = 100, params object[] args)
        {
            //if (!_hasError) return this;
            BehaviorTree? subTree;
            subTree = BTRegister.Build(subTreeName, args);
            if (subTree == null)
            {
                throw new CanNotCreateSubTreeException(subTreeName);
                //_hasError = true;
                //return this;
            }
            _currentNode.AddChild(subTree.RootNode);
            return this;
        }
        public BehaviorTreeBuilder<TTree> Up()
        {
            //if (!_hasError) return this;
            if (_previousNodes.Count == 0) throw new AlreadyInRootException(typeof(TTree));
            _currentNode = _previousNodes.Pop();
            return this;
        }
        public BehaviorTreeBuilder<TTree> AddTask<TTask>(TTask task) where TTask : BTTask
        {
            //if (_hasError) return this;
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
                    throw new MissingTreeBlackBoardException(TreeBeingBuild, interfaceType, node);
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
                    throw new IncorrectPropertyException(source, property);
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
