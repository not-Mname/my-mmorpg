using System.Collections;
using UnityEngine;
namespace Managers
{
    public class CoroutineManager : MonoSingleton<CoroutineManager>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
        }

        public static Coroutine startCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        public static void stopCoroutine(Coroutine routine)
        {
            if (routine != null)
                Instance.StopCoroutine(routine);
        }

        public static void stopAllCoroutines()
        {
            Instance.StopAllCoroutines();
        }
    }
}
