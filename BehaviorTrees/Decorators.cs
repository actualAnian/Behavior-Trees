using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BehaviorTrees.Nodes;

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
        public BTNode NodeBeingDecoracted { get; internal set; }

        private readonly OnDecoratorFalse _onFalse;
        protected internal AbstractDecorator(OnDecoratorFalse onFalse)
        {
            _onFalse = onFalse;
        }
        abstract public bool Evaluate();
        public Task<bool> AddListener(CancellationToken cancellationToken)
        {
            listener.Subscribe(cancellationToken);
            return listener.Task;
        }
        public void Remove()
        {
            listener.UnSubscribe();
        }
        public abstract void Notify(object[] data);
        internal OnDecoratorFalse OnDecoratorFalse => _onFalse;
    }
    public abstract class BTDecorator : AbstractDecorator
    {
        internal protected BehaviorTree Tree { get; set; }
        public BTDecorator(OnDecoratorFalse onFalse) : base(onFalse)
        {
        }
        public abstract void CreateListener();
    }
}