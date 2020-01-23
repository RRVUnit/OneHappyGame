using UnityEngine;

namespace Main.World
{
    public class GamePictureItemMouseManager : MonoBehaviour
    {
        public delegate void PictureClick(string name, Vector3 position);
        public event PictureClick OnPictureClick;
        
        private void OnMouseUp()
        {
            Vector3 position = transform.position;
            Vector3 globalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localPosition = globalPosition - position;
            localPosition.z = 0;
            
            OnPictureClick?.Invoke(gameObject.name, localPosition);
        }
    }
}