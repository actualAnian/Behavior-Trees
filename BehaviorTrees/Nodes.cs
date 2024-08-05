using System.Collections.Generic;
using System.Threading.Tasks;

namespace BehaviorTree
{
    public abstract class Node
    {
        public BehaviorTree tree;
        protected Node(BehaviorTree tree)
        {
            this.tree = tree;
        }
        //public bool Evaluate()
        //{
        //    return decorator == null ? true : decorator.Evaluate();
        //}
        public abstract void RemoveDecorators();
        public virtual async Task<bool> Execute() { return true; }
        //public void Update()
        //{
        //    decorator?.Update();
        //}
        public abstract void UpdateTreeProperty();

    }
    public abstract class ControlNode : Node
    {
        public List<Node> children;
        public List<Node> currentlyExecutableChildren = new();
        public BTDecorator? decorator;
        public List<BTListener> Listeners { get; private set; }
        protected Dictionary<Task<bool>, BTListener> tasks = new Dictionary<Task<bool>, BTListener>();

        public void AddDecoratorsListeners()
        {
            BTListener? newListener = decorator?.Add();
            if (newListener != null)
                tasks[newListener.NotifyAsync()] = newListener;
        }
        protected ControlNode(BehaviorTree tree, BTDecorator? decorator = null, List<Node>? children = null) : base(tree)
        {
            this.decorator = decorator;
            if (children == null) this.children = new List<Node>();
            else this.children = children;
        }

        public ControlNode BuildNode(Node nextNode)
        {
            children.Add(nextNode);
            return this;
        }
        public void Reevaluate()
        {
            currentlyExecutableChildren = new();
            foreach (Node chi in children) 
            {
                if (chi.Evaluate())
                    currentlyExecutableChildren.Add(chi);
            }
        }
        public override async Task<bool> Execute()
        {
            tree.CurrentControlNode = this;
            foreach (Node chi in children)
            {
                chi.AddDecoratorsListeners();
            }
            return true;
        }
        public override void RemoveDecorators()
        {
            foreach (Node chi in children)
            {
                chi.decorator?.Remove();
            }
        }
        public override void UpdateTreeProperty()
        {
            throw new System.NotImplementedException();
        }
    }
    public class Selector : ControlNode
    {
        
        public Selector(BehaviorTree tree, List<Node>? children = null, BTDecorator? decorator = null) : base(tree, decorator, children) { }
        public override async Task<bool> Execute()
        {
            base.Execute();
            bool shouldStop = true;
            Task<bool> completedTask = await System.Threading.Tasks.Task.WhenAny(tasks.Keys);

            foreach (var child in currentlyExecutableChildren)
            {
                if (child.decorator != null && child.decorator.Evaluate() == false) continue;
                shouldStop = await child.Execute();
                if (shouldStop) return true;
            }
            return false;
        }
    }
    public class Sequence : ControlNode
    {
        public Sequence(BehaviorTree tree, List<Node>? children = null, BTDecorator? decorator = null) : base(tree, decorator, children) {}
        public override async Task<bool> Execute()
        {
            base.Execute();
            bool shouldContinue = true;
            foreach (var child in currentlyExecutableChildren)
            {
                shouldContinue = await child.Execute();
                if (!shouldContinue) return false;
            }
            return true;
        }
    }
    public abstract class Task : Node
    {
        BTListener? isExecutedListener;
        protected Task(BehaviorTree tree, BTDecorator? decorator = null, BTListener? listener = null) : base(tree, decorator) 
        {
            isExecutedListener = listener;
        }

        public override async Task<bool> Execute()
        {
            return true;
        }
    }
}
