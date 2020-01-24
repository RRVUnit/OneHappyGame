using UnityEngine;

namespace Main.World
{
    public class GameWorldComponent : MonoBehaviour
    {
        private const string MARKED_SPOT_PREFAB_NAME = "Embedded/pfMarkedSpot";
        private const string ERROR_SPOT_PREFAB_NAME = "Embedded/pfErrorSpot";
        
        private GameObject _correctMark;
        private GameObject _errorMark;
        
        private void Awake()
        {
            PreloadMarkers();
        }

        private void PreloadMarkers()
        {
            _correctMark = Resources.Load<GameObject>(MARKED_SPOT_PREFAB_NAME);
            _errorMark = Resources.Load<GameObject>(ERROR_SPOT_PREFAB_NAME);
        }

        public GameObject CorrectMark
        {
            get { return _correctMark; }
        }
        
        public GameObject ErrorMark
        {
            get { return _errorMark; }
        }
    }
}