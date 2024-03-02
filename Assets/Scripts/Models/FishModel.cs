using UnityEngine;

namespace Models
{
    public class FishModel : IModel
    {
        public ItemRarity Rarity { get; private set; }
        public float DropChance { get; private set; }
        public Color Color { get; private set; }

        public FishModel(ItemRarity rarity)
        {
            Rarity = rarity;
            DropChance = Helpers.RarityHelper.GetDropChance(rarity);;
            Color = Helpers.RarityHelper.GetColor(rarity);
        }
    }
}
