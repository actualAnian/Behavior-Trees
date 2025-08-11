using BehaviorTrees.Nodes;
using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.Agent;

namespace BehaviorTreeWrapper.Tasks
{
    public class HealTask : BTTask, IBTBannerlordBase
    {
        BTBlackboardValue<Agent> _agent;
        public BTBlackboardValue<Agent> Agent { get => _agent; set => _agent = value; }
        float percentage;
        public HealTask(float percentage) : base() { this.percentage = percentage; }

        public override BTTaskStatus Execute()
        {
            Agent agent = Agent.GetValue();
            agent.Health += agent.HealthLimit * percentage;
            if (agent.Health > agent.HealthLimit)
                agent.Health = agent.HealthLimit;
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
