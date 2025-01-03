using BehaviorTree;
using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper
{
    public class BannerlordTree : BehaviorTree.BehaviorTree
    {
        public Agent Agent { get; }

        public BannerlordTree(Agent agent)
        {
            Agent = agent;
        }
    }
    public class BasicTree : BannerlordTree
    {
        public bool IsSuspicious = false;
        public BasicTree(Agent agent) : base(agent)
        {
            //Sequence seq = new Sequence(this, new List<Node> { new PrintTask(this) }, new TrueDecorators(this, SubscriptionPossibilities.OnAgentAlarmedStateChanged));
            //RootNode = seq;
            //RootNode.Update();
        }

        public static BasicTree BuildBasicTree(Agent agent)
        {
            BasicTree tree = new BasicTree(agent);
            Sequence rootNode = new Sequence(tree);
            /*              root
             *            sequence
             *     printTask    printTask2
             */

            /*              root
             *            sequence
             *      (spotted)     (hit)
             *     printTask    printTask2, printTask3
             */
            //var printTask = new PrintTask2(tree);
            //BTNode<BehaviorTree.BehaviorTree> node = (BTNode<BehaviorTree.BehaviorTree>)printTask;
            //new List<BTNode<BannerlordTree>> { printTask };
            //rootNode.BuildNode(new Sequence(tree, new List<BTNode<BannerlordTree>> { (BTNode<BehaviorTree.BehaviorTree>)printTask }, new AlarmedDecorator(tree, SubscriptionPossibilities.OnAgentAlarmedStateChanged)));
            
            rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask(tree) }, new AlarmedDecorator(tree, SubscriptionPossibilities.OnAgentAlarmedStateChanged)));
            rootNode.BuildNode(new Sequence(tree, new List<BTNode> { new PrintTask2(tree), new PrintTask3(tree) }, new HitDecorator(tree, SubscriptionPossibilities.OnAgentHit)));
            //BannerlordBTListener listener = new(SubscriptionPossibilities.OnMissionTick, tree);
            //tree.BuildTree(rootNode, listener);
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
