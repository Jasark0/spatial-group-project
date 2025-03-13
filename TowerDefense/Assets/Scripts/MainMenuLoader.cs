using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update

    public void LoadMainMenu(){
        SceneManager.LoadScene("Main Menu");
    }
    public void LoadPlacingScene(){
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadAboutGameScene(){
        SceneManager.LoadScene("AboutGame");
    }
}
