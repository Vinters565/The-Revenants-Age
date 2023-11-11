using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public sealed class ScrollController : MonoBehaviour
    {
        [SerializeField] private VerticalLayoutGroup container;
        private RectTransform containerRt;

        private IEnumerable<RectTransform> Elements
        {
            get
            {
                var childCount = container.transform.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    yield return (RectTransform) container.transform.GetChild(i).transform;
                }
            }
        }

        void Awake()
        {
            containerRt = container.GetComponent<RectTransform>();
            containerRt.sizeDelta = new Vector2(containerRt.sizeDelta.x, 0);
        }

        public void AddElementEnd(RectTransform elem)
        {
            elem.SetParent(containerRt);
            containerRt.sizeDelta += new Vector2(0,
                elem.sizeDelta.y + container.spacing);
        }
    
        public void AddElementBegin(RectTransform elem)
        {
            AddElementEnd(elem);
            elem.SetAsFirstSibling();
        }

        public void Clear()
        {
            foreach (var elem in Elements.ToArray())
                Destroy(elem.gameObject);
            containerRt.sizeDelta = new Vector2(containerRt.sizeDelta.x, 0);
        }
    }
}