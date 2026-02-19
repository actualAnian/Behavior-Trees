using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Decorators
{
    public class HealthBelowPercentageDecorator : BannerlordNoWaitDecorator
    {
        readonly BTBlackboardBannerlordBase _bbBase;
        readonly float _healthPercentageThreshold;
        public HealthBelowPercentageDecorator(int healthPercentageThreshold, BTBlackboardBannerlordBase bbBase)
        {
            _healthPercentageThreshold = healthPercentageThreshold;
            _bbBase = bbBase;
        }
        public override bool Evaluate()
        {
            var minHealth = _bbBase.Agent.HealthLimit * _healthPercentageThreshold / 100;
            return _bbBase.Agent.Health < minHealth;
        }
    }
}
