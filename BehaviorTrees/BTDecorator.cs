using System.Collections.Generic;
using System.Threading;

namespace BehaviorTree
{
    public abstract class BTDecorator : INotifiable
    {
        public BehaviorTree Tree;
        protected BTListener listener;
        public CancellationTokenSource CancellationTokenSource { get; }
        //BehaviorTree INotifiable.NotifiedTree { get { return Tree; } }

        protected BTDecorator(BehaviorTree tree)
        {
            CancellationTokenSource = new();
            Tree = tree;
        }
        abstract public void Update();
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
}
