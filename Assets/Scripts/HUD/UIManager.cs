using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Image healthBar;
    public Image torchBar;
    public TextMeshProUGUI scoreText;

    public IPlayer player;
    public float currentPlayerHealth;
    public float currentPlayerHealthPercentage;
    public int currentScore;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<IPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void SetHealthBar()
    {
        currentPlayerHealth = player.Health;
        currentPlayerHealthPercentage = currentPlayerHealth / player.HealthMax;
        healthBar.fillAmount = currentPlayerHealthPercentage;
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
