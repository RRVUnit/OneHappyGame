using System.Collections;
using UnityEngine;

namespace Main.UI
{
    public class ErrorMarkController : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(HideByTimer());
        }

        private IEnumerator HideByTimer()
        {
            yield return new WaitForSeconds(1f);
            
            Destroy(gameObject);
        }
    }
}