using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public abstract class BTNode
    {
        protected BehaviorTree BaseTree { get; set; }
        public virtual AbstractDecorator? Decorator
        {
            get
            {
                return null;
            }
        }
        protected BTNode() { }
        public virtual async Task<bool> Execute(CancellationToken cancellationToken) { return true; }
        public Task<bool>? AddDecoratorsListeners(CancellationToken cancellationToken)
        {
            return Decorator?.AddListener(cancellationToken);
        }
    }
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
            //Reevaluate();
            //foreach (BTControlNode chi in controlChildren)
            //{
            //    if (chi.decorator == null) continue;
            //    tasks[chi.AddDecoratorsListeners()] = chi.decorator;
            //}
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
            //for(int i = 0; i < tasks.Count; i++)
            //{
            //    decorators[i].CancellationTokenSource.Cancel();
            //    decorators[i].CancellationTokenSource.Dispose();
            //}
            decorators.Clear();
            tasks.Clear();
        }
        protected bool ExecutedAll()
        {
            return alreadyExecutedNodes >= allChildren.Count;
        }
    }
    public class Selector : BTControlNode
    {
        public Selector(BehaviorTree tree, List<BTNode>? children = null, AbstractDecorator? decorator = null) : base(tree, decorator, children) { }
        protected override async Task<bool> ExecuteImplementation(CancellationToken cancellationToken)
        {
            foreach (BTNode child in allChildren)
            {
                if (child.Decorator == null) currentlyExecutableChildren.Add(child);
                else
                {
                    if (child.Decorator.Evaluate()) currentlyExecutableChildren.Add(child);
                    else if (child.Decorator.OnDecoratorFalse == OnDecoratorFalse.AwaitEvent) childrenWithTasks.Add(child); 
                }
            }
            bool shouldStop = false;
            foreach (var child in currentlyExecutableChildren)
            {
                shouldStop = await child.Execute(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (shouldStop) return true;
                alreadyExecutedNodes += 1;
            }
            if (childrenWithTasks.Count == 0) return false;
            AddTasks(cancellationToken);
            while (!ExecutedAll())
            {
                currentlyExecutableChildren = new();
                if (childrenWithTasks.Count > 0)
                {
                    List<AbstractDecorator> decoratorsList = new(decorators);
                    List<Task<bool>> tasksList = new(tasks)
                    {
                        GetCancellationTask(cancellationToken)
                    };
                    Task<bool> completedTask = await Task.WhenAny(tasksList);
                    cancellationToken.ThrowIfCancellationRequested();
                    int completedIndex = tasks.IndexOf(completedTask);
                    if (!completedTask.Result) return false;
                    AbstractDecorator decoratorCalled = decoratorsList[completedIndex];
                    BTNode child = decoratorCalled.NodeBeingDecoracted;
                    if (decoratorCalled.Evaluate())
                    {
                        shouldStop = await child.Execute(cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                    } 
                    else
                    {
                        lock (tasks)
                        {
                            tasks.RemoveAt(completedIndex);
                            RemoveDecorator(decoratorCalled);
                            decorators.RemoveAt(completedIndex);
                            tasks.Add(child.AddDecoratorsListeners(cancellationToken));
                            decorators.Add(child.Decorator);
                        }
                    }
                    if (shouldStop) return true;
                }
            }
            return false;
        }
    }
    public class Sequence : BTControlNode
    {
        public Sequence(BehaviorTree tree, List<BTNode>? children = null, AbstractDecorator? decorator = null) : base(tree, decorator, children) { }
        protected override async Task<bool> ExecuteImplementation(CancellationToken cancellationToken)
        {
            bool shouldContinue = true;
            foreach (BTNode child in allChildren)
            {
                if (child.Decorator == null)
                {
                    shouldContinue = await child.Execute(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!shouldContinue) return false;
                }
                else
                {
                    bool evaluate = child.Decorator.Evaluate();
                    if (!evaluate && child.Decorator.OnDecoratorFalse == OnDecoratorFalse.ReturnFalse) return false;
                    while (!evaluate && child.Decorator.OnDecoratorFalse == OnDecoratorFalse.AwaitEvent)
                    {
                        await child.AddDecoratorsListeners(cancellationToken);
                        evaluate = child.Decorator.Evaluate();
                    }

                    shouldContinue = await child.Execute(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!shouldContinue) return false;
                }
            }
            return true;
        }
    }
    public abstract class BTTask<TTree> : BTNode where TTree : BehaviorTree
    {
        BTListener? isExecutedListener;
        private TTree _tree;
        public TTree Tree 
        {
            get { return _tree; }
            set 
            {
                _tree = value;
                BaseTree = _tree;
            }
        }
        protected BTTask(BTListener? listener = null) : base()
        {
            isExecutedListener = listener;
        }

        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
