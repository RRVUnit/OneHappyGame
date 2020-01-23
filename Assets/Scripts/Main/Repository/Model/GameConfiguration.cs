using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Repository.Model
{
    public class GameConfiguration : MonoBehaviour
    {
        [SerializeField]
        private List<GameStageConfiguration> _stageConfigurations;

        public bool HasNextStage(int stageIndex)
        {
            return _stageConfigurations.Count >= stageIndex + 1;
        }
        
        public GameStageConfiguration GetStageConfiguration(int stageIndex)
        {
            return _stageConfigurations[stageIndex];
        }
    }

    [Serializable]
    public struct GameStageConfiguration
    {
        public int MaxWarningsCount;

        public List<PictureDifferenceSpot> Differences;

        public Sprite FirstPicture;
        public Sprite SecondPicture;
    }

    [Serializable]
    public struct PictureDifferenceSpot
    {
        public float Size;
        public Vector2 Position;
    }
}