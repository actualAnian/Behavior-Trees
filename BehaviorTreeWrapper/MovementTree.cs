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
            MovementTree tree = new MovementTree(agent);
            Sequence rootNode = new Sequence(tree);
            /*              root
             *            sequence
             *     printTask    printTask2
             */

            /*              root
             *            sequence
             *      (spotted)     (hit)       (Moved)
             *     printTask      MoveTo   ClearNavigator
             */

            //Vec3 position = new(373, 295, 20);
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask(tree) }, new AlarmedDecorator(tree, SubscriptionPossibilities.OnAgentAlarmedStateChanged)));
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new MoveToPlaceTask(tree, position), new PrintTask3(tree) }, new HitDecorator(tree, SubscriptionPossibilities.OnAgentHit)));
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new ClearMovementTask(tree) }, new InPositionDecorator(tree, position, SubscriptionPossibilities.OnMissionTick)));

            //tree.BuildTree(rootNode, listener);

            /* whyyy
             
                          root
             *            sequence
             *   (spotted)             (hit)             //    (Moved)
             *  printTask    printTask MoveTo (hit)      // ClearNavigator
                                          ClearNavigator


            */
            Vec3 position = new(373, 295, 20);
            Sequence secondNode = new Sequence(tree, new List<BTNode> { new ClearMovementTask(tree) }, new HitDecorator(tree, SubscriptionPossibilities.OnAgentHit));
            rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask(tree) }, new AlarmedDecorator(tree, SubscriptionPossibilities.OnAgentAlarmedStateChanged)));
            rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask(tree), new MoveToPlaceTask(tree, position) }, new HitDecorator(tree, SubscriptionPossibilities.OnAgentHit)));
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new ClearMovementTask(tree) }, new InPositionDecorator(tree, position, SubscriptionPossibilities.OnMissionTick)));

            tree.BuildTree(rootNode);
            return tree;
        }
        // @TODO test if these are 
        internal void RetireTree()
        {
            CurrentControlNode.RemoveDecorators();
        }
    }
}
