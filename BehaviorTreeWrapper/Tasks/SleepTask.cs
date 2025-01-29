using BehaviorTrees.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace BehaviorTreeWrapper.Tasks
{
    public class SleepTask : BTTask
    {
        private readonly int sleepMiliseconds;
        public SleepTask(int sleepTimeInMiliseconds) { sleepMiliseconds = sleepTimeInMiliseconds; }
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(sleepMiliseconds);
            return true;
        }
    }
}
