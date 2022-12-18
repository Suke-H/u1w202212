using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    const string FirstLevel = "StageSelect";
    [SerializeField] private CustomButton startButton;
    // [SerializeField] private CustomButton soundButton;

    private void Start()
    {

        startButton.onClickCallback = () => { 
            SceneManager.LoadScene(FirstLevel);
        };
        
        // soundButton.onClickCallback = () =>
        // {
        //     SEController.instance.playSE("quote");
        // };
    }

}
