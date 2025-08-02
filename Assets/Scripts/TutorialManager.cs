using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private string sceneName; // Set this in the Inspector
    
    [SerializeField]
    private PickupSelector pickupSelector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pickupSelector.ClearAllPickups();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSceneAfterDelay()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(sceneName);
    }
}
