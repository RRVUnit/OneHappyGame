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
        private const string MATCH_OBJECTS_CONTAINER_NAME = "MatchObjects";
        private const string ERROR_MARKS_CONTAINER_NAME = "ErrorMarksContainer";
        private const string MATCH_MARKS_CONTAINER_NAME = "MatchMarksContainer";
        private const string PICTURE_CONTAINER_NAME = "PictureContainer";
        private const string DIFF_SPRITE_CONTAINER_NAME = "DiffSpriteContainer";
        private const string MATCH_OBJECT_NAME_PREFIX = "MatchObject";
        
        private List<GameObject> CurrentStagePictures { get; set; }
        
        private GameWorldComponent _world;
        private GameScreenUIComponent _ui;
        private GameConfiguration _configuration;

        private GameStageModel _currentGameStageModel;
        private int _stageIndex;

        private bool Paused { get; set; }

        public void Init(GameWorldComponent world, GameScreenUIComponent ui, GameConfiguration configuration)
        {
            _ui = ui;
            _world = world;
            _configuration = configuration;
        }

        public void StartGame()
        {
            _stageIndex = 0;
            NextStage();
        }

        private void OnPictureItemClick(string goName, Vector3 position)
        {
            if (Paused) {
                return;
            }
            if (AlreadyChecked(goName)) {
                return;
            }
            GameObject go = _world.gameObject.RequireChildRecursive(goName);
            
            AddFoundMark(goName);
            
            _currentGameStageModel.MarkedDifferenceGONames.Add(goName);
            
            IncDifferencesFoundCount();
            CheckStageCompleted();
        }

        private void OnPictureClick(string goName, Vector3 position)
        {
            if (Paused) {
                return;
            }
            
            AddFailureMark(position);
            IncWarningsCount();

            CheckStageFailed();
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
            ClearPrevStage();

            CreateStageModel();
            InitStageParameters();
            LoadStageAssets();

            Paused = false;
        }

        private void ShowWinGameDialog()
        {
            Paused = true;
            
            _ui.ShowWinGameDialog(OnRestartGame);
        }

        private void OnRestartGame()
        {
            StartGame();
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
            const float PICTURES_OFFSET = 0.05F;
            const float GLOBAL_OFFSET = -.2F;
            GameObject firstPicture = CurrentStagePictures[0];
            Vector3 bounds = firstPicture.GetComponentInChildren<SpriteRenderer>().sprite.bounds.extents;
            firstPicture.transform.position = new Vector3(0, - bounds.y - PICTURES_OFFSET + GLOBAL_OFFSET, 0);
            
            GameObject secondPicture = CurrentStagePictures[1];
            bounds = secondPicture.GetComponentInChildren<SpriteRenderer>().sprite.bounds.extents;
            secondPicture.transform.position = new Vector3(0, bounds.y + PICTURES_OFFSET + GLOBAL_OFFSET, 0);
        }

        private GameObject CreatePicture(Sprite pictureSprite)
        {
            GameObject spriteContainer = new GameObject(DIFF_SPRITE_CONTAINER_NAME);
            GameObject pictureContainer = new GameObject(PICTURE_CONTAINER_NAME);
            pictureContainer.transform.SetParent(spriteContainer.transform);
            SpriteRenderer spriteRenderer = pictureContainer.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = pictureSprite;
            
            pictureContainer.AddComponent<BoxCollider2D>();
            GamePictureItemMouseManager gamePictureItemMouseManager = pictureContainer.AddComponent<GamePictureItemMouseManager>();
            gamePictureItemMouseManager.OnPictureClick += OnPictureClick;

            CreateItemsContainer(MATCH_OBJECTS_CONTAINER_NAME, spriteContainer);
            CreateItemsContainer(ERROR_MARKS_CONTAINER_NAME, spriteContainer);
            CreateItemsContainer(MATCH_MARKS_CONTAINER_NAME, spriteContainer);
            
            int matchObjectsCount = 0;
            GameObject matchMarksContainer = spriteContainer.RequireChildRecursive(MATCH_OBJECTS_CONTAINER_NAME);
            _currentGameStageModel.Differences.ForEach(d => {
                GameObject matchObject = new GameObject();
                matchObject.name = MATCH_OBJECT_NAME_PREFIX + matchObjectsCount;
                
                BoxCollider2D boxCollider2D = matchObject.AddComponent<BoxCollider2D>();
                boxCollider2D.size = d.Dimentions;
                
                matchObject.transform.position = d.Position;
            
                GamePictureItemMouseManager itemPictureClickManager = matchObject.AddComponent<GamePictureItemMouseManager>();
                itemPictureClickManager.OnPictureClick += OnPictureItemClick;

                matchObject.transform.SetParent(matchMarksContainer.transform, false);
                
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
            if (_currentGameStageModel.FoundDifferencesCount < _currentGameStageModel.MaxDifferencesCount) {
                return;
            }
            ShowStageCompletedDialog();
        }

        private void CheckStageFailed()
        {
            if (_currentGameStageModel.WarningsCount == 0 || _currentGameStageModel.WarningsCount <= _currentGameStageModel.MaxWarningsCount) {
                return;
            }
            ShowStageFailedDialog();
        }

        private void AddFoundMark(string goName)
        {
            CurrentStagePictures.ForEach(p => {
                GameObject go = p.GetChildRecursive(goName);
                BoxCollider2D colliderObject = go.GetComponent<BoxCollider2D>();
                GameObject container = p.RequireChildRecursive(MATCH_MARKS_CONTAINER_NAME);
                GameObject mark = Instantiate(_world.CorrectMark, container.transform);

                Transform markBg = mark.RequireChildRecursive(CheckMarkController.MARK_BG).transform;
                markBg.localScale = colliderObject.size;
                mark.transform.position = go.transform.position;
            });
        }

        private void IncDifferencesFoundCount()
        {
            ++_currentGameStageModel.FoundDifferencesCount;
            _ui.DifferencesFound = _currentGameStageModel.FoundDifferencesCount;
        }

        private void ShowStageCompletedDialog()
        {
            Paused = true;
            _stageIndex++;
            if (!_configuration.HasNextStage(_stageIndex)) {
                ShowWinGameDialog();
                return;
            }
            _ui.ShowStageCompletedDialog(NextStage);
        }

        private void ShowStageFailedDialog()
        {
            Paused = true;
            
            _ui.ShowStageFailedDialog(NextStage);
        }
        
        private void IncWarningsCount()
        {
            ++_currentGameStageModel.WarningsCount;
            _ui.WarningsCount = _currentGameStageModel.WarningsCount;
        }

        private void AddFailureMark(Vector3 position)
        {
            CurrentStagePictures.ForEach(p => {
                GameObject container = p.RequireChildRecursive(ERROR_MARKS_CONTAINER_NAME);
                GameObject mark = Instantiate(_world.ErrorMark, container.transform, false);
                mark.transform.localPosition = position;
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