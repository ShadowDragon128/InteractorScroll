using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;

namespace InteractorScroll
{
    public class InteractorScroll : ResoniteMod
    {
        public override string Name => "InteractorScroll"; //"UIX.Displazor";
        public override string Author => "ShadowAPI";
        public override string Version => "3.1.0";
        public override string Link => "https://github.com/ShadowDragon128/InteractorScroll";

		[AutoRegisterConfigKey]
		public static ModConfigurationKey<float> KEY_SPEED = new ModConfigurationKey<float>("scroll_speed", "How fast you scroll, default is 120.", () => 120);
		public static ModConfiguration config;

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("usmx.ShadowAPI.InspectorScroll");
            harmony.PatchAll();
			config = GetConfiguration();
        }

        [HarmonyPatchCategory(nameof(InteractorScroll))]
        internal class Patch
        {
            /// <summary>
            /// Thanks to art0007i
            /// https://github.com/art0007i/InspectorScroll
            /// </summary>
            /// <param name="__instance"></param>
            [HarmonyPostfix]
            [HarmonyPatch(typeof(InteractionHandler), "OnInputUpdate")]
            public static void OnInputUpdate(InteractionHandler __instance)
            {
                if (__instance.Laser.CurrentTouchable as FrooxEngine.UIX.Canvas == null)
                    return; // NAH!, keep looking.

                IAxisActionReceiver axisActionReceiver = __instance.Laser.CurrentTouchable as IAxisActionReceiver;

                if (axisActionReceiver != null && !__instance.InputInterface.ScreenActive)
                {
                    Elements.Core.float2 val = __instance.Inputs.Axis.Value.Value; // wa la no more reflections needed
                    val *= new Elements.Core.float2(-1, 1);
                    axisActionReceiver.ProcessAxis(__instance.Laser.TouchSource, val * config.GetValue(KEY_SPEED));
                }
                
            }

            /// <summary>
            /// Thanks to zyntaks
            /// https://github.com/furrz/NoTankControls
            /// </summary>
            [HarmonyPostfix]
            [HarmonyPatch(typeof(InteractionHandler), "BeforeInputUpdate")]
            public static void BeforeInputUpdate(InteractionHandler __instance)
            {                
                __instance.Inputs.Axis.RegisterBlocks = ((__instance.LocalUser.IsAppDashOpened && Userspace.Pointers.Count > 1) &&
                    Userspace.Pointers[1].ActiveHandler.Laser.CurrentTouchable as FrooxEngine.UIX.Canvas != null) || 
                    __instance.Laser.CurrentTouchable as FrooxEngine.UIX.Canvas != null; // Make it work with Userspace lasers :D
            }
        }
    }
}