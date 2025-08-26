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
    public interface IBTNotifiable
    {
        public BTListener Listener { get; set; }
        public void HandleNotification(object[] data);
        public BehaviorTree Tree { get; set; }
        public abstract void CreateListener();
    }

    public abstract class BTEventDecorator : AbstractDecorator, IBTNotifiable
    {
        public BTListener Listener { get; set; }
        public BehaviorTree Tree { get; set; }
        public abstract void Notify(object[] data);
        public void HandleNotification(object[] data)
        {
            Notify(data);
            NodeBeingDecoracted.Parent.Status = BTStatus.ReceivedEvent;
            Tree.NodeReceivingEvent = NodeBeingDecoracted;
            Tree.ShouldRunNextTick = true;
        }
        public void AddListener()
        {
            NodeBeingDecoracted.Parent.Status = BTStatus.WaitingForEvent;
            Listener.Subscribe();
        }
        public void Remove()
        {
            Listener.UnSubscribe();
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