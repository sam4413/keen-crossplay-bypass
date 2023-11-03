using HarmonyLib;
using NLog;
using Sandbox;
using Sandbox.Game;
using Sandbox.Game.World;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;
using VRage.Game;
using VRage.Library.Utils;

namespace EOS_Limit_Bypass
{
    public class EOS_Limit_Bypass : TorchPluginBase
    {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Harmony _harmony = new Harmony("EOS_Limit_Bypass.EOS_Limit_Bypass");

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");

        Log.Warn("Starting EOS Limit Bypass");
        _harmony.PatchAll();
        Log.Warn("Running EOS Limit Bypass");
            
        }

        private void SessionChanged(ITorchSession session, TorchSessionState state)
        {

            switch (state)
            {

                case TorchSessionState.Loaded:
                    Log.Info("Session Loaded!");
                    break;

                case TorchSessionState.Unloading:
                    Log.Info("Session Unloading!");
                    break;
            }
        }
        [HarmonyPatch(typeof(MySession), "PerformPlatformPatchBeforeLoad", new Type[] { typeof(MyObjectBuilder_SessionSettings), typeof(MyGameModeEnum?), typeof(MyOnlineModeEnum?) })]
        class PerformPlatformPatchBeforeLoadPatch
        {
            [HarmonyPrefix]
            private static bool PerformPlatformPatchBeforeLoad()
            {
                return false;
            }
        }
    }
}