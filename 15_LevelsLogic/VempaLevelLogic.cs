using UnityEngine;

public class VempaLevelLogic : MonoBehaviour
{
    public static VempaLevelLogic Instance { get; private set; }

    public int LifeCount { get; set; } = 5;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("There's more than one Instance of VempaLevelLogic");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        PlayerAction.Instance.IsInteracting = false;
    }


}
