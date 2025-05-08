using System.Threading.Tasks;
using System.Threading;
using BehaviorTrees.Nodes;
using TaleWorlds.Library;

namespace BehaviorTreeWrapper.Tasks
{
    public class PrintBottomLeftMessageTask : BTTask
    {
        private readonly string message;
        public PrintBottomLeftMessageTask(string message) { this.message = message; }
        public override BTNode Execute()
        {
            InformationManager.DisplayMessage(new(message));
            return Parent;
        }
    }
}
