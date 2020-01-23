using System.Collections.Generic;
using GameKit.Util.Extension;
using Main.Model;
using Main.Repository.Model;
using Main.UI;
using Main.World;
using UnityEngine;

namespace Main.Controller
{
    public class GameController : MonoBehaviour
    {
        private List<GameObject> CurrentStagePictures { get; set; }
        
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

        public void StartGame()
        {
            NextStage();
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

        private void OnPictureClick(string goName, Vector3 position)
        {
            AddFailureMark(position);
            IncWarningsCount();
        }

        private void InitStageParameters()
        {
            _ui.MaxWarningsCount = _currentGameStageModel.MaxWarningsCount;
            _ui.MaxDifferencesCount = _currentGameStageModel.MaxDifferencesCount;

            _ui.DifferencesFound = 0;
            _ui.WarningsCount = 0;
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
            if (CurrentStagePictures == null || CurrentStagePictures.Count == 0) {
                return;
            }
            CurrentStagePictures.ForEach(p => {
                Destroy(p);
                p = null;
            });
        }

        private void LoadStageAssets()
        {
            CurrentStagePictures = new List<GameObject> {
                    CreatePicture(_currentGameStageModel.FirstPicture),
                    CreatePicture(_currentGameStageModel.SecondPicture)
            };

            AlignPictures();
        }

        private void AlignPictures()
        {
            float offset = 0.05F;
            GameObject firstPicture = CurrentStagePictures[0];
            Vector3 bounds = firstPicture.GetComponentInChildren<SpriteRenderer>().sprite.bounds.extents;
            firstPicture.transform.position = new Vector3(0, - bounds.y - offset, 0);
            
            GameObject secondPicture = CurrentStagePictures[1];
            bounds = secondPicture.GetComponentInChildren<SpriteRenderer>().sprite.bounds.extents;
            secondPicture.transform.position = new Vector3(0, bounds.y + offset, 0);
        }

        private GameObject CreatePicture(Sprite pictureSprite)
        {
            GameObject spriteContainer = new GameObject();
            GameObject pictureContainer = new GameObject("Picture");
            pictureContainer.transform.SetParent(spriteContainer.transform);
            SpriteRenderer spriteRenderer = pictureContainer.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = pictureSprite;
            
            pictureContainer.AddComponent<BoxCollider2D>();
            GamePictureController gamePictureController = pictureContainer.AddComponent<GamePictureController>();
            gamePictureController.OnPictureClick += OnPictureClick;

            CreateItemsContainer("MatchObjects", spriteContainer);
            CreateItemsContainer("ErrorMarksContainer", spriteContainer);
            CreateItemsContainer("MatchMarksContainer", spriteContainer);
            
            int matchObjectsCount = 0;
            _currentGameStageModel.Differences.ForEach(d => {
                GameObject matchObject = new GameObject();
                matchObject.name = "MatchObject" + matchObjectsCount;
                
                BoxCollider2D boxCollider2D = matchObject.AddComponent<BoxCollider2D>();
                boxCollider2D.size = new Vector2(d.Size, d.Size);
                
                matchObject.transform.position = d.Position;
            
                GamePictureController itemPictureClickManager = matchObject.AddComponent<GamePictureController>();
                itemPictureClickManager.OnPictureClick += OnPictureItemClick;

                matchObject.transform.SetParent(spriteContainer.transform);
                
                matchObjectsCount++;
            });
            spriteContainer.transform.SetParent(_world.transform);

            return spriteContainer;
        }

        private void CreateItemsContainer(string containerName, GameObject rootContainer)
        {
            GameObject container = new GameObject(containerName);
            container.transform.position = new Vector3(0, 0, -0.1F);
            container.transform.SetParent(rootContainer.transform);
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
            CurrentStagePictures.ForEach(p => {
                GameObject container = p.RequireChildRecursive("MatchMarksContainer");
                GameObject mark = Instantiate(_world.CorrectMark, container.transform);
            });
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

        private void IncWarningsCount()
        {
            ++_currentGameStageModel.WarningsCount;
            _ui.WarningsCount = _currentGameStageModel.WarningsCount;
        }

        private void AddFailureMark(Vector3 position)
        {
            CurrentStagePictures.ForEach(p => {
                GameObject container = p.RequireChildRecursive("ErrorMarksContainer");
                GameObject mark = Instantiate(_world.ErrorMark, container.transform);
            });
        }

        private void CreateStageModel()
        {
            GameStageConfiguration gameStageConfiguration = _configuration.GetStageConfiguration(_stageIndex);
            GameStageModel gameStageModel = new GameStageModel();
            gameStageModel.Init(gameStageConfiguration);
            
            gameStageModel.MarkedDifferenceGONames = new List<string>();

            _currentGameStageModel = gameStageModel;
        }
    }
}