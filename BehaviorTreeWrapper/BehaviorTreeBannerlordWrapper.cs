using BehaviorTrees;
using BehaviorTreeWrapper.Decorators;
using System;
using System.Threading;

namespace BehaviorTreeWrapper
{
    public class BehaviorTreeBannerlordWrapper : IDisposable
    {
        private BehaviorTreeMissionLogic? _missionLogic;
        public BehaviorTreeMissionLogic CurrentMissionLogic
        {
            get => _missionLogic;
            set
            {
                _missionLogic = value;
                _disposed = false;
            }
        }
        static BehaviorTreeBannerlordWrapper? _instance;
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
            CurrentMissionLogic?.Subscribe(listener);
        }
        public void UnSubscribe(BannerlordBTListener listener)
        {
            CurrentMissionLogic?.UnSubscribe(listener);
        }
        public void Subscribe(BannerlordBTTickListener listener)
        {
            CurrentMissionLogic?.Subscribe(listener);
        }
        public void UnSubscribe(BannerlordBTTickListener listener)
        {
            CurrentMissionLogic?.UnSubscribe(listener);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (BehaviorTree tree in CurrentMissionLogic.trees.Values)
                {
                    tree.Dispose();
                }
                CurrentMissionLogic = null;
                _disposed = true;
            }
        }
    }
}