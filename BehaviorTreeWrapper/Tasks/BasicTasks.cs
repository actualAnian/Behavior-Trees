using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using System.Threading;
using BehaviorTrees.Nodes;

namespace BehaviorTreeWrapper.Tasks
{
    public class PrintTask : BTTask
    {
        public PrintTask() { }
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject("I spotted you!", null), 0, null, "");
            return true;
        }
    }
    public class PrintMessageTask : BTTask
    {
        string message;
        public PrintMessageTask(string message) : base() { this.message = message; }
        public override async Task<bool> Execute(CancellationToken cancellationToken)
        {
            MBInformationManager.AddQuickInformation(new TextObject(message, null), 0, null, "");
            return true;
        }
    }
}
