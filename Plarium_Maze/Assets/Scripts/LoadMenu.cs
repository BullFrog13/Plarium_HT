using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(onButtonClick);
    }

    private void onButtonClick()
    {
        SceneManager.LoadScene("Menu");
    }
}
