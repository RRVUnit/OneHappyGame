using System;
using UnityEngine;

namespace Main.World
{
    public class GameWorldComponent : MonoBehaviour
    {
        private GameObject _correctMark;
        private GameObject _errorMark;
        
        private void Awake()
        {
            PreloadMarkers();
        }

        private void PreloadMarkers()
        {
            _correctMark = Resources.Load<GameObject>("Embedded/pfMarkedSpot");
            _errorMark = Resources.Load<GameObject>("Embedded/pfErrorSpot");
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