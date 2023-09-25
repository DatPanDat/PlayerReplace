using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using MEC;

namespace PlayerReplace.API.Features.ExternalRoles;

public class CiSpyRole : ExternalRoleChecker
{
    public override void Init(Assembly assembly)
    {
        PluginEnabled = true;
        Assembly = assembly;

        Log.Debug("CiSpy assembly attached.");
    }

    public override bool IsRole(Player player)
    {
        if (!PluginEnabled)
            return false;

        var apiType = Assembly.GetType("CiSpy.API.API");
        
        object instance = Activator.CreateInstance(apiType);
        
        var isSpyMethod = apiType.GetMethod("IsSpy");
        
        if (isSpyMethod == null)
        {
            Log.Error("DC: CiSpy API method IsSpy not found.");
            return false;
        }
        
        var isSpy = (bool) isSpyMethod.Invoke(instance, new object[] {player, nameof(PlayerReplace)});
        
        return isSpy;
    }

    public override void SpawnRole(Player oldPlayer, Player newPlayer)
    {
        if (!PluginEnabled)
            return;
        
        var apiType = Assembly.GetType("CiSpy.API.API");
        
        object instance = Activator.CreateInstance(apiType);
        
        var isNtfSpyMethod = apiType.GetMethod("IsNtfSpy");
        var isChaosSpyMethod = apiType.GetMethod("IsChaosSpy");
        
        if (isNtfSpyMethod == null)
        {
            Log.Error("DC: CiSpy API method GetNtfSpyList not found.");
            return;
        }
        
        if (isChaosSpyMethod == null)
        {
            Log.Error("DC: CiSpy API method GetChaosSpyList not found.");
            return;
        }
        
        bool isNtfSpy = (bool) isNtfSpyMethod.Invoke(instance, new object[] {oldPlayer, nameof(PlayerReplace)});
        bool isChaosSpy = (bool) isChaosSpyMethod.Invoke(instance, new object[] {oldPlayer, nameof(PlayerReplace)});
        
        if (isNtfSpy)
        {
            var spawnNtfSpyMethod = apiType.GetMethod("SpawnNtfSpy");
            
            if (spawnNtfSpyMethod == null)
            {
                Log.Error("DC: CiSpy API method GetSpawnNtfSpy not found.");
                return;
            }
            
            spawnNtfSpyMethod.Invoke(instance, new object[] {newPlayer, nameof(PlayerReplace)});
            Log.Debug($"DC: Sucessfully spawned {newPlayer.Nickname} as NTF Spy.");
        }
        else if (isChaosSpy)
        {
            var spawnChaosSpyMethod = apiType.GetMethod("SpawnChaosSpy");
            
            if (spawnChaosSpyMethod == null)
            {
                Log.Error("DC: CiSpy API method GetSpawnChaosSpy not found.");
                return;
            }
            
            spawnChaosSpyMethod.Invoke(instance, new object[] {newPlayer, nameof(PlayerReplace)});
            Log.Debug($"DC: Sucessfully spawned {newPlayer.Nickname} as Chaos Spy.");
        }

        Timing.CallDelayed(0.5f, () =>
        {
            newPlayer.SessionVariables["Damagable"] = oldPlayer.SessionVariables["Damagable"];
            newPlayer.SessionVariables["ShootedAsSpy"] = oldPlayer.SessionVariables["ShootedAsSpy"];
        });
    }
}