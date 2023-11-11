using UnityEngine;
using UnityEngine.UI;

namespace Interface.Menu
{
    public class OptionButton: MonoBehaviour
    {
        [SerializeField] private Options options;
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            options.gameObject.SetActive(true);
        }
    }
}