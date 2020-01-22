using UnityEngine;

namespace Main.World
{
    public class GamePictureItemClickManager : MonoBehaviour
    {
        public delegate void PictureClick(Vector3 position);

        public event PictureClick OnPictureClick;
        
        private void OnMouseUp()
        {
            OnPictureClick?.Invoke(Input.mousePosition);
        }
    }
}