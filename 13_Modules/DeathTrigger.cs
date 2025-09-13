using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class DeathTrigger : MonoBehaviour
{
    private BoxCollider2D _boxColl;
    [SerializeField] private SceneField _gameOver;
    [SerializeField] private SceneField _vempaLevel;
    [SerializeField] private GameObject _VempaLevelLogic;

    private void Awake()
    {
        _boxColl = GetComponent<BoxCollider2D>();
        _boxColl.isTrigger = true; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (VempaLevelLogic.Instance.LifeCount == 0)
        {
            MoveToActiveScene(_VempaLevelLogic);
            LevelManager.LoadScene(_gameOver);
        }
        else {
            VempaLevelLogic.Instance.LifeCount--;
            //LevelManager.LoadScene(_vempaLevel);
            PlayerConfiguration.Instance.VempaConfig();
        }
    }

    private void MoveToActiveScene(GameObject objToMove)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(objToMove, activeScene);
    }
}
