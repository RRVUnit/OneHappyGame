using Main.UI;
using Main.World;
using UnityEngine;

namespace Main.Controller
{
    public class GameController : MonoBehaviour
    {
        private GameWorldComponent _world;
        private GameScreenUIComponent _ui;
        
        public void Init(GameWorldComponent world, GameScreenUIComponent ui)
        {
            _ui = ui;
            _world = world;

            _ui.WarningsCount = 0;
            _ui.DifferenciesCount = 5;
        }
    }
}