using TheRevenantsAge;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.Menu.Pause
{
    public class ToMainMenuButton: MonoBehaviour
    {
        [SerializeField] private bool shouldSave = true;
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GlobalMapController.ClearState();
            if(shouldSave)
                GameSession.Current.Save("out");
            GameSession.Current = null;
            SceneTransition.LoadScene(SceneName.MainMenu);
        }
    }
}