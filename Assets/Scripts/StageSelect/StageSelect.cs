using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    const string FirstLevel = "Stage";
    [SerializeField] private CustomButton stageButton;
    
    private void Start()
    {
        stageButton.onClickCallback = () => { 
            SceneManager.LoadScene(FirstLevel);
        };
    }


}
