namespace PlayerReplace
{
    using Exiled.API.Interfaces;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class Config : IConfig
    {
        [Description("If the plugin is enabled or not. (Make sure your disconnect_drop is set to false!)")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enable debugging?")]
        public bool Debug { get; set; } = false;

        [Description("The roles to not attempt replacing.")]
        public List<RoleTypeId> RestrictedRoles { get; set; } = new()
    {
        RoleTypeId.Spectator,
        RoleTypeId.Tutorial,
        RoleTypeId.Overwatch,
        RoleTypeId.Filmmaker,
    };

        [Description("The text displayed to the player after replacing.")]
        public string ReplacedMessage { get; set; } = "<i>You have replaced a player who has disconnected.</i>";

        [Description("The duration of message above.")]
        public ushort ReplacedMessageTime { get; set; } = 5;

        [Description("The text displayed to the server if no replacement was found.")]
        public string NonReplaceMessage { get; set; } = "<i><size=32>%oldPlayerName% with a role %oldRole% has disconnected without anyone replacing them!</size></i>";

        [Description("The duration of no replacement message above.")]
        public ushort NonReplaceMessageTime { get; set; } = 8;

        [Description("The death reason that will appear on the ragdoll if no replacement was found.")]
        public string DCDeathReason { get; set; } = "Disconnected from the server.";
    }
}