using UnityEngine;

namespace Models
{
    public class FishModel : IModel
    {
        public ItemRarity Rarity { get; private set; }
        public float DropChance { get; private set; }
        public Color Color { get; private set; }

        public FishModel(ItemRarity rarity, float dropChance)
        {
            Rarity = rarity;
            DropChance = dropChance;
            var color = Helpers.ItemRarityHelper.GetColor(Rarity);
            Color = color;
        }
    }
}
