using System;
using GameKit.Util.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public class GameScreenUIComponent : MonoBehaviour
    {
        private const string DIFFERENCIES_COUNT_LABEL_NAME = "DifferenciesCountLabel";
        private const string WARNINGS_COUNT_LABEL_NAME = "WarningsCountLabel";
        private const string FOG_OBJECT = "ModalFog";
        private const string DIALOG_CONTAINER = "DialogContainer";

        private const string STAGE_FAILED_DIALOG_PREFAB = "Embedded/UIDialog/pfStageFailedDialogPrefab";
        private const string STAGE_COMPLETED_DIALOG_PREFAB = "Embedded/UIDialog/pfStageCompletedDialogPrefab";
        private const string GAME_OVER_DIALOG_PREFAB = "Embedded/UIDialog/pfGameOverDialogPrefab";
        
        private Text _differenciesLabel;
        private Text _warningsLabel;

        private GameObject _modalFog;
        private GameObject _dialogContainer;

        public int MaxDifferencesCount { get; set; }
        public int MaxWarningsCount { get; set; }
        
        private void Awake()
        {
            PrefetchComponents();
            HideModalFog();
        }

        private void PrefetchComponents()
        {
            _differenciesLabel = gameObject.GetChildRecursive(DIFFERENCIES_COUNT_LABEL_NAME)?.GetComponent<Text>();
            _warningsLabel = gameObject.GetChildRecursive(WARNINGS_COUNT_LABEL_NAME)?.GetComponent<Text>();

            _modalFog = gameObject.GetChildRecursive(FOG_OBJECT);
            _dialogContainer = gameObject.GetChildRecursive(DIALOG_CONTAINER);
        }
        
        public int DifferencesFound
        {
            set { _differenciesLabel.text = "Отличий: " + value + " / " + MaxDifferencesCount; }
        }
        
        public int WarningsCount
        {
            set { _warningsLabel.text = "Ошибок: " + value + (MaxWarningsCount > 0 ? (" / " + MaxWarningsCount) : ""); }
        }

        public void ShowStageCompletedDialog(Action callback)
        {
            ShowDialog(STAGE_COMPLETED_DIALOG_PREFAB, callback);
        }

        public void ShowWinGameDialog(Action restartGameCallback)
        {
            ShowDialog(GAME_OVER_DIALOG_PREFAB, restartGameCallback);
        }

        public void ShowStageFailedDialog(Action restartStageCallback)
        {
            ShowDialog(STAGE_FAILED_DIALOG_PREFAB, restartStageCallback);
        }

        private void ShowDialog(string dialogPrefab, Action closeCallback)
        {
            ShowModalFog();
            
            GameObject dialogPrefabAsset = Resources.Load<GameObject>(dialogPrefab);
            GameObject dialog = Instantiate(dialogPrefabAsset, _dialogContainer.transform);
            
            CloseDialogCallbackHandler closeHandler = dialog.AddComponent<CloseDialogCallbackHandler>();
            closeHandler.OnCloseClick += () => {
                HideModalFog();

                dialog.RemoveChildren();
                Destroy(dialog);

                closeCallback?.Invoke();
            };
        }

        private void ShowModalFog()
        {
            _modalFog.SetActive(true);
        }

        private void HideModalFog()
        {
            _modalFog.SetActive(false);
        }
    }
}