using GameKit.Util.Extension;
using Main.Controller;
using Main.Repository;
using Main.UI;
using Main.World;
using UnityEngine;

namespace Main
{
    public class GameApplication : MonoBehaviour
    {
        private const string GAME_SCREEN_UI_NAME = "GameScreenUI";
        private const string WORLD_NAME = "World";
        
        private GameObject _world;
        private GameObject _ui;

        private GameScreenUIComponent _gameScreenUIComponent;
        private GameWorldComponent _gameWorldComponent;
        private GameController _gameController;
        private GameConfigurationRepository _configurationRepository;

        private void Awake()
        {
            PrefetchObjects();
            Init();
        }

        private void Init()
        {
            _gameScreenUIComponent = _ui.AddComponent<GameScreenUIComponent>();
            _gameWorldComponent = _world.AddComponent<GameWorldComponent>();
            _gameController = gameObject.AddComponent<GameController>();

            _configurationRepository = new GameConfigurationRepository();
            _configurationRepository.Load();
            
            _gameController.Init(_gameWorldComponent, _gameScreenUIComponent, _configurationRepository.GameConfiguration);
            _gameController.StartGame();
        }

        private void PrefetchObjects()
        {
            _ui = gameObject.GetChildRecursive(GAME_SCREEN_UI_NAME);
            _world = gameObject.GetChildRecursive(WORLD_NAME);
        }
    }
}