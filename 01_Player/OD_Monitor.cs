using UnityEngine;

public class OD_Monitor : MonoBehaviour
{
    [SerializeField] private SceneField _bossFight; 

    private InteractionHandler interactionHandler;
    private readonly float _OD_Threshold = 100;
    private readonly float _drugsConsumedAmmountAtStart;
    private float _drugsConsumedAmmount;

    private void Start()
    {
        interactionHandler = PlayerAction.Instance.GetComponent<InteractionHandler>();
        interactionHandler.OnDrugTaken += DrugsHandler_OnDrugTaken;

        ResetDosage();
        _drugsConsumedAmmount = _drugsConsumedAmmountAtStart;
    }

    private void DrugsHandler_OnDrugTaken(float dose)
    {
        _drugsConsumedAmmount += dose;
        Mathf.Clamp(_drugsConsumedAmmount, 0, _OD_Threshold);

        CheckDrugConsumption();
    }

    private void CheckDrugConsumption()
    {
        if (_drugsConsumedAmmount == _OD_Threshold)
        {
            LevelManager.LoadScene(_bossFight);
        }
    }
    
    public void ResetDosage()
    {
        _drugsConsumedAmmount = 0f;
    }

    public float GetDrugsConsumedAmmountAtStart()
    {
        return _drugsConsumedAmmountAtStart; 
    }

    public float GetODThreshold()
    {
        return _OD_Threshold;
    }
}
