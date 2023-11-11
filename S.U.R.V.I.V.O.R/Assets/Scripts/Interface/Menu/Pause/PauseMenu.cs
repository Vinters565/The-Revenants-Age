using TheRevenantsAge;
using UnityEngine;

namespace Interface.Menu.Pause
{
    public class PauseMenu : MonoBehaviour, ITurnEndAction
    {
        [SerializeField] private GameObject pauseMenuContainer;
        [SerializeField] private Animator autoSaveLabel;
        private TheRevenantsAge.GlobalMapController globalMapController;
        private static readonly int activate = Animator.StringToHash("Activate");

        private void Awake()
        {
            globalMapController = GlobalMapController.Instance;
        }

        private void OnEnable()
        {
            TurnController.Instance.TurnEnded += OnTurnEnd;
        }

        private void OnDisable()
        {
            TurnController.Instance.TurnEnded -= OnTurnEnd;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (TheRevenantsAge.GlobalMapController.OnPause)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            pauseMenuContainer.gameObject.SetActive(false);
            globalMapController.Resume();
        }

        public void Pause()
        {
            pauseMenuContainer.gameObject.SetActive(true);
            globalMapController.Pause();
        }

        public void OnTurnEnd()
        {
            GameSession.Current.AutoSave();
            autoSaveLabel.SetTrigger(activate);
        }
    }
}