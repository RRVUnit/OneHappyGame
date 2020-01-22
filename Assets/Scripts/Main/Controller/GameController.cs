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

        private void InitStartParameters()
        {
            _ui.WarningsCount = _currentGameStageModel.MaxWarningsCount;
            _ui.DifferencesCount = _currentGameStageModel.DifferencesCount;
        }

        public void StartGame()
        {
            NextStage();
        }

        private void NextStage()
        {
            if (!_configuration.HasNextStage(_stageIndex)) {
                Debug.Log("Game over. You win.");
                return;
            }
            ClearPrevStage();
            
            CreateStageModel();
            InitStartParameters();
            LoadStageAssets();
        }

        private void ClearPrevStage()
        {
            if (FirstPictureContainer != null) {
                Destroy(FirstPictureContainer);
            }
            if (SecondPictureContainer != null) {
                Destroy(SecondPictureContainer);
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

            BoxCollider2D boxCollider2D = spriteContainer.AddComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(1f, 1f);
            
            GamePictureItemClickManager itemPictureClickManager = spriteContainer.AddComponent<GamePictureItemClickManager>();
            itemPictureClickManager.OnPictureClick += OnPictureItemClick;

            spriteContainer.transform.SetParent(_world.transform);

            return spriteContainer;
        }

        private void OnPictureItemClick(Vector3 position)
        {
            Debug.Log("item clicked : " + position);
        }

        private void OnPictureClick(Vector3 position)
        {
            Debug.Log("clicked : " + position);
        }

        private void CreateStageModel()
        {
            GameStageConfiguration gameStageConfiguration = _configuration.GetStageConfiguration(_stageIndex);
            GameStageModel gameStageModel = new GameStageModel();
            gameStageModel.Init(gameStageConfiguration);

            _currentGameStageModel = gameStageModel;
        }
        
        private GameObject FirstPictureContainer { get; set; }
        
        private GameObject SecondPictureContainer { get; set; }
    }
}