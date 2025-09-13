using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    [SerializeField] private SceneField _cutScene; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LevelManager.LoadScene(_cutScene);
    }
}
