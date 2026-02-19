using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.Tasks
{
    public class HealTask : BTTask
    {
        readonly BTBlackboardBannerlordBase _bbBase;
        readonly float _percentage;
        public HealTask(float percentage, BTBlackboardBannerlordBase bbBase) : base()
        {
            _percentage = percentage; 
            _bbBase = bbBase;
        }

        public override BTTaskStatus Execute()
        {
            Agent agent = _bbBase.Agent;
            agent.Health += agent.HealthLimit * _percentage;
            if (agent.Health > agent.HealthLimit)
                agent.Health = agent.HealthLimit;
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
