using System;
using System.Threading;

namespace BehaviorTreeWrapper
{
    public class BehaviorTreeBannerlordWrapper : IDisposable
    {
        public BehaviorTreeMissionLogic CurrentMission { get;  set; }
        static BehaviorTreeBannerlordWrapper _instance;
        private bool _disposed = false;
        public static BehaviorTreeBannerlordWrapper Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new BehaviorTreeBannerlordWrapper();
                return _instance;
            }
        }
        public void Subscribe(BannerlordBTListener listener)
        {
            CurrentMission.Subscribe(listener);
        }
        public void UnSubscribe(BannerlordBTListener listener)
        {
            CurrentMission.UnSubscribe(listener);
        }

        public void Dispose()
        {
            if (!_disposed)//true)//!_disposed)
            {
                foreach (MovementTree tree in CurrentMission.trees.Values)
                {
                    tree.Dispose();
                    tree.RetireTree();
                }
                //foreach (BannerlordBTListener listener in CurrentMission.GetAllListeners())
                //    listener.NotifyWithCancel();
                CurrentMission = null;
                _disposed = true;
            }
        }
    }
}