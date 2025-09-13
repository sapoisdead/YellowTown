using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Gradient _gradualDamageGradient;
    [SerializeField] private Image _fill;

    [Header("Drugs")]
    [SerializeField] private Slider _drugSlider;

    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI _ammoAmountTxt;
    private AmmoAndStaminaManager _ammoAndStaminaManager;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _scoreTxt;
    private Wallet _wallet;

    private InteractionHandler _interactionHandler;
    private OD_Monitor _od_Monitor;
    private PlayerHealth _playerHealth;
    private float _drugsConsumedAmmount;

    private void Start()
    {
        // Recupera le referenze di Player (eseguito dopo Awake su tutti gli oggetti)
        _interactionHandler = PlayerAction.Instance.GetComponent<InteractionHandler>();
        _od_Monitor = PlayerAction.Instance.GetComponent<OD_Monitor>();
        _playerHealth = PlayerAction.Instance.GetComponent<PlayerHealth>();
        _ammoAndStaminaManager = PlayerAction.Instance.GetComponent<AmmoAndStaminaManager>();
        _wallet = PlayerAction.Instance.GetComponent<Wallet>();

        // Verifica che tutte le referenze siano valide
        if (_interactionHandler == null || _od_Monitor == null || _playerHealth == null || _ammoAndStaminaManager == null || _wallet == null)
        {
            Debug.LogError("Missing references in UIPlayerStats");
            return;
        }

        // Inizializza i valori della UI
        _healthSlider.maxValue = _playerHealth.GetMaxHealth();
        _fill.color = _gradient.Evaluate(1f);
        UpdateHealthUI();

        _drugSlider.maxValue = _od_Monitor.GetODThreshold();
        _drugsConsumedAmmount = _od_Monitor.GetDrugsConsumedAmmountAtStart();
        _drugSlider.value = _drugsConsumedAmmount;

        _scoreTxt.text = _wallet.Balance.ToString(); // Forza l'aggiornamento dello score
        UpdateAmmoUI(_ammoAndStaminaManager.Ammo);    // Forza l'aggiornamento delle munizioni

        // Iscrizione agli eventi dopo il controllo `null`
        SubscribeToPlayerEvent();
    }

    private void OnDisable()
    {
        UnsubscribeToPlayerEvent();
    }

    private void SubscribeToPlayerEvent()
    {
        _interactionHandler.OnDrugTaken += DrugsHandler_OnDrugTaken;
        _playerHealth.OnTakeDamage += PlayerHealth_OnTakeDamage;
        _playerHealth.OnTakeGradualDamage += PlayerHealth_OnTakeGradualDamage;
        _playerHealth.OnEndGradualDamage += PlayerHealth_OnEndGradualDamage;
        _playerHealth.OnHeal += PlayerHealth_OnHeal;
        _ammoAndStaminaManager.OnAmmoChanged += UpdateAmmoUI;
        _wallet.OnBalanceChanged += Wallet_OnBalanceChanged;
    }

    private void UnsubscribeToPlayerEvent()
    {
        if (_interactionHandler != null) _interactionHandler.OnDrugTaken -= DrugsHandler_OnDrugTaken;
        if (_playerHealth != null)
        {
            _playerHealth.OnTakeDamage -= PlayerHealth_OnTakeDamage;
            _playerHealth.OnTakeGradualDamage -= PlayerHealth_OnTakeGradualDamage;
            _playerHealth.OnEndGradualDamage -= PlayerHealth_OnEndGradualDamage;
            _playerHealth.OnHeal -= PlayerHealth_OnHeal;
        }
        if (_ammoAndStaminaManager != null) _ammoAndStaminaManager.OnAmmoChanged -= UpdateAmmoUI;
        if (_wallet != null) _wallet.OnBalanceChanged -= Wallet_OnBalanceChanged;
    }

    private void PlayerHealth_OnEndGradualDamage(object sender, System.EventArgs e)
    {
        UpdateHealthUI();
    }

    private void Wallet_OnBalanceChanged(int balance)
    {
        _scoreTxt.text = balance.ToString();
    }

    private void DrugsHandler_OnDrugTaken(float dose)
    {
        _drugsConsumedAmmount += dose;
        _drugsConsumedAmmount = Mathf.Clamp(_drugsConsumedAmmount, 0, _drugSlider.maxValue);
        _drugSlider.value = _drugsConsumedAmmount;
    }

    private void PlayerHealth_OnTakeDamage(object sender, System.EventArgs e)
    {
        UpdateHealthUI();
    }

    private void PlayerHealth_OnTakeGradualDamage(object sender, System.EventArgs e)
    {
        UpdateHealthGradualDamageUI();
    }

    private void PlayerHealth_OnHeal(object sender, System.EventArgs e)
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        _healthSlider.value = _playerHealth.GetHealth();
        _fill.color = _gradient.Evaluate(_healthSlider.normalizedValue);
    }

    private void UpdateHealthGradualDamageUI()
    {
        _healthSlider.value = _playerHealth.GetHealth();
        _fill.color = _gradualDamageGradient.Evaluate(_healthSlider.normalizedValue);
    }

    private void UpdateAmmoUI(int ammoAmount)
    {
        _ammoAmountTxt.text = ammoAmount.ToString();
    }
}
