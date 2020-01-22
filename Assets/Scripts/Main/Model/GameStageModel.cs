using Main.Repository.Model;
using UnityEngine;

namespace Main.Model
{
    public class GameStageModel
    {
        public void Init(GameStageConfiguration configuration)
        {
            MaxWarningsCount = configuration.MaxWarningsCount;
            DifferencesCount = configuration.DifferencesCount;

            FirstPicture = configuration.FirstPicture;
            SecondPicture = configuration.SecondPicture;
        }

        public int MaxWarningsCount { get; set; }
        
        public int DifferencesCount { get; set; }

        public Sprite FirstPicture { get; private set; }
        
        public Sprite SecondPicture { get; private set; }
    }
}