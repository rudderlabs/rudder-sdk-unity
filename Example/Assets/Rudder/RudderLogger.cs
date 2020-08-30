using UnityEngine;

namespace RudderStack
{
  public class RudderLogger
  {

    private static int logLevel = RudderLogLevel.INFO;

    public static void Init(int _logLevel)
    {
      if (_logLevel < RudderLogLevel.NONE)
      {
        _logLevel = RudderLogLevel.NONE;
      }
      if (_logLevel > RudderLogLevel.VERBOSE)
      {
        _logLevel = RudderLogLevel.VERBOSE;
      }
      logLevel = _logLevel;
    }

    public static void LogError(string message)
    {
      if (logLevel >= RudderLogLevel.ERROR)
      {
        Debug.LogError(_WrapMessage("Error", message));
      }
    }

    public static void LogWarn(string message)
    {
      if (logLevel >= RudderLogLevel.WARN)
      {
        Debug.LogWarning(_WrapMessage("Warning", message));
      }
    }

    public static void LogInfo(string message)
    {
      if (logLevel >= RudderLogLevel.INFO)
      {
        Debug.Log(_WrapMessage("Info", message));
      }
    }

    public static void LogDebug(string message)
    {
      if (logLevel >= RudderLogLevel.DEBUG)
      {
        Debug.Log(_WrapMessage("Debug", message));
      }
    }


    private static string _WrapMessage(string type, string message)
    {
      return "RudderSDK: " + type + ": " + message;
    }
  }

  public static class RudderLogLevel
  {
    public static int VERBOSE = 5;
    public static int DEBUG = 4;
    public static int INFO = 3;
    public static int WARN = 2;
    public static int ERROR = 1;
    public static int NONE = 0;
  }
}
