
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    private int coin;
    [SerializeField] private int coinIncreaseAmount;
    public static CoinManager instance;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        coin = PlayerPrefs.GetInt("Coin", 0);
        UpdateCoinText();
    }

    public void AddCoin()
    {
        coin += coinIncreaseAmount;
        UpdateCoinText();

    }
    public void UpdateCoinText()
    {
        coinText.text = coin.ToString();
    }

    void SaveCoin()
    {
        PlayerPrefs.SetInt("Coin", coin);
    }

    private void OnApplicationQuit()
    {
        SaveCoin();
    }


}
