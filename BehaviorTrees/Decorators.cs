using System.Collections.Generic;
using System.Threading;

namespace BehaviorTrees
{
    public enum OnDecoratorFalse
    {
        ReturnFalse,
        AwaitEvent
    }
    public abstract class AbstractDecorator : INotifiable
    {
        protected BTListener listener;
        public CancellationTokenSource CancellationTokenSource { get; }

        protected internal AbstractDecorator()
        {
            CancellationTokenSource = new();
        }
        //abstract public void Update();
        abstract public bool Evaluate();
        public BTListener Add()
        {
            listener.Subscribe();
            return listener;
        }
        public void Remove()
        {
            listener.UnSubscribe();
        }

        public virtual void Notify(List<object> data)
        {
        }
    }
    public abstract class BTDecorator<TTree> : AbstractDecorator where TTree : BehaviorTree
    {
        public TTree Tree;
        public BTDecorator(TTree tree)
        {
            Tree = tree;
        }
    }
}
