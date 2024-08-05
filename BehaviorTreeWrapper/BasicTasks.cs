using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BehaviorTreeWrapper
{
    public class PrintTask : BehaviorTree.Task
    {
        public PrintTask(BehaviorTree.BehaviorTree tree) : base(tree) { }

        public override async Task<bool> Execute()
        {
            MBInformationManager.AddQuickInformation(new TextObject("I spotted you!", null), 0, null, "");
            return true;
        }

        public override void RemoveDecorators() { }

        public override void UpdateTreeProperty() { }
    }
}
