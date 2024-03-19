using System;
using System.Collections;
using UnityEngine;

namespace Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void InvokeActionPeriodically(this MonoBehaviour monoBehaviour,
            Action action, float intervalSeconds = 1f)
        {
            var invokeActionPeriodicallyCoroutine =
                monoBehaviour.StartCoroutine(InvokeActionPeriodicallyCoroutine(action, intervalSeconds));
        }

        private static IEnumerator InvokeActionPeriodicallyCoroutine(Action action, float intervalSeconds)
        {
            while (true)
            {
                action?.Invoke();

                yield return new WaitForSeconds(intervalSeconds); // WaitForSecondsRealtime
            }
        }
    }
}
