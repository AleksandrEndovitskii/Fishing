using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class ColorHelper
    {
        private static List<string> _colorNames = new List<string>
        {
            "Red",
            "Green",
            "Blue",
            "White",
            "Black",
            "Yellow",
            "Magenta",
            "Cyan",
            "Gray",
            "Purple",
            "Orange",
        };
        private static List<Color> _predefinedColors = new List<Color>
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.white,
            Color.black,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.gray,
            new Color(0.8f, 0.4f, 1f), // Purple
            new Color(1f, 0.5f, 0.2f), // Orange
        };

        public static string GetColorName(Color color)
        {
            var closestDistance = float.MaxValue;
            var closestColorName = "";

            var inputColorVector = new Vector3(color.r, color.g, color.b);

            foreach (var predefinedColor in _predefinedColors)
            {
                var predefinedColorVector = new Vector3(predefinedColor.r, predefinedColor.g, predefinedColor.b);
                var distance = Vector3.Distance(inputColorVector, predefinedColorVector);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    var index = _predefinedColors.IndexOf(predefinedColor);
                    closestColorName = _colorNames[index];
                }
            }

            return closestColorName;
        }
    }
}
