using UnityEngine;

namespace RudderStack
{
    public class RudderCache
    {
        private static string cachedAnonymousId = null;
        private static string cachedUserId = null;

        public static void Init()
        {
            if (cachedAnonymousId == null)
            {
                cachedAnonymousId = SystemInfo.deviceUniqueIdentifier;
            }
            if (cachedAnonymousId == null)
            {
#if !UNITY_EDITOR
                cachedUserId = PlayerPrefs.GetString("rl_user_id", null);
#endif
            }
        }

        public static void SetUserId(string userId)
        {
            cachedUserId = userId;
#if !UNITY_EDITOR
                PlayerPrefs.SetString("rl_user_id", userId);
#endif
        }

        public static void Reset()
        {
            cachedUserId = null;
#if !UNITY_EDITOR
                PlayerPrefs.SetString("rl_user_id", null);
#endif
        }

        public static string GetAnonymousId()
        {
            return cachedAnonymousId;
        }

        public static string GetUserId()
        {
            return cachedUserId;
        }
    }
}