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
        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public override string Author { get; } = "Based from @Cyanox62, \"\"\"Improved\"\"\" by @DatPanDat, un-stupid by @User_NonExist";
        public override string Name { get; } = "PlayerReplace";
        public override string Prefix { get; } = "PlayerReplace";
        public override Version Version { get; } = new Version(1, 0, 1);

        public static PlayerReplace Instance;
        public EventHandlers ev;

        public override void OnEnabled()
        {
            Instance = this;


            ev = new EventHandlers();

            Player.Left += ev.OnLeft;
            Server.RoundStarted += ev.OnRoundStart;
        }

        public override void OnDisabled()
        {
            Instance = null;

            Player.Left -= ev.OnLeft;
            Server.RoundStarted -= ev.OnRoundStart;

            ev = null;
        }

    }
}
