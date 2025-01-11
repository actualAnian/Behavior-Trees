using BehaviorTrees;
using SandBox;
using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper
{
    public class BannerlordTree : BehaviorTree
    {
        public Agent Agent { get; }

        public BannerlordTree(Agent agent)
        {
            Agent = agent;
        }
    }
    public class MovementTree : BannerlordTree
    {
        public AgentNavigator Navigator { get; }
        public bool IsSuspicious = false;
        public MovementTree(Agent agent) : base(agent)
        {
            Navigator = Agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
        }

        public static MovementTree BuildBasicTree(Agent agent)
        {
            //MovementTree tree = new MovementTree(agent);
            //Sequence rootNode = new Sequence(tree);

            //MovementTree tree = BehaviorTree.BuildTree(new MovementTree(agent))
            //        .AddSequence("spotted", new AlarmedDecorator(SubscriptionPossibilities.OnAgentAlarmedStateChanged))
            //            .AddTask(new PrintTask())
            //            .AddTask(new MoveToPlaceTask(new Vec3(1, 1, 1)))
            //        .Finish();


            //tree.AddSequence("mainSequence")
            //         .AddTask<PrintTask>()
            //         .AddTask<MoveToPlaceTask>(new Vec3(1, 1, 1));

            //tree.Build()
            //    .Add(rootNode);
            //    .Add

            /*              root
             *            sequence
             *     printTask    printTask2
             */

            /*              root
             *            sequence
             *      (spotted)     (hit)       (Moved)
             *     printTask      MoveTo   ClearNavigator
             */

            Vec3 position = new(373, 295, 20);
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask(tree) }, new AlarmedDecorator(tree, SubscriptionPossibilities.OnAgentAlarmedStateChanged)));
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new MoveToPlaceTask(tree, position), new PrintTask3(tree) }, new HitDecorator(tree, SubscriptionPossibilities.OnAgentHit)));
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new ClearMovementTask(tree) }, new InPositionDecorator(tree, position, SubscriptionPossibilities.OnMissionTick)));

            MovementTree tree = BehaviorTree.BuildTree(new MovementTree(agent))
                .AddSelector("main")
                    .AddSequence("spotted", new AlarmedDecorator(SubscriptionPossibilities.OnAgentAlarmedStateChanged))
                        .AddTask(new PrintTask())
                        .Up()
                    .AddSequence("spotted", new HitDecorator(SubscriptionPossibilities.OnAgentHit))
                        .AddTask(new MoveToPlaceTask(position))
                        .AddTask(new PrintTask3())
                        .Up()
                    .AddSequence("spotted", new InPositionDecorator(position, SubscriptionPossibilities.OnMissionTick))
                        .AddTask(new ClearMovementTask())
                        .Up()
                    .Finish();
            //tree.BuildTree(rootNode, listener);

            /* whyyy
             
                          root
             *            sequence
             *   (spotted)             (hit)             //    (Moved)
             *  printTask    printTask MoveTo (hit)      // ClearNavigator
                                          ClearNavigator


            */
            //Vec3 position = new(373, 295, 20);
            //Sequence secondNode = new Sequence(tree, new List<BTNode> { new ClearMovementTask(tree) }, new HitDecorator(tree, SubscriptionPossibilities.OnAgentHit));
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask(tree) }, new AlarmedDecorator(tree, SubscriptionPossibilities.OnAgentAlarmedStateChanged)));
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask(tree), new MoveToPlaceTask(tree, position) }, new HitDecorator(tree, SubscriptionPossibilities.OnAgentHit)));
            ////rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new ClearMovementTask(tree) }, new InPositionDecorator(tree, position, SubscriptionPossibilities.OnMissionTick)));

            tree.StartTree();
            return tree;
        }
    }
}
