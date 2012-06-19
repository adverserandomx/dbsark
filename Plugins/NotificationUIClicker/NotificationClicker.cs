using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;

using Zeta;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Internals.Actors;

namespace MBot
{
    public class TestInv : IPlugin 
    {
        public string Author { get { return "adx"; } }

        public string Description { get { return "Annoying UI, it clicks it away.."; } }

        public Window DisplayWindow { get { return null; } }

        public string Name { get { return "Notifcation UI Clicker"; } }

        public System.Version Version { get { return new Version(1, 0); } }

        public bool Equals(IPlugin other)
        {
            return (other.Name == Name) && (other.Version == Version);
        }

        void LogDiags(string message)
        {
            Logging.WriteDiagnostic(string.Format("[{0}] {1}", Name, message));
        }

        void Log(string message)
        {
            Logging.Write(string.Format("[{0}] {1}", Name, message));
        }

        public void OnDisabled()
        {
            Log("Disabled");

        }
        public void OnEnabled()
        {
            Log("Enabled");
        }
        public void OnInitialize()
        {
            return;
        }
        public void OnPulse()
        {
            CheckNotificationUI();
        }
        public void OnShutdown()
        {
            return;

        }

        void CheckNotificationUI()
        {
            try
            {
                Zeta.Internals.UIElement ui = Zeta.Internals.UIElement.FromHash(0x4CC93A73A58BAFFF);
                if (ui != null && ui.IsValid)
                {
                    if (ui.IsVisible)
                    {
                        Log(String.Format("Detected UI = {0}, Visible = {1}", ui.Name, ui.IsVisible));

                        Zeta.Internals.UIElement Button = Zeta.Internals.UIElement.FromHash(0xB4433DA3F648A992);
                        if (Button != null && Button.IsValid)
                        {
                            if (Button.IsVisible)
                            {
                                Log("Notification clicked");
                                Button.Click();
                            }                                
                        }
                    }
                }
            }
            catch (Exception)
            {
            }       
        } 
    }
}
