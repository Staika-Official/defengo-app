using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using System;
using System.Linq;

namespace Framework.Util
{
    public class AdjustInitializer : MonoBehaviour
    {
        private static AdjustInitializer instance;
        private static bool isInitialized = false;

        private const string SENT_EVENTS_KEY = "SentAdjustEvents";

        private static Queue<Action> eventQueue = new Queue<Action>();
        private static HashSet<string> sentEventTokens = new HashSet<string>();

        void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("[AdjustInitializer] Unity Editor에서는 AdjustManager 작동 안함");
            return;
#endif

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSentEvents();
                isInitialized = true;
                Log("[AdjustInitializer] Adjust 사용 준비 완료");

                while (eventQueue.Count > 0)
                {
                    eventQueue.Dequeue()?.Invoke();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ------------------------------
        // PUBLIC STATIC API
        // ------------------------------

        public static void TrackEvent(string eventToken)
        {
#if UNITY_EDITOR
            return;
#endif
            if (IsDuplicate(eventToken)) return;

            Action action = () =>
            {
                instance.StartCoroutine(instance.SendBasicEventCoroutine(eventToken));
            };

            if (isInitialized) action();
            else eventQueue.Enqueue(action);
        }

        public static void TrackRevenue(string eventToken, double revenue, string currency = "USD")
        {
#if UNITY_EDITOR
            return;
#endif
            string uniqueId = $"REVENUE_{eventToken}_{revenue}_{currency}";
            if (IsDuplicate(uniqueId)) return;

            Action action = () =>
            {
                instance.StartCoroutine(instance.SendRevenueEventCoroutine(eventToken, revenue, currency));
            };

            if (isInitialized) action();
            else eventQueue.Enqueue(action);
        }

        public static void TrackCustomEvent(string eventToken, Dictionary<string, string> parameters)
        {
#if UNITY_EDITOR
            return;
#endif
            string uniqueId = $"CUSTOM_{eventToken}_{string.Join("_", parameters.Select(p => $"{p.Key}:{p.Value}"))}";
            if (IsDuplicate(uniqueId)) return;

            Action action = () =>
            {
                instance.StartCoroutine(instance.SendCustomEventCoroutine(eventToken, parameters));
            };

            if (isInitialized) action();
            else eventQueue.Enqueue(action);
        }

        // ------------------------------
        // INTERNAL COROUTINES
        // ------------------------------

        private IEnumerator SendBasicEventCoroutine(string token)
        {
            yield return null;

            try
            {
                AdjustEvent ev = new AdjustEvent(token);
                Adjust.trackEvent(ev);
                RegisterSent(token);
                Log($"[AdjustInitializer] 기본 이벤트 전송: {token}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AdjustInitializer] 기본 이벤트 오류: {e.Message}");
            }
        }

        private IEnumerator SendRevenueEventCoroutine(string token, double amount, string currency)
        {
            yield return null;

            try
            {
                AdjustEvent ev = new AdjustEvent(token);
                ev.setRevenue(amount, currency);
                Adjust.trackEvent(ev);
                RegisterSent($"REVENUE_{token}_{amount}_{currency}");
                Log($"[AdjustInitializer] Revenue 전송: {token}, {amount} {currency}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AdjustInitializer] Revenue 이벤트 오류: {e.Message}");
            }
        }

        private IEnumerator SendCustomEventCoroutine(string token, Dictionary<string, string> parameters)
        {
            yield return null;

            try
            {
                AdjustEvent ev = new AdjustEvent(token);
                foreach (var kvp in parameters)
                {
                    ev.addCallbackParameter(kvp.Key, kvp.Value);
                }

                Adjust.trackEvent(ev);
                RegisterSent($"CUSTOM_{token}_{string.Join("_", parameters.Select(p => $"{p.Key}:{p.Value}"))}");
                Log($"[AdjustInitializer] Custom 이벤트 전송: {token} (파라미터 {parameters.Count}개)");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AdjustInitializer] Custom 이벤트 오류: {e.Message}");
            }
        }

        // ------------------------------
        // 중복 방지 (Persistent)
        // ------------------------------

        private static void LoadSentEvents()
        {
            var raw = PlayerPrefs.GetString(SENT_EVENTS_KEY, "");
            if (!string.IsNullOrEmpty(raw))
            {
                sentEventTokens = new HashSet<string>(raw.Split(','));
            }
        }

        private static void RegisterSent(string id)
        {
            sentEventTokens.Add(id);
            PlayerPrefs.SetString(SENT_EVENTS_KEY, string.Join(",", sentEventTokens));
            PlayerPrefs.Save();
        }

        private static bool IsDuplicate(string id)
        {
            if (sentEventTokens.Contains(id))
            {
                Log($"[AdjustInitializer] 중복 이벤트 무시: {id}");
                return true;
            }
            return false;
        }

        private static void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
