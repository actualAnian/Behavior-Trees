using BehaviorTree;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper
{
    public class BannerlordTree : BehaviorTree.BehaviorTree
    {
        Agent Agent;

        public BannerlordTree(Agent agent)
        {
            Agent = agent;
        }

        public override void Notify()
        {
            throw new System.NotImplementedException();
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
            Sequence rootNode = new Sequence(tree, new List<Node> { new Sequence(tree, new List<Node> { new PrintTask(tree) }, new TrueDecorators(tree, SubscriptionPossibilities.OnAgentAlarmedStateChanged)) });
            //BannerlordBTListener listener = new(SubscriptionPossibilities.OnMissionTick, tree);
            //tree.BuildTree(rootNode, listener);
            tree.BuildTree(rootNode);
            return tree;
        }
    }
}
