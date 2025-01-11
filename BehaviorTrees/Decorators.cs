using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        //public CancellationTokenSource CancellationTokenSource { get; }

        private readonly OnDecoratorFalse _onFalse;
        protected internal AbstractDecorator(OnDecoratorFalse onFalse)
        {
            _onFalse = onFalse;
            //CancellationTokenSource = new();
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
        public virtual void Notify(List<object> data) { }
        internal OnDecoratorFalse OnDecoratorFalse => _onFalse;
    }
    public abstract class BTDecorator<TTree> : AbstractDecorator where TTree : BehaviorTree
    {
        public TTree Tree { get; set; }
        public BTDecorator(OnDecoratorFalse onFalse) : base(onFalse)
        {
        }
        public abstract void CreateListener();
    }
}
