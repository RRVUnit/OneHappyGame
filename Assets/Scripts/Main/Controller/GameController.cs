using System.Collections.Generic;
using System.Security.Cryptography;
using Main.Model;
using Main.Repository.Model;
using Main.UI;
using Main.World;
using UnityEngine;

namespace Main.Controller
{
    public class GameController : MonoBehaviour
    {
        private GameWorldComponent _world;
        private GameScreenUIComponent _ui;
        private GameConfiguration _configuration;

        private GameStageModel _currentGameStageModel;
        private int _stageIndex;
        
        public void Init(GameWorldComponent world, GameScreenUIComponent ui, GameConfiguration configuration)
        {
            _ui = ui;
            _world = world;
            _configuration = configuration;
        }

        private void InitStageParameters()
        {
            _ui.MaxWarningsCount = _currentGameStageModel.MaxWarningsCount;
            _ui.MaxDifferencesCount = _currentGameStageModel.MaxDifferencesCount;

            _ui.DifferencesFound = 0;
            _ui.WarningsCount = 0;
        }

        public void StartGame()
        {
            NextStage();
        }

        private void NextStage()
        {
            if (!_configuration.HasNextStage(_stageIndex)) {
                ShowWinGameDialog();
                return;
            }
            ClearPrevStage();

            CreateStageModel();
            InitStageParameters();
            LoadStageAssets();
        }

        private void ShowWinGameDialog()
        {
            Debug.Log("GAME OVER");
        }

        private void ClearPrevStage()
        {
            if (FirstPictureContainer != null) {
                Destroy(FirstPictureContainer);
                FirstPictureContainer = null;
            }
            if (SecondPictureContainer != null) {
                Destroy(SecondPictureContainer);
                SecondPictureContainer = null;
            }
        }

        private void LoadStageAssets()
        {
            FirstPictureContainer = CreatePicture(_currentGameStageModel.FirstPicture);
            SecondPictureContainer = CreatePicture(_currentGameStageModel.SecondPicture);

            AlignPictures();
        }

        private void AlignPictures()
        {
            float offset = 0.05F;
            Vector3 bounds = FirstPictureContainer.GetComponentInChildren<SpriteRenderer>().sprite.bounds.extents;
            FirstPictureContainer.transform.position = new Vector3(0, - bounds.y - offset, 0);
            
            bounds = SecondPictureContainer.GetComponentInChildren<SpriteRenderer>().sprite.bounds.extents;
            SecondPictureContainer.transform.position = new Vector3(0, bounds.y + offset, 0);
        }

        private GameObject CreatePicture(Sprite pictureSprite)
        {
            GameObject spriteContainer = new GameObject();
            GameObject pictureContainer = new GameObject("Picture");
            pictureContainer.transform.SetParent(spriteContainer.transform);
            SpriteRenderer spriteRenderer = pictureContainer.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = pictureSprite;
            
            pictureContainer.AddComponent<BoxCollider2D>();
            GamePictureItemClickManager gamePictureItemClickManager = pictureContainer.AddComponent<GamePictureItemClickManager>();
            gamePictureItemClickManager.OnPictureClick += OnPictureClick;

            int matchObjectsCount = 0;
            _currentGameStageModel.Differences.ForEach(d => {
                GameObject matchObject = new GameObject();
                matchObject.name = "MatchObject" + matchObjectsCount;
                
                BoxCollider2D boxCollider2D = matchObject.AddComponent<BoxCollider2D>();
                boxCollider2D.size = new Vector2(d.Size, d.Size);
                
                matchObject.transform.position = d.Position;
            
                GamePictureItemClickManager itemPictureClickManager = matchObject.AddComponent<GamePictureItemClickManager>();
                itemPictureClickManager.OnPictureClick += OnPictureItemClick;

                matchObject.transform.SetParent(spriteContainer.transform);
                
                matchObjectsCount++;
            });
            spriteContainer.transform.SetParent(_world.transform);

            return spriteContainer;
        }

        private void OnPictureItemClick(string goName, Vector3 position)
        {
            if (AlreadyChecked(goName)) {
                return;
            }
            AddFoundMark(goName);
            IncDifferencesFoundCount();
            CheckStageCompleted();
        }

        private bool AlreadyChecked(string goName)
        {
            return _currentGameStageModel.MarkedDifferenceGONames.IndexOf(goName) >= 0;
        }

        private void CheckStageCompleted()
        {
            if (_currentGameStageModel.FoundDifferencesCount >= _currentGameStageModel.MaxDifferencesCount) {
                ShowStageCompletedDialog();
            }
        }

        private void AddFoundMark(string goName)
        {
            _currentGameStageModel.MarkedDifferenceGONames.Add(goName);
        }

        private void IncDifferencesFoundCount()
        {
            ++_currentGameStageModel.FoundDifferencesCount;
            _ui.DifferencesFound = _currentGameStageModel.FoundDifferencesCount;
        }

        private void ShowStageCompletedDialog()
        {
            _stageIndex++;
            
            NextStage();
        }

        private void OnPictureClick(string goName, Vector3 position)
        {
            AddFailureMark();
            IncWarningsCount();
        }

        private void IncWarningsCount()
        {
            ++_currentGameStageModel.WarningsCount;
            _ui.WarningsCount = _currentGameStageModel.WarningsCount;
        }

        private void AddFailureMark()
        {
            
        }

        private void CreateStageModel()
        {
            GameStageConfiguration gameStageConfiguration = _configuration.GetStageConfiguration(_stageIndex);
            GameStageModel gameStageModel = new GameStageModel();
            gameStageModel.Init(gameStageConfiguration);
            
            gameStageModel.MarkedDifferenceGONames = new List<string>();

            _currentGameStageModel = gameStageModel;
        }
        
        private GameObject FirstPictureContainer { get; set; }
        
        private GameObject SecondPictureContainer { get; set; }
    }
}