using System;
using System.Collections;
using UnityEngine;

namespace Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine InvokeActionAfterDelay(this MonoBehaviour monoBehaviour,
            Action action, float delaySecondsCount)
        {
            var invokeActionAfterDelayCoroutine =
                monoBehaviour.StartCoroutine(InvokeActionAfterDelayCoroutine(action, delaySecondsCount));

            return invokeActionAfterDelayCoroutine;
        }

        public static void InvokeActionPeriodically(this MonoBehaviour monoBehaviour,
            Action action, float intervalSeconds = 1f)
        {
            var invokeActionPeriodicallyCoroutine =
                monoBehaviour.StartCoroutine(InvokeActionPeriodicallyCoroutine(action, intervalSeconds));
        }

        private static IEnumerator InvokeActionAfterDelayCoroutine(Action action, float delaySecondsCount)
        {
            yield return new WaitForSecondsRealtime(delaySecondsCount);

            action?.Invoke();
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
