// This script is optional, only for the demo scene

using System.Collections;
using UnityEngine;

namespace KevinIglesias
{
    public class BasicMotionsRootmotionReset : MonoBehaviour
    {
        public float delay = 1f;

        private Vector3 initPosition;

        private void Start()
        {
            initPosition = transform.localPosition;
            if (delay > 0)
            {
                InvokeRepeating("ResetPosition", 0f, delay);
            }
        }

        public void ResetPosition()
        {
            transform.localPosition = initPosition;
        }

        public void ResetPositionFromSMB()
        {
            StartCoroutine(ResetPositionAfterFrame());
        }

        private IEnumerator ResetPositionAfterFrame()
        {
            yield return new WaitForEndOfFrame();
            ResetPosition();
        }
    }
}