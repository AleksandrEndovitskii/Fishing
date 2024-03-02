using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Helpers
{
    public static class RarityHelper
    {
        private static Dictionary<Color, string> _colorNames = new Dictionary<Color, string>
        {
            [Color.gray] = "Gray",
            [Color.white] = "White",
            [Color.green] = "Green",
            [Color.blue] =  "Blue",
            [new Color(0.8f, 0.4f, 1f)] = "Purple",
            [Color.yellow] = "Yellow",
            [new Color(1f, 0.5f, 0.2f)] = "Orange",
        };
        private static Dictionary<ItemRarity, Color> _itemRarityColors = new Dictionary<ItemRarity, Color>
        {
            [ItemRarity.Trash] = Color.gray,
            [ItemRarity.Common] = Color.white,
            [ItemRarity.Uncommon] = Color.green,
            [ItemRarity.Rare] = Color.blue,
            [ItemRarity.Epic] = new Color(0.8f, 0.4f, 1f), // Purple
            [ItemRarity.Legendary] = Color.yellow,
            [ItemRarity.Mythical] = new Color(1f, 0.5f, 0.2f) // Orange
        };
        private static Dictionary<ItemRarity, float> _itemRarityDropChances = new Dictionary<ItemRarity, float>
        {
            [ItemRarity.Trash] = 100f,
            [ItemRarity.Common] = 50f,
            [ItemRarity.Uncommon] = 20f,
            [ItemRarity.Rare] = 10f,
            [ItemRarity.Epic] = 5f,
            [ItemRarity.Legendary] = 2f,
            [ItemRarity.Mythical] = 1f
        };

        public static Color GetColor(ItemRarity itemRarity)
        {
            var color = _itemRarityColors[itemRarity];

            return color;
        }
        public static string GetClosestColorName(Color color)
        {
            var closestDistanceBetweenColors = float.MaxValue;
            var closestColorName = "";

            var inputColorVector = new Vector3(color.r, color.g, color.b);

            foreach (var colorName in _colorNames)
            {
                var predefinedColorVector = new Vector3(colorName.Key.r, colorName.Key.g, colorName.Key.b);
                var distanceBetweenColors = Vector3.Distance(inputColorVector, predefinedColorVector);

                if (distanceBetweenColors < closestDistanceBetweenColors)
                {
                    closestDistanceBetweenColors = distanceBetweenColors;
                    closestColorName = colorName.Value;
                }
            }

            return closestColorName;
        }
        public static float GetDropChance(ItemRarity rarity)
        {
            var dropChance = _itemRarityDropChances[rarity];

            return dropChance;
        }
    }
}
