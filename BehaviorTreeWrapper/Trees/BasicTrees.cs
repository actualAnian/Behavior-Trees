﻿using BehaviorTrees;
using SandBox;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using BehaviorTreeWrapper.Decorators;
using BehaviorTreeWrapper.Tasks;
using BehaviorTreeWrapper.BlackBoardClasses;

namespace BehaviorTreeWrapper.Trees
{
    public class ExampleTree : BehaviorTree, IBTBannerlordBase, IBTMovable
    {
        public BTBlackboardValue<AgentNavigator> Navigator { get; set; }
        public BTBlackboardValue<Agent> Agent { get; set; }

        public ExampleTree(Agent agent) : base(2000)
        {
            Navigator = new BTBlackboardValue<AgentNavigator>(agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator());
            Agent = new BTBlackboardValue<Agent>(agent);
        }
        public static new BehaviorTree? BuildTree(object[] objects)
        {
            if (objects[0] is not Agent agent) return null;
            Vec3 position = new(373, 295, 20);
            ExampleTree? tree = StartBuildingTree(new ExampleTree(agent))
                .AddSelector("main")
                    .AddSequence("spotted", new AlarmedDecorator(SubscriptionPossibilities.OnSelfAlarmedStateChanged))
                        .AddTask(new PrintTask())
                        .Up()
                    .AddSequence("spotted", new HitDecorator(SubscriptionPossibilities.OnSelfIsHit))
                        .AddTask(new MoveToPlaceTask(position))
                        .AddTask(new PrintMessageTask("I am hit"))
                        .Up()
                    .AddSequence("spotted", new InPositionDecorator(position))
                        .AddTask(new ClearMovementTask())
                        .Up()
                .Finish();
            return tree;
        }
    }
}