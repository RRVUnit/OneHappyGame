using System.Collections;
using GameKit.Util.Extension;
using UnityEngine;

namespace Main.UI
{
    public class CheckMarkController : MonoBehaviour
    {
        public const string MARK_BG = "CheckMarkBg";
        public const string MARK_ICON = "CheckMarkIcon";
        
        private GameObject _checkGameObject;
        
        private void Start()
        {
            _checkGameObject = gameObject.GetChildRecursive(MARK_ICON);
            
            StartCoroutine(HideByTimer());
        }

        private IEnumerator HideByTimer()
        {
            yield return new WaitForSeconds(1f);
            
            Destroy(_checkGameObject);
        }
    }
}