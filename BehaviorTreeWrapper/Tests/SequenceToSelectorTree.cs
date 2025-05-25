using BehaviorTrees;
using SandBox;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using BehaviorTreeWrapper.Decorators;
using BehaviorTreeWrapper.Tasks;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Tests
{
    public class SequenceToSelectorTree : BehaviorTree, IBTBannerlordBase, IBTMovable
    {
        public BTBlackboardValue<AgentNavigator> Navigator { get; set; }
        public BTBlackboardValue<Agent> Agent { get; set; }

        public SequenceToSelectorTree(Agent agent) : base(2000)
        {
            Navigator = new BTBlackboardValue<AgentNavigator>(agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator());
            Agent = new BTBlackboardValue<Agent>(agent);
        }
        public static new BehaviorTree? BuildTree(object[] objects)
        {
            if (objects[0] is not Agent agent) return null;

            Vec3 position = new(373, 295, 20);
            SequenceToSelectorTree? tree = StartBuildingTree(new SequenceToSelectorTree(agent))
                .AddSequence("main")
                    .AddSelector("main", new HealthBelowPercentageDecorator(99))
                        .AddSequence("spotted", new AlarmedDecorator(SubscriptionPossibilities.OnSelfAlarmedStateChanged))
                            .AddTask(new PrintQuickMessageTask("I am spotted"))
                            .AddTask(new SleepTask(new System.TimeSpan(0, 0, 4)))
                            .AddTask(new PrintQuickMessageTask("the sleep finished!"))
                            .Up()
                        .AddSequence("hit", new HitDecorator(SubscriptionPossibilities.OnSelfIsHit))
                            .AddTask(new MoveToPlaceTask(position))
                            .AddTask(new PrintQuickMessageTask("I am hit"))
                            .Up()
                        .AddSequence("moved", new InPositionDecorator(position))
                            .AddTask(new ClearMovementTask())
                            .Up()
                        .Up()
                    .AddTask(new PrintQuickMessageTask("End of the tree!"))
                    .Finish();
            return tree;
        }
    }
}