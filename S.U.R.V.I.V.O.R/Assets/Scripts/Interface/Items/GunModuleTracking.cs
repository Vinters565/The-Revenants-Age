using UnityEngine;


namespace Interface.Items
{
    public class GunModuleTracking : MonoBehaviour
    {
        private Vector2 onAwakeRectTransformSize;
        private Vector2 onAwakeRectTransformScale;

        public Vector2 OnAwakeRectTransformSize => onAwakeRectTransformSize;

        public Vector2 OnAwakeRectTransformScale => onAwakeRectTransformScale;

        private void Awake()
        {
            var rect = GetComponent<RectTransform>();
            onAwakeRectTransformSize = rect.sizeDelta;
            onAwakeRectTransformScale = rect.localScale;
        }
    }
}