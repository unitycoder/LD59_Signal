using UnityEngine;

public class Restar : MonoBehaviour
{
    void Update()
    {

            if (Input.GetKeyDown(KeyCode.R))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

    }
}
