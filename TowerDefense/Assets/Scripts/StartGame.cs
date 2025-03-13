using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadPlacingScene(){
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadAboutGameScene(){
        SceneManager.LoadScene("AboutGame");
    }
}
