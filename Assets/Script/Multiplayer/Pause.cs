using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Pause : MonoBehaviour
{
    public static bool pause = false;
    bool disconnecting = false;


    public void TogglePause()
    {
        if (disconnecting) return;

        pause = !pause;

        transform.GetChild(0).gameObject.SetActive(pause);
        Cursor.lockState = (pause) ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = pause;
    }

    public void Quit()
    {
        disconnecting = true;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
