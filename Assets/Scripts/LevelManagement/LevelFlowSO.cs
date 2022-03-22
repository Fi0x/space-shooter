using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

namespace LevelManagement
{
    [CreateAssetMenu(fileName = "newLevelFlowSettings", menuName = "Level/FlowSettings", order = 0)]
    public class LevelFlowSO : ScriptableObject
    {
        public List<string> standardLevelOrder;
        public List<string> bossLevelOrder;
        public string mainMenuName;

        public string GetNextScene(int index)
        {
            if (index < 0) return mainMenuName;
            int i = index % (standardLevelOrder.Count + 1);
            if (i == standardLevelOrder.Count)
            {
                //boss level
                i = index / (standardLevelOrder.Count + 1);
                return bossLevelOrder[i % bossLevelOrder.Count];
            }
            return standardLevelOrder[i];
        }
    }
}