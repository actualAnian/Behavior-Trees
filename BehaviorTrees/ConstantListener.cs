namespace BehaviorTrees
{
    public abstract class ConstantEventListener : IBTNotifiable
    {
        public BTListener Listener { get; set; }
        public BehaviorTree Tree { get; set; }
        public abstract void CreateListener();

        public void HandleNotification(object[] data)
        {
            Notify(data);
        }
        public abstract void Notify(object[] data);
    }
}
