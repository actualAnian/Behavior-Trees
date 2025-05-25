using System.Collections.Generic;
using System.Data;
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
        abstract internal bool HandleEvaluation();
    }
    public abstract class BTEventDecorator : AbstractDecorator
    {
        protected BTListener listener;
        public abstract void Notify(object[] data);
        internal virtual void HandleNotification(object[] data)
        {
            Notify(data);
            NodeBeingDecoracted.Parent.Status = BTStatus.ReceivedEvent;
            Tree.NodeReceivingEvent = NodeBeingDecoracted;
            Tree.ShouldRunNextTick = true;
        }
        internal protected BehaviorTree Tree { get; set; }
        public void AddListener()
        {
            NodeBeingDecoracted.Parent.Status = BTStatus.WaitingForEvent;
            listener.Subscribe();
        }
        public void Remove()
        {
            listener.UnSubscribe();
        }
        internal override bool HandleEvaluation()
        {
            if (Evaluate()) return true;
            AddListener();
            return false;
        }

        public BTEventDecorator() : base() { }
        public abstract void CreateListener();
    }
    public abstract class BTReturnFalseDecorator : AbstractDecorator
    {
        internal override bool HandleEvaluation()
        {
            return Evaluate();
        }
    }
}