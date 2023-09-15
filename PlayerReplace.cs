using Exiled.API.Features;
using System;
using Exiled.API.Enums;
using Server = Exiled.Events.Handlers.Server;
using Player = Exiled.Events.Handlers.Player;

using PlayerReplace.EventHandler;

namespace PlayerReplace
{
    
    public class PlayerReplace : Plugin<Config>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public override string Author { get; } = "@Cyanox62, Recreate and \"\"\"improved\"\"\" by @DatPanDat";
        public override string Name { get; } = "PlayerReplace";
        public override string Prefix { get; } = "PlayerReplace";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 2, 1);
        public static PlayerReplace Instance { get; private set; }
        public EventHandlers EventHandlers { get; private set; }

        public EventHandlers ev;

        public override void OnEnabled()
        {
            base.OnEnabled();

            if (!Config.IsEnabled) return;

            ev = new EventHandlers();

            Player.Destroying += ev.OnPlayerDestroying;
            Server.RoundStarted += ev.OnRoundStart;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            Player.Destroying -= ev.OnPlayerDestroying;
            Server.RoundStarted -= ev.OnRoundStart;

            ev = null;
        }

    }
}
