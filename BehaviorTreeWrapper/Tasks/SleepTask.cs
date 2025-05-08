using BehaviorTrees.Nodes;
using System;
namespace BehaviorTreeWrapper.Tasks
{
    public class SleepTask : BTTask
    {
        private readonly TimeSpan sleepMiliseconds;
        public SleepTask(int sleepTimeInMiliseconds) { sleepMiliseconds = new TimeSpan(0, 0, 0, 0, sleepTimeInMiliseconds); }

        private DateTime _lastTime;
        private bool isExecuting;

        public override BTNode Execute()
        {
            if (isExecuting)
            {
                if (DateTime.Now - _lastTime > sleepMiliseconds)
                {
                    isExecuting = false;
                    Status = BehaviorTrees.BTStatus.FinishedWithTrue;
                    return Parent;
                }
                else
                {
                    Status = BehaviorTrees.BTStatus.Running;
                    return this;
                }
            }
            else
            {
                _lastTime = DateTime.Now;
                Status = BehaviorTrees.BTStatus.Running;
                return this;
            }
        }
    }
}
