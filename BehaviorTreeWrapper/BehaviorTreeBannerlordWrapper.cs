using BehaviorTree;

namespace BehaviorTreeWrapper
{
    public class BehaviorTreeBannerlordWrapper
    {
        public BehaviorTreeMissionLogic CurrentMission { get;  set; }
        static BehaviorTreeBannerlordWrapper _instance;
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
    }
}
