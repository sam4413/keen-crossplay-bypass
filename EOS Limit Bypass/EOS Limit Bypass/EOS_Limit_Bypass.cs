using HarmonyLib;
using NLog;
using Sandbox;
using Sandbox.Game;
using Sandbox.Game.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
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
using Sandbox.Game.World;            // for MySession
using VRage.Library.Utils;           // for MyPlatformGameSettings
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;
using VRage.Game.ObjectBuilders;
using VRage.Game.ObjectBuilders.Definitions;

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

        public class SetMaxPlayerCountPatch
        {
            static MethodBase TargetMethod()
            {
                var myEOSGameServerType = AccessTools.TypeByName("VRage.EOS.MyEOSGameServer");
                if (myEOSGameServerType == null)
                {
                    return null;
                }
                return AccessTools.Method(myEOSGameServerType, "SetMaxPlayerCount", new[] { typeof(int) });
            }

            static bool Prefix(int count)
            {
                return false;
            }
        }


        [HarmonyPatch(
    typeof(MySession),
    "Load",
    new[] {
        typeof(string),
        typeof(MyObjectBuilder_Checkpoint),
        typeof(ulong),
        typeof(bool),
        typeof(bool)
    }
)]
        public class LoadConsoleCompatBypassPatch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                // point at the static bool MyPlatformGameSettings.CONSOLE_COMPATIBLE
                var consoleField = typeof(MyPlatformGameSettings)
                                        .GetField("CONSOLE_COMPATIBLE",
                                                  BindingFlags.Public | BindingFlags.Static);

                for (int i = 0; i < codes.Count; i++)
                {
                    var ci = codes[i];
                    // look for:      ldc.i4.0
                    //                stsfld bool MyPlatformGameSettings::CONSOLE_COMPATIBLE
                    if (ci.opcode == OpCodes.Stsfld
                        && ci.operand is FieldInfo fi
                        && fi == consoleField)
                    {
                        // blow away the store
                        ci.opcode = OpCodes.Nop;
                        ci.operand = null;

                        // also blow away the preceding ldc.i4.0
                        if (i > 0 && codes[i - 1].opcode == OpCodes.Ldc_I4_0)
                        {
                            codes[i - 1].opcode = OpCodes.Nop;
                            codes[i - 1].operand = null;
                        }
                    }
                }

                return codes;
            }
        }

    }
}
