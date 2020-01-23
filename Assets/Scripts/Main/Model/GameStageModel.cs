using System.Collections.Generic;
using Main.Repository.Model;
using UnityEngine;

namespace Main.Model
{
    public class GameStageModel
    {
        public void Init(GameStageConfiguration configuration)
        {
            MaxWarningsCount = configuration.MaxWarningsCount;
            MaxDifferencesCount = configuration.Differences.Count;

            FirstPicture = configuration.FirstPicture;
            SecondPicture = configuration.SecondPicture;

            Differences = configuration.Differences;
        }

        public int MaxWarningsCount { get; set; }
        
        public int MaxDifferencesCount { get; set; }

        public Sprite FirstPicture { get; private set; }
        
        public Sprite SecondPicture { get; private set; }
        
        public int WarningsCount { get; set; }
        
        public int FoundDifferencesCount { get; set; }

        public List<PictureDifferenceSpot> Differences { get; private set; }
        
        public List<string> MarkedDifferenceGONames { get; set; }
    }
}