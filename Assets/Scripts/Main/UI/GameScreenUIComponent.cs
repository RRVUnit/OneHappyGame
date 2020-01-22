using GameKit.Util.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public class GameScreenUIComponent : MonoBehaviour
    {
        private const string DIFFERENCIES_COUNT_LABEL_NAME = "DifferenciesCountLabel";
        private const string WARNINGS_COUNT_LABEL_NAME = "WarningsCountLabel";
        
        private Text _differenciesLabel;
        private Text _warningsLabel;

        private void Awake()
        {
            PrefetchComponents();
        }

        private void PrefetchComponents()
        {
            _differenciesLabel = gameObject.GetChildRecursive(DIFFERENCIES_COUNT_LABEL_NAME)?.GetComponent<Text>();
            _warningsLabel = gameObject.GetChildRecursive(WARNINGS_COUNT_LABEL_NAME)?.GetComponent<Text>();
        }
        
        public int DifferencesCount
        {
            set { _differenciesLabel.text = "Отличий: " + value; }
        }
        
        public int WarningsCount
        {
            set { _warningsLabel.text = "Ошибок: " + value; }
        }
    }
}