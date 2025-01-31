using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BehaviorTrees.Nodes;

namespace BehaviorTrees
{
    public abstract class AbstractDecorator
    {
        public BTNode NodeBeingDecoracted { get; internal set; }

        protected internal AbstractDecorator() {}
        abstract public bool Evaluate();
    }
    public abstract class BTEventDecorator : AbstractDecorator, INotifiable
    {
        protected BTListener listener;
        public abstract void Notify(object[] data);
        internal protected BehaviorTree Tree { get; set; }
        public Task<bool> AddListener(CancellationToken cancellationToken)
        {
            listener.Subscribe(cancellationToken);
            return listener.Task;
        }
        public void Remove()
        {
            listener.UnSubscribe();
        }

        public BTEventDecorator() : base() { }
        public abstract void CreateListener();
    }
    public abstract class BTReturnFalseDecorator : AbstractDecorator
    {

    }
}