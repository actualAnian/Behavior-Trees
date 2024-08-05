namespace BehaviorTree
{
    public abstract class BTDecorator : INotifiable
    {
        public BehaviorTree Tree;
        protected BTListener listener;

        BehaviorTree INotifiable.NotifiedTree { get { return Tree; } }

        protected BTDecorator(BehaviorTree tree)
        {
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
        public void NotifyTree()
        {
            Tree.NotifyOnPropertyChange();
        }

        public abstract void Notify();
    }
}
