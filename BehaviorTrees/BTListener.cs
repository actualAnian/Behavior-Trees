using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTrees
{
    public abstract class BTListener
    {
        public BehaviorTree Tree { get; private set; }
        public IBTNotifiable Notifies { get; private set; }
        protected BTListener(BehaviorTree tree, IBTNotifiable notifies)
        {
            Notifies = notifies;
            Tree = tree;
        }

        public virtual void Subscribe() { }
        public abstract void UnSubscribe();
        public void Notify(object[] data)
        {
            Notifies.HandleNotification(data);
        }
    }
}
