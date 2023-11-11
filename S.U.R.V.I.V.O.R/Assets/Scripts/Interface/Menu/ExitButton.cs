using UnityEngine;
using UnityEngine.UI;

namespace Interface.Menu
{
    public class ExitButton: MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            Application.Quit();
        }
    }
}