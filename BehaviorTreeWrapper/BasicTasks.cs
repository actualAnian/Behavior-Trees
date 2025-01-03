using BehaviorTree;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BehaviorTreeWrapper
{
    public class PrintTask : BTTask<BannerlordTree>
    {
        public PrintTask(BannerlordTree tree) : base(tree) { }

        public override async Task<bool> Execute()
        {
            MBInformationManager.AddQuickInformation(new TextObject("I spotted you!", null), 0, null, "");
            return true;
        }
    }
    public class PrintTask2 : BTTask<BannerlordTree>
    {
        public PrintTask2(BannerlordTree tree) : base(tree) { }

        public override async Task<bool> Execute()
        {
            MBInformationManager.AddQuickInformation(new TextObject("I am hit!", null), 0, null, "");
            return true;
        }
    }
    public class PrintTask3 : BTTask<BannerlordTree>
    {
        public PrintTask3(BannerlordTree tree) : base(tree) { }

        public override async Task<bool> Execute()
        {
            MBInformationManager.AddQuickInformation(new TextObject("Second message!", null), 0, null, "");
            return true;
        }
    }
    public class MoveToPlaceTask : BTTask<BannerlordTree>
    {
        public MoveToPlaceTask(BannerlordTree tree) : base(tree) { }

        public override async Task<bool> Execute()
        {
            MBInformationManager.AddQuickInformation(new TextObject("Second message!", null), 0, null, "");
            return true;
        }
    }
}
