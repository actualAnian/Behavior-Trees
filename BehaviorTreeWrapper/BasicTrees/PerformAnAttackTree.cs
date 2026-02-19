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
    public class PerformAnAttackTree : BehaviorTree
    {
        BTBlackboardBannerlordBase _bbBase;
        public BTBlackboardValue<Agent> Agent { get; set; }

        public PerformAnAttackTree(BTBlackboardBannerlordBase bbBase) : base(2000)
        {
            _bbBase = bbBase;
        }
        public static new BehaviorTree BuildTree(object[] objects)
        {
            if (objects[0] is not Agent agent ||
                objects[1] is not string attackId ||
                objects[2] is not int sleepMiliseconds) throw new IncorrectParametersException(typeof(PerformAnAttackTree));

            var bbBase = new BTBlackboardBannerlordBase(agent);
            PerformAnAttackTree? tree = StartBuildingTree(new PerformAnAttackTree(bbBase))
                .AddSequence("make_attack")
                    .AddTask(new PlayAnimationTask(attackId, bbBase))
                    .AddTask(new SleepTask(new TimeSpan(0, 0, 2)))
                    .Up()
                .Finish();
            return tree;
        }
    }
}
