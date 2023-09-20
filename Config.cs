using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlayerReplace
{
    public sealed class Config : IConfig
    {
        [Description("If the plugin is enabled or not. (Make sure your disconnect_drop is set to false!)")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debugging?")]
        public bool Debug { get; set; } = false;

        [Description("The roles to not attempt replacing/detect as afk.")]
        public List<RoleTypeId> RestrictedRoles { get; set; } = new()
    {
        RoleTypeId.Spectator,
        RoleTypeId.Tutorial,
        RoleTypeId.Overwatch,
        RoleTypeId.Filmmaker,
    };
        [Description("The text displayed to the player after replacing.")]
        public string ReplacedMessage { get; set; } = "<i>You have replaced a disconnected player.</i>";

        [Description("The duration of message above.")]
        public ushort ReplacedMessageTime { get; set; } = 5;

        [Description("The text displayed to the server if no replacement was found. (Leave empty to turn it off)")]
        public string NoReplaceMessage { get; set; } = "<i>%oldPlayerName% with a role %oldRole% has disconnected without anyone replacing them!</i>";

        [Description("The duration of no replacement message above.")]
        public ushort NoReplaceMessageTime { get; set; } = 8;

        [Description("The death reason that will appear on the ragdoll if player disconnected and no replacement was found.")]
        public string DCDeathReason { get; set; } = "Disconnected from the server.";
    }
}