using BehaviorTreeWrapper.AbstractDecoratorsListeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorTreeWrapper.Tests
{
    internal class AlwaysFalseDecorator : BannerlordNoWaitDecorator
    {
        public override bool Evaluate()
        {
            return false;
        }
    }
}
