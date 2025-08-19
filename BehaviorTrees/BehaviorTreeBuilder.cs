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
        internal class AlreadyInRootException : Exception
        {
            public AlreadyInRootException(Type type) : base($"Error building the tree {type.Name} in method, can not go Up, already in root.") { }
        }
        internal class MissingTreeBlackBoardException : Exception
        {
            public MissingTreeBlackBoardException(BehaviorTree TreeBeingBuild, Type interfaceType, object node) : 
                base($"error creating tree {TreeBeingBuild.GetType().Name}. The tree does not implement the interface {interfaceType.Name}, but {node.GetType().Name} does.") { }
        }
        internal class IncorrectPropertyException : Exception
        {
            public IncorrectPropertyException(BehaviorTree source, PropertyInfo property) :
                base($"error creating tree {source.GetType().Name}. The property {property.Name} can not be assigned. Is it missing get or set methods?")
            { }
        }
        internal class CanNotCreateSubTreeException : Exception
        {
            public CanNotCreateSubTreeException(string treeName) : base($"Can not create a subtree of type {treeName}") { }
        }
        private TTree TreeBeingBuild { get; set; }
        private Stack<BTControlNode> _previousNodes;
        private BTControlNode _currentNode;
        internal BehaviorTreeBuilder(TTree tree)
        {
            TreeBeingBuild = tree;
            _currentNode = new Sequence(TreeBeingBuild, typeof(TTree).Name.ToString()); //@TODO switch to selector
            _previousNodes = new();
            TreeBeingBuild.RootNode = _currentNode;
            TreeBeingBuild.CurrentNode = _currentNode;
            _currentNode.Parent = _currentNode;
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
        /// <summary>
        /// Adds a sequence control node to the tree. The sequence moves through children from left to right, until one of the children returns a false
        /// If a sequence encounters an unfulfilled event decorator, it will wait until the event is called and the decorator condition is true
        /// When encountering a return-false decorator, the sequence will immediately return with false
        /// </summary>
        /// <typeparam name="DDecorator"></typeparam>
        /// <param name="name"> A string, used to make the tree more readable.</param>
        /// <param name="decorator"> A decorator, accepts both return-false decorators, and await event decorators.</param>
        /// <param name="weight">Weight of the node, useful if it is a child of the RandomSelector, a chance that this child is chosen.</param>
        /// <returns>Returns the builder.</returns>
        public BehaviorTreeBuilder<TTree> AddSequence<DDecorator>(string name, DDecorator? decorator = null, int weight = 100) where DDecorator : AbstractDecorator
        {
            Sequence sequence = new(TreeBeingBuild, name, new(), decorator, weight);
            _currentNode.AddChild(sequence);
            _previousNodes.Push(_currentNode);
            sequence.Parent = _currentNode;
            _currentNode = sequence;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }
        /// <summary>
        /// Adds a sequence control node to the tree. The sequence moves through children from left to right, until one of the children returns a false
        /// If a sequence encounters an unfulfilled event decorator, it will wait until the event is called and the decorator condition is true
        /// When encountering a return-false decorator, the sequence will immediately return with false
        /// </summary>
        /// <param name="name">A string, used to make the tree more readable.</param>
        /// <param name="weight">Weight of the node, useful if it is a child of the RandomSelector, a chance that this child is chosen.</param>
        /// <returns>Returns the builder.</returns>
        public BehaviorTreeBuilder<TTree> AddSequence(string name, int weight = 100)
        {
            return AddSequence<BTEventDecorator>(name, null, weight);
        }
        /// <summary>
        /// Adds a selector control node to the tree. The selector moves through all executable (no decorator or decorator returning true) children from left to right, until one of the children return a true.
        /// If the selector moved through all the executable children, and none returned true, the selector adds listeners to all event decorators, and waits till one of the event occurs, and the decorator returns true.
        /// If the child returns false, it is removed from the waiting list.
        /// This process occurs until there are no more decorators waiting for events, at which point the selector returns false.
        /// </summary>
        /// <typeparam name="DDecorator"></typeparam>
        /// <param name="name">A string, used to make the tree more readable.</param>
        /// <param name="decorator"> A decorator, accepts both return-false decorators, and await event decorators.</param>
        /// <param name="weight">Weight of the node, useful if it is a child of the RandomSelector, a chance that this child is chosen.</param>
        /// <returns>Returns the builder.</returns>

        public BehaviorTreeBuilder<TTree> AddSelector<DDecorator>(string name, DDecorator? decorator = null, int weight = 100) where DDecorator : AbstractDecorator
        {
            Selector selector = new(TreeBeingBuild, name, new(), decorator, weight);
            _currentNode.AddChild(selector);
            _previousNodes.Push(_currentNode);
            selector.Parent = _currentNode;
            _currentNode = selector;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }

        /// <summary>
        /// Adds a selector control node to the tree. The selector moves through all executable (no decorator or decorator returning true) children from left to right, until one of the children return a true.
        /// If the selector moved through all the executable children, and none returned true, the selector adds listeners to all event decorators, and waits till one of the event occurs, and the decorator returns true.
        /// If the child returns false, it is removed from the waiting list.
        /// This process occurs until there are no more decorators waiting for events, at which point the selector returns false.
        /// </summary>
        /// <param name="name">A string, used to make the tree more readable.</param>
        /// <param name="weight">Weight of the node, useful if it is a child of the RandomSelector, a chance that this child is chosen.</param>
        /// <returns>Returns the builder.</returns>

        public BehaviorTreeBuilder<TTree> AddSelector(string name, int weight = 100)
        {
            return AddSelector<BTEventDecorator>(name, null, weight);
        }

        /// <summary>
        /// A random selector control node, chooses which child to execute based on the weight.
        /// Ignores decorators awaiting events, can only execute children whose decorators return true
        /// </summary>
        /// <typeparam name="DDecorator"></typeparam>
        /// <param name="name">A string, used to make the tree more readable.</param>
        /// <param name="decorator">Treats return-false and await event decorators the same way, only looks if the decorator currently return true.</param>
        /// <param name="weight">Weight of the node, useful if it is a child of the RandomSelector, a chance that this child is chosen.</param>
        /// <returns>Returns the builder.</returns>

        public BehaviorTreeBuilder<TTree> AddRandomSelector<DDecorator>(string name, DDecorator? decorator = null, int weight = 100) where DDecorator : AbstractDecorator
        {
            RandomSelector randomSelector = new(TreeBeingBuild, name, decorator, new(), weight);
            _currentNode.AddChild(randomSelector);
            _previousNodes.Push(_currentNode);
            randomSelector.Parent = _currentNode;
            _currentNode = randomSelector;
            if (decorator != null) SetDecorator(decorator);
            return this;
        }

        /// <summary>
        /// Adds a behavior tree from the register as a node to tree being currently built.
        /// The subtree has to be registered with BTRegister.RegisterClass first.
        /// </summary>
        /// <param name="subTreeName">Name of the tree in the register.</param>
        /// <param name="weight">Weight of the node, useful if it is a child of the RandomSelector, a chance that this child is chosen.</param>
        /// <param name="args">Arguments that the sub tree needs to be built.</param>
        /// <returns>Returns the builder.</returns>
        /// <exception cref="CanNotCreateSubTreeException"></exception>
        public BehaviorTreeBuilder<TTree> AddSubTree(string subTreeName, int weight = 100, params object[] args)
        {
            BehaviorTree? subTree;
            subTree = BTRegister.Build(subTreeName, args);
            if (subTree == null)
                throw new CanNotCreateSubTreeException(subTreeName);
            subTree.RootNode.weight = weight;
            subTree.RootNode.Parent = _currentNode;
            _currentNode.AddChild(subTree.RootNode);
            return this;
        }
        /// <summary>
        /// Used to finish building the current sequence, selector or random selector.
        /// </summary>
        /// <returns>Returns the builder</returns>
        /// <exception cref="AlreadyInRootException"></exception>
        public BehaviorTreeBuilder<TTree> Up()
        {
            if (_previousNodes.Count == 0) throw new AlreadyInRootException(typeof(TTree));
            _currentNode = _previousNodes.Pop();
            return this;
        }
        /// <summary>
        /// Adds a task to the tree.
        /// </summary>
        /// <typeparam name="TTask">Has to be a class inheriting after BTTask.</typeparam>
        /// <param name="task">Has to be an object of a class inheriting after BTTask</param>
        /// <returns>Returns the builder.</returns>
        public BehaviorTreeBuilder<TTree> AddTask<TTask>(TTask task) where TTask : BTTask
        {
            CopyAllInterfacesProperties(task);
            _currentNode.AddChild(task);
            task.Parent = _currentNode;
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
                    throw new MissingTreeBlackBoardException(TreeBeingBuild, interfaceType, node);
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
        /// <summary>
        /// Finish building the tree.
        /// </summary>
        /// <returns>The tree object</returns>
        public TTree? Finish()
        {
            return TreeBeingBuild;
        }
    }
}
