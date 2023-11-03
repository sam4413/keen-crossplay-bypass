# keen-crossplay-bypass
Bypass keen's superimposed limits for EOS (Crossplay) servers! No longer shall you be limited to 3 planets or low sync range!
**Please note this plugin is TORCH only. It will not work on vanilla Dedicated Server.**
Get it for yourself: https://torchapi.com/

## What it does
Essentially what this plugin does is that it seals off the PerformPlatformPatchBeforeLoad method for Crossplay servers, effectively bypassing the limits.

Note: If you already have a config with standard EOS Default config and trying to edit the maximum planets, you will have to edit <MaxPlanets>3</MaxPlanets> to <MaxPlanets>99</MaxPlanets> in your Sandbox_config.sbc and SpaceEngineers_Dedicated.cfg files respectively.

## FYI:
Because the plugin effectively bypasses keen's Crossplay limits, depending on what values you set it to the game may be unstable for some users, or run slower depending on what values you edited. Im not responsible if you manage to corrupt your savegame with this plugin. 

