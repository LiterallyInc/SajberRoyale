public class Helper
{
    public static bool isDev = false;
    public static string devhash = "83B7ADDB5720A55C15BC1F567E66D40F202801C94B785F01C3BED80A563D6AF163A7C2F4FA9902A58ADD033AD1405521365827316DF56B0EF9A538E8A8947818";
    /// <summary>
    /// Contains all playerpref-keys used by SajberRoyale settings.
    /// </summary>
    public enum Settings
    {
        /// <summary> (0.0-1.0) Global volume </summary>
        volumeMaster,
        /// <summary> (0/1) Whether or not to play the title screen theme </summary>
        musicTheme,
        /// <summary> (0/1) Whether or not to use Discord RPC </summary>
        discordRpc,
        /// <summary> (0/1) Whether or not to play the game intro </summary>
        playIntro,
        /// <summary> (60-120) In-game FOV </summary>
        fov,
        /// <summary> (0/1) Whether or not the user got access to developer tools </summary>
        isDev

    }
}
