using UnityEngine;
using UnityEngine.Playables;

public class TimeLineSceneLoader : MonoBehaviour
{
    [SerializeField] private SceneField _nextScene;
    private PlayableDirector _playableDirector;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
        _playableDirector.stopped += OnCutSceneEnded;
    }

    private void OnDisable()
    {
        _playableDirector.stopped -= OnCutSceneEnded;
    }

    private void OnCutSceneEnded(PlayableDirector playableDirector)
    {
        if (playableDirector == _playableDirector)
        {
            GameManager.Instance.TogglePersistentObjects(false);
            LevelManager.LoadScene(_nextScene);
        }
    }
}
