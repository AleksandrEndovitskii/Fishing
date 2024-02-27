using System.Collections.Generic;
using Models;
using UnityEngine;

namespace Helpers
{
    public static class ItemRarityHelper
    {
        private static Dictionary<ItemRarity, Color> _colorsByRarity = new Dictionary<ItemRarity, Color>
        {
            [ItemRarity.Trash] = Color.gray,
            [ItemRarity.Common] = Color.white,
            [ItemRarity.Uncommon] = Color.green,
            [ItemRarity.Rare] = Color.blue,
            [ItemRarity.Epic] = new Color(0.8f, 0.4f, 1f), // Purple
            [ItemRarity.Legendary] = Color.yellow,
            [ItemRarity.Mythical] = new Color(1f, 0.5f, 0.2f) // Orange
        };

        public static Color GetColor(ItemRarity itemRarity)
        {
            return _colorsByRarity[itemRarity];
        }
    }
}
