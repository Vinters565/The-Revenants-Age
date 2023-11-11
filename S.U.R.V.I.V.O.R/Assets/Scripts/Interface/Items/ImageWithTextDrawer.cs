using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Items
{
    public class ImageWithTextDrawer : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;

        public void ReDrawItem(Sprite sprite, string text)
        {
            image.sprite = sprite;
            this.text.text = text;
        }
    }
}