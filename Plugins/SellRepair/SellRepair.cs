using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Zeta;
using Zeta.Common;
using Zeta.CommonBot;
using Zeta.Common.Plugins;
using Zeta.Internals;
using Zeta.Internals.Actors;
using Zeta.Internals.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace MBot.SellRepair
{
    public class SellRepair : IPlugin
    {
        #region iPlugin
        public string Author { get { return "adx"; } }

        public string Description { get { return "Sell and repair"; } }

        public System.Windows.Window DisplayWindow
        {
            get { return null; }
        }

        public string Name
        {
            get { return "Sell and repair " + Version; }
        }

        public Version Version
        {
            get { return new Version(1, 1); }
        }


        bool IEquatable<IPlugin>.Equals(IPlugin other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }

        #endregion  

        void IPlugin.OnDisabled()
        {
            Log("Disabled", Name);
        }

        void IPlugin.OnEnabled()
        {
            Log("Enabled", Name);
            Execute();
        }

        void IPlugin.OnInitialize()
        {
            return;
        }

        void IPlugin.OnPulse()
        {
            return;
        }

        void IPlugin.OnShutdown()
        {
            return;
        }

        Zeta.Common.Helpers.WaitTimer _timer = new Zeta.Common.Helpers.WaitTimer(new TimeSpan(0, 0, 1));
        public void Execute()
        {
            BotMain.CurrentBot.Pulse();
            Log(new Zeta.CommonBot.Settings.CharacterSettings().ItemRuleSetPath, Name);

            Zeta.CommonBot.ItemManager.LoadRules(new Zeta.CommonBot.Settings.CharacterSettings().ItemRuleSetPath);

            MoveTo(2973, 2797);
            Thread.Sleep(500);
            
            
            Interact("Player_Shared_Stash"); // this is failing
            Thread.Sleep(1000);

            Zeta.CommonBot.ItemManager.StashItems();

        }

        public void RepairAndSell()
        {
            if (ZetaDia.Me.Inventory.NumFreeBackpackSlots <= 4)
            {

                ProfileManager.CurrentProfileBehavior.ResetCachedDone();

                ZetaDia.Me.UseTownPortal();

                IEnumerable<DiaObject> objQuery =
                    from unit in ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false)
                    where unit.ActorType == Zeta.Internals.SNO.ActorType.Gizmo
                    select unit;

            
                Zeta.CommonBot.ItemManager.StashItems();
                //stash

                //walk to vendor
                //sell 
                //repair
                //salvage
            }
        }

        public bool Interact(string name)
        {
            DiaObject unit = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault(u => u.Name.Contains(name));
            if (unit.Name.Contains(name))
                return unit.Interact();
            else 
                return false;
        }

        public bool Interact(DiaObject u)
        {
            return ZetaDia.Me.UsePower(u.ActorType == Zeta.Internals.SNO.ActorType.Gizmo || u.ActorType == Zeta.Internals.SNO.ActorType.Item ? SNOPower.Axe_Operate_Gizmo : SNOPower.Axe_Operate_NPC, u.Position);
        }

        public bool MoveTo(float x, float y)
        {
            if ((float)Math.Sqrt(Math.Pow(ZetaDia.Me.Position.X - x, 2) + Math.Pow(ZetaDia.Me.Position.Y - y, 2)) < 5)
                return true;
            Zeta.Navigation.Navigator.PlayerMover.MoveTowards(new Vector3(x, y, ZetaDia.Me.Position.Z));
            return false;
        }

        public static void RemoveAllBehavior()
        {
            ProfileManager.CurrentProfileBehavior.ResetCachedDone();
        }

        public static void Log(string message, string name)
        {
            Logging.Write(string.Format("[{0}] {1}", name, message));
        }

    }
}
