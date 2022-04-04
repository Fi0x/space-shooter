using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UpgradeSystem
{
    [CreateAssetMenu(fileName = "new UpgradeSpriteLookup", menuName = "UpgradeSystem/SpriteLookup")]
    public class UpgradeSpriteLookupSO : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<UpgradeNames, Sprite> sprites = new SerializedDictionary<UpgradeNames, Sprite>();

        [SerializeField] private List<UpgradeSprite> upgradeSprites = new List<UpgradeSprite>();

        public Sprite GetSprite(UpgradeNames type)
        {
            if (sprites.ContainsKey(type))
            {
                return sprites[type];
            }
            return null;
        }

        [ContextMenu("Populate Dictionary")]
        public void PopulateDictionary()
        {
            sprites.Clear();
            foreach (var upgradeSprite in upgradeSprites)
            {
                sprites[upgradeSprite.type] = upgradeSprite.sprite;
            }
        }

        [ContextMenu("GenerateList")]
        public void GenerateList()
        {
            upgradeSprites.Clear();
            foreach (var type in (UpgradeNames[])Enum.GetValues(typeof(UpgradeNames)))
            {
                upgradeSprites.Add(new UpgradeSprite(type));
            }
        }
    }

    [Serializable]
    public class UpgradeSprite
    {
        public UpgradeNames type;
        public Sprite sprite;

        public UpgradeSprite(UpgradeNames type)
        {
            this.type = type;
        }
    }
}
