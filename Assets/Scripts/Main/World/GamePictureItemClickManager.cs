using UnityEngine;

namespace Main.World
{
    public class GamePictureItemClickManager : MonoBehaviour
    {
        public delegate void PictureClick(string name, Vector3 position);
        public event PictureClick OnPictureClick;
        
        private void OnMouseUp()
        {
            OnPictureClick?.Invoke(gameObject.name, Input.mousePosition);
        }
    }
}