using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class RestartGame : MonoBehaviour
    {
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(onButtonClick);
        }

        private void onButtonClick()
        {
            SceneManager.LoadScene("Game");
        }
    }
}