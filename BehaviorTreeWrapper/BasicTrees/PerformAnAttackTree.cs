using BehaviorTrees;
using BehaviorTreeWrapper.BlackBoardClasses;
using BehaviorTreeWrapper.Decorators;
using BehaviorTreeWrapper.Tasks;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace BehaviorTreeWrapper.Trees
{
    public class PerformAnAttackTree : BehaviorTree, IBTBannerlordBase
    {
        public BTBlackboardValue<Agent> Agent { get; set; }

        public PerformAnAttackTree(Agent agent) : base(2000)
        {
            Agent = new BTBlackboardValue<Agent>(agent);
        }
        // agent, attack_string_id, time to sleep afterwards
        public static new BehaviorTree BuildTree(object[] objects)
        {
            if (objects[0] is not Agent agent ||
                objects[1] is not string attackId ||
                objects[2] is not int sleepMiliseconds) throw new IncorrectParametersException(typeof(PerformAnAttackTree));

            PerformAnAttackTree? tree = StartBuildingTree(new PerformAnAttackTree(agent))
                .AddSequence("make_attack")
                    .AddTask(new PlayAnimationTask(attackId))
                    .AddTask(new SleepTask(sleepMiliseconds))
                    .Up()
                .Finish();
            return tree;
        }
    }
}
