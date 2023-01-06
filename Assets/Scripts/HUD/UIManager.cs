using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Image playerHealthBar;
    public Image torchBar;
    public TextMeshProUGUI scoreText;

    private IPlayer player;
    private float currentPlayerHealth;
    private float currentPlayerHealthPercentage;

    public int currentScore = 0;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<IPlayer>();
    }

    public void SetPlayerHealthBar(float bla)
    {
        currentPlayerHealth = player.Health;
        currentPlayerHealthPercentage = currentPlayerHealth / player.HealthMax;
        playerHealthBar.fillAmount = currentPlayerHealthPercentage;
    }

    public void SetTorchBar(float currentCharge)
    {
        torchBar.fillAmount = currentCharge;
    }

    public void SetScoreText()
    {
        currentScore += 10;
        scoreText.text = currentScore.ToString();
    }
}
