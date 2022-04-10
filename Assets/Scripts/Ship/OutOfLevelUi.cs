using UnityEngine;
using static LevelManagement.OutOfLevelNotifierScriptableObject;

namespace Ship
{
    public class OutOfLevelUi : MonoBehaviour
    {

        private OutOfLevelState currentState = OutOfLevelState.NoNoise;
        

        public void HandleNewOutOfLevelState(OutOfLevelState newState)
        {
            if (currentState == newState)
            {
                return;
            }
            
            
        }
    }
}