using Helpers;
using Models;
using UnityEngine;

namespace Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FishView : BaseView<FishModel>
    {
        private const string MATERIALS_FOLDER_PATH = "Materials/";

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Redraw(FishModel model)
        {
            base.Redraw(model);

            var colorName = RarityHelper.GetClosestColorName(model.Color);
            var path = MATERIALS_FOLDER_PATH + colorName;
            var material = Resources.Load<Material>(path);
            _spriteRenderer.material = material;
        }
    }
}
