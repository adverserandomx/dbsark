using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.CommonBot.Logic;
using Zeta.Common.Plugins;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.Service;

namespace MBot
{
    public static class Helper
    {

        public static void RemoveAllBehavior()
        {
            ProfileManager.CurrentProfileBehavior.ResetCachedDone();
        }

        public static void Log(string message) 
        { 
            Logging.Write(string.Format("[{0}] {1}", Name, message)); 
        }
    }
}
