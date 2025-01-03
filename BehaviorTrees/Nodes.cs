using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public abstract class BTNode
    {
        internal BehaviorTree tree;
        public virtual BTDecorator decorator
        { 
            get
            { 
                return null;
            }
        }
        protected BTNode(BehaviorTree tree)
        {
            this.tree = tree;
        }
        public virtual async Task<bool> Execute() { return true; }
        //public void Update()
        //{
        //    decorator?.Update();
        //}
        //public abstract void UpdateTreeProperty();

    }
    public abstract class BTControlNode : BTNode
    {
        public List<BTNode> taskChildren = new();
        public List<BTControlNode> controlChildren = new();
        public List<BTNode> currentlyExecutableChildren = new();
        private BTDecorator? _decorator;
        public override BTDecorator decorator { get { return _decorator; } }
        public List<BTListener> Listeners { get; private set; }
        protected Dictionary<Task<bool>, BTDecorator> tasks = new Dictionary<Task<bool>, BTDecorator>();
        public Task<bool> AddDecoratorsListeners()
        {
            BTListener newListener = decorator.Add();
            return newListener.NotifyAsync();
        }
        protected BTControlNode(BehaviorTree tree, BTDecorator? decorator = null, List<BTNode>? children = null) : base(tree)
        {
            _decorator = decorator;
            if (children == null)
            {
                taskChildren = new List<BTNode>();
                controlChildren = new List<BTControlNode>();
            } 
            else taskChildren = children;
        }

        public bool Evaluate()
        {
            return decorator == null ? true : decorator.Evaluate();
        }

        public BTControlNode BuildNode(BTNode nextNode)
        {
            if (nextNode is BTControlNode controlNode) controlChildren.Add(controlNode);
            else taskChildren.Add(nextNode);
            return this;
        }
        public void Reevaluate() //@TODO put them in order
        {
            currentlyExecutableChildren = new(taskChildren);
            foreach (BTControlNode chi in controlChildren) 
            {
                if (chi.Evaluate())
                    currentlyExecutableChildren.Add(chi);
            }
        }
        public override sealed async Task<bool> Execute()
        {
            tree.CurrentControlNode = this;
            Reevaluate();
            foreach (BTControlNode chi in controlChildren)
            {
                if (chi.decorator == null) continue;
                tasks[chi.AddDecoratorsListeners()] = chi.decorator;
            }
            bool result = await ExecuteImplementation();
            RemoveDecorators();
            tasks = new();
            return result;
        }
        protected abstract Task<bool> ExecuteImplementation();
        private void AfterExecute()
        {
            RemoveDecorators();
        }
        public void RemoveDecorators()
        {
            foreach (BTControlNode chi in controlChildren)
            {
                chi.decorator?.Remove();
            }
        }
        public void ClearTasks()
        {
            foreach(KeyValuePair<Task<bool>, BTDecorator> taskPair in tasks)
            {
                taskPair.Value.CancellationTokenSource.Cancel();
                taskPair.Value.CancellationTokenSource.Dispose();
            }
            tasks.Clear();
        }
    }
    public class Selector : BTControlNode
    {
        public Selector(BehaviorTree tree, List<BTNode>? children = null, BTDecorator? decorator = null) : base(tree, decorator, children) { }
        protected override async Task<bool> ExecuteImplementation()
        {
            await base.Execute();
            bool shouldStop = true;
            if (currentlyExecutableChildren.Count == 0)
            {
                Task<bool> completedTask = await Task.WhenAny(tasks.Keys);
                if (!completedTask.Result) return false; //TODO check if this works
                BTDecorator decoratorCalled = tasks[completedTask];
                Reevaluate();
            }

            foreach (var child in currentlyExecutableChildren)
            {
                if (child.decorator != null && child.decorator.Evaluate() == false) continue;
                shouldStop = await child.Execute();
                if (shouldStop) return true;
            }
            return false;
        }
    }
    public class Sequence : BTControlNode
    {
        public Sequence(BehaviorTree tree, List<BTNode>? children = null, BTDecorator? decorator = null) : base(tree, decorator, children) {}
        protected override async Task<bool> ExecuteImplementation()
        {
            if (currentlyExecutableChildren.Count == 0)
            {
                if (tasks.Count == 0)
                {
                    var a = 5;
                }
                Task<bool> completedTask = await Task.WhenAny(tasks.Keys);
                if (completedTask.Status == TaskStatus.Canceled) return false; //TODO check if this works
                BTDecorator decorator = tasks[completedTask];
                //tasks.Remove(completedTask);
                Reevaluate();
            }
            bool shouldContinue = true;
            foreach (var child in currentlyExecutableChildren)
            {
                shouldContinue = await child.Execute();
                if (!shouldContinue) return false;
            }
            return true;
        }
    }
    public abstract class BTTask<TTree> : BTNode where TTree : BehaviorTree
    {
        BTListener? isExecutedListener;
        protected TTree Tree { get; private set; }
        protected BTTask(TTree tree, BTListener? listener = null) : base(tree) 
        {
            Tree = tree;
            isExecutedListener = listener;
        }

        public override async Task<bool> Execute()
        {
            return true;
        }
    }
}
