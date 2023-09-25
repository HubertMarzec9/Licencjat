using UnityEngine;
using UnityEngine.UI;

public class CloseApplication : MonoBehaviour
{
    private void Start()
    {
        Button closeButton = GetComponent<Button>();
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ExitGame);
        }
    }

    private void ExitGame()
    {
        Debug.Log("Koniec");
        Application.Quit();
    }
}
