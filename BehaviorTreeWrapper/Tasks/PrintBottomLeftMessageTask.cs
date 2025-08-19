using System.Threading.Tasks;
using System.Threading;
using BehaviorTrees.Nodes;
using TaleWorlds.Library;
using BehaviorTrees;

namespace BehaviorTreeWrapper.Tasks
{
    public class PrintBottomLeftMessageTask : BTTask
    {
        private readonly string message;
        public PrintBottomLeftMessageTask(string message) { this.message = message; }
        public override BTTaskStatus Execute()
        {
            InformationManager.DisplayMessage(new(message));
            return BTTaskStatus.FinishedWithTrue;
        }
    }
}
