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

namespace Crypts
{
    public class Crypts : IPlugin
    {
        private bool IsRestarting { get; set; }
        public Version Version { get { return new Version(1, 0); } }
        public string Author { get { return "McCloud"; } }
        public string Description { get { return "Runs to Sarkoth fast."; } }
        public string Name { get { return "DH SarkothRunner v" + Version; } }
        public Window DisplayWindow { get { return null; } }

        private void Log(string message) { Logging.Write(string.Format("[{0}] {1}", Name, message)); }

        public void OnShutdown() { OnDisabled(); }

        public void OnEnabled()
        {
            Log("Enabled.");
            Zeta.CommonBot.GameEvents.OnGameJoined += GameJoined;
            //Zeta.CommonBot.GameEvents.OnPlayerDied += OnPlayerDied;
            running = true;
            if (ZetaDia.IsInGame && !ZetaDia.IsLoadingWorld) Execute();
        }

        public void OnInitialize() { }

        public void OnDisabled()
        {
            Log("Disabled.");
            Zeta.CommonBot.GameEvents.OnGameJoined -= GameJoined;
            //Zeta.CommonBot.GameEvents.OnPlayerDied -= OnPlayerDied;
            running = false;
        }
        public bool Equals(IPlugin other) { return (other.Name == Name) && (other.Version == Version); }

        public void GameJoined(object sender, EventArgs e) { step = 0; Execute(); }

        //public void OnPlayerDied(object sender, EventArgs e) { Log("Died"); step = 13; }

        public int step = 0;
        public bool running = false;

        public void Execute()
        {
            while (running && BotMain.IsRunning)
            {
                BotMain.CurrentBot.Pulse();
                switch (step)
                {
                    case 0:
                        if (ZetaDia.IsInGame && !ZetaDia.IsLoadingWorld && ZetaDia.Me.IsValid)
                        {
                            ZetaDia.Me.UsePower(SNOPower.DemonHunter_Companion, ZetaDia.Me.Position);
                            step++;
                        }
                        break;
                    case 1:
                        if (MoveSuperFuckingFast(1993, 2603))
                
                            step++;
                        break;
                    case 2:
                        if (MoveSuperFuckingFast(2026, 2557))
                            step++;
                        break;
                    case 3:
                        if (!ConditionParser.ActorExistsAt(176007, 2059, 2478, 27, 15))
                        {
                            ZetaDia.Me.UsePower(SNOPower.DemonHunter_SmokeScreen, ZetaDia.Me.Position);
                            step = 13;
                            break;
                        }
                        step++;
                        break;
                    case 4:
                        if (MoveSuperFuckingFast(2046, 2527))
                            step++;
                        break;
                    case 5:
                        if (MoveSuperFuckingFast(2078, 2492))
                            step++;
                        break;
                    case 6:
                        if (MoveSuperFuckingFast(2066, 2477))
                            step++;
                        break;
                    case 7:
                        ZetaDia.Me.UsePower(SNOPower.DemonHunter_Preparation, ZetaDia.Me.Position);
                        step++;
                        break;
                    case 8:
                        if (Zeta.ZetaDia.CurrentWorldId == 71150)
                            Interact("g_Portal_Square_Blue");
                        step++;
                        break;
                    case 9:
                        if (Zeta.ZetaDia.CurrentWorldId == 71150)
                            step = 7;
                        else step++;
                        break;
                    case 10:
                        ZetaDia.Me.UsePower(SNOPower.DemonHunter_Companion, ZetaDia.Me.Position);
                        step++;
                        break;
                    case 11:
                        if (MoveSuperFuckingFast(125, 160))
                            step++;
                        break;
                    case 12:
                        if (MoveSuperFuckingFast(119, 132))
                            step++;
                        break;
					case 13:
						break;
                }
				if (ZetaDia.Me.IsDead) step = 13;
                if (step == 13) break;
            }

        }

        public void OnPulse()
        {
        }

        public bool Interact(string name)
        {
            DiaObject unit = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, false).FirstOrDefault(u => u.Name.Contains(name));
            if (unit.Name.Contains(name))
                return unit.Interact();
            else return false;
        }

        public bool Interact(DiaObject u)
        {
            return ZetaDia.Me.UsePower(u.ActorType == Zeta.Internals.SNO.ActorType.Gizmo || u.ActorType == Zeta.Internals.SNO.ActorType.Item ? SNOPower.Axe_Operate_Gizmo : SNOPower.Axe_Operate_NPC, u.Position);
        }

        public bool MoveTo(float x, float y)
        {
			if (ZetaDia.Me.IsDead) { step = 13; }
            if ((float)Math.Sqrt(Math.Pow(ZetaDia.Me.Position.X - x, 2) + Math.Pow(ZetaDia.Me.Position.Y - y, 2)) < 5)
                return true;
            Zeta.Navigation.Navigator.PlayerMover.MoveTowards(new Vector3(x, y, ZetaDia.Me.Position.Z));
            return false;
        }

        public bool MoveSuperFuckingFast(float x, float y)
        {
            while (new Vector3(x, y, ZetaDia.Me.Position.Z).Distance(ZetaDia.Me.Position) > 10 && running && BotMain.IsRunning)
            {
                if (new Vector3(x, y, ZetaDia.Me.Position.Z).Distance(ZetaDia.Me.Position) > 50 && Zeta.CommonBot.PowerManager.CanCast(SNOPower.DemonHunter_Vault))
                {
                    ZetaDia.Me.UsePower(SNOPower.DemonHunter_Vault, new Vector3(x, y, ZetaDia.Me.Position.Z), ZetaDia.Me.WorldDynamicId, 2, -1);
                    Thread.Sleep(350);
                }
                //else if (new Vector3(x, y, ZetaDia.Me.Position.Z).Distance(ZetaDia.Me.Position) > 30 && Zeta.CommonBot.PowerManager.CanCast(SNOPower.DemonHunter_SmokeScreen))
                //{
                //    ZetaDia.Me.UsePower(SNOPower.DemonHunter_SmokeScreen, new Vector3(x, y, ZetaDia.Me.Position.Z), ZetaDia.Me.WorldDynamicId, 2, -1);
                //    MoveTo(x, y);
                //}
                else
                {
                    MoveTo(x, y);
                }
            }
            return true;
        }

    }
}