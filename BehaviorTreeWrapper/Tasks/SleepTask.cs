using BehaviorTrees;
using BehaviorTrees.Nodes;
using System;
namespace BehaviorTreeWrapper.Tasks
{
    public class SleepTask : BTTask
    {
        private readonly TimeSpan sleepMiliseconds;
        public SleepTask(TimeSpan timeSpan) { sleepMiliseconds = timeSpan; }

        private DateTime _lastTime;
        private bool isExecuting;

        public override BTTaskStatus Execute()
        {
            if (isExecuting)
            {
                if (DateTime.Now - _lastTime > sleepMiliseconds)
                {
                    isExecuting = false;
                    return BTTaskStatus.FinishedWithTrue;
                }
                else
                {
                    return BTTaskStatus.Running;
                }
            }
            else
            {
                isExecuting = true;
                _lastTime = DateTime.Now;
                return BTTaskStatus.Running;
            }
        }
    }
}
