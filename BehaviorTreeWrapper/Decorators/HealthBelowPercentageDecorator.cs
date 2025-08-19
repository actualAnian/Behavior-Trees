using BehaviorTrees;
using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using BehaviorTreeWrapper.BlackBoardClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.Decorators
{
    public class HealthBelowPercentageDecorator : BannerlordNoWaitDecorator, IBTBannerlordBase
    {
        BTBlackboardValue<Agent> agent;
        private int healthPercentageThreshold;
        public HealthBelowPercentageDecorator(int healthPercentageThreshold)
        {
            this.healthPercentageThreshold = healthPercentageThreshold;
        }

        public BTBlackboardValue<Agent> Agent { get => agent; set => agent = value; }

        public override bool Evaluate()
        {
            int minHealth = (int)Agent.GetValue().HealthLimit * healthPercentageThreshold / 100;
            return Agent.GetValue().Health < minHealth;
        }
    }
}
