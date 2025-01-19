using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BehaviorTrees.Nodes;

namespace BehaviorTrees
{
    public abstract class BTControlNode : BTNode
    {
        protected int alreadyExecutedNodes = 0;
        public List<BTNode> allChildren;
        public List<BTNode> currentlyExecutableChildren = new();
        public List<BTNode> childrenWaiting = new();
        public List<BTNode> childrenWithTasks = new();

        private readonly AbstractDecorator? _decorator;
        public override AbstractDecorator? Decorator => _decorator;
        public List<BTListener> Listeners { get; private set; }
        protected List<Task<bool>> tasks = new();
        protected List<AbstractDecorator> decorators = new();
        
        protected BTControlNode(BehaviorTree tree, AbstractDecorator? decorator = null, List<BTNode>? children = null) : base()
        {
            BaseTree = tree;
            _decorator = decorator;
            allChildren = children ?? new();
        }
        public bool Evaluate()
        {
            return Decorator == null || Decorator.Evaluate();
        }
        internal void AddChild(BTNode child)
        {
            allChildren.Add(child);
        }
        public BTControlNode BuildNode(BTNode nextNode)
        {
            allChildren.Add(nextNode);
            return this;
        }
        Task<bool> _cancellationTask;
        protected Task<bool> GetCancellationTask(CancellationToken cancellationToken)
        {
            _cancellationTask ??= Task.Run(() =>
            {
                cancellationToken.WaitHandle.WaitOne();
                return false;
            });
            return _cancellationTask;
        }
        protected void AddTasks(CancellationToken cancellationToken)
        {
            foreach (BTNode chi in childrenWithTasks)
            {
                if (chi.Decorator == null) continue;
                tasks.Add(chi.AddDecoratorsListeners(cancellationToken));
                decorators.Add(chi.Decorator);
            }
        }
        public override sealed async Task<bool> Execute(CancellationToken cancellationToken)
        {
            alreadyExecutedNodes = 0;
            BaseTree.CurrentControlNode = this;
            bool result = await ExecuteImplementation(cancellationToken);
            RemoveDecorators();
            tasks = new();
            decorators = new();
            childrenWithTasks.Clear();
            currentlyExecutableChildren.Clear();
            childrenWaiting.Clear();
            return result;
        }
        protected abstract Task<bool> ExecuteImplementation(CancellationToken cancellationToken);
        protected void RemoveDecorator(AbstractDecorator decorator)
        {
            decorator.Remove();
        }
        internal void RemoveDecorators()
        {
            childrenWithTasks.ForEach(child => { if (child.Decorator != null) RemoveDecorator(child.Decorator); });
        }
        public void ClearTasks()
        {
            decorators.Clear();
            tasks.Clear();
        }
        protected bool ExecutedAll()
        {
            return alreadyExecutedNodes >= allChildren.Count;
        }
    }
}
