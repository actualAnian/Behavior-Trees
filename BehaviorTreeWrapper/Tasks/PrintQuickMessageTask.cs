using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using System.Threading;
using BehaviorTrees.Nodes;

namespace BehaviorTreeWrapper.Tasks
{
    public class PrintQuickMessageTask : BTTask
    {
        private readonly string message;
        public PrintQuickMessageTask(string message) : base() { this.message = message; }
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject(message, null));
            return true;
        }
    }
}
