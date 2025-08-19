using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using System;

namespace BehaviorTreeWrapper.Decorators
{
    public class WaitNSecondsTickDecorator : BannerlordTickTimedDecorator
    {
        private readonly TimeSpan waitSeconds;
        private DateTime? _lastTime;
        public WaitNSecondsTickDecorator(double timeToWait) : base(timeToWait)
        {
            waitSeconds = TimeSpan.FromSeconds(timeToWait);
        }
        public override bool Evaluate()
        {
            if (_lastTime == null)
            {
                _lastTime = DateTime.Now;
                return false;
            }
            if (DateTime.Now - _lastTime > waitSeconds)
            {
                _lastTime = null;
                return true;
            }
            else return false;
        }
        public override void Notify(object[] data) { }
    }
}