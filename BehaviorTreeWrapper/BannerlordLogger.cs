using BehaviorTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BehaviorTreeWrapper
{
    public class BannerlordLogger : ILogger
    {
        public void LogMessage(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message, new Color(1, 0, 0)));
        }
    }
}
