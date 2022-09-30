namespace HiGames.Framework.Utilities
{
    public static class ExtensionMethods
{
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }
}
}
