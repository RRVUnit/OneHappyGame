using System;
using Main.Repository.Model;
using UnityEngine;

namespace Main.Repository
{
    public class GameConfigurationRepository
    {
        private const string GAME_CONFIGURATION_PREFAB_PATH = "Embedded/pfGameConfiguration";
        
        private GameConfiguration _gameConfiguration;
        
        public void Load()
        {
            GameObject configurationObject = Resources.Load<GameObject>(GAME_CONFIGURATION_PREFAB_PATH);
            if (configurationObject == null) {
                throw new Exception("Cant load game configuration. Prefab is missing");
            }
            GameConfiguration gameConfiguration = configurationObject.GetComponent<GameConfiguration>();
            if (gameConfiguration == null) {
                throw new Exception("Game configuration prefab dont contains GameConfiguration behaviour");
            }
            _gameConfiguration = gameConfiguration;
        }

        public GameConfiguration GameConfiguration
        {
            get { return _gameConfiguration; }
        }
    }
}