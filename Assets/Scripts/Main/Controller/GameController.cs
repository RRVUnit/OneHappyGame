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
            
            CreateStageModel();
            InitStartParameters();
            LoadStageAssets();
        }

        private void LoadStageAssets()
        {
            
        }

        private void CreateStageModel()
        {
            GameStageConfiguration gameStageConfiguration = _configuration.GetStageConfiguration(_stageIndex);
            GameStageModel gameStageModel = new GameStageModel();
            gameStageModel.Init(gameStageConfiguration);

            _currentGameStageModel = gameStageModel;
        }
    }
}