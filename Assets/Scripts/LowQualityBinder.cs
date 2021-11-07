using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LowQualityBinder : MonoBehaviour
{
    private BallController ball;
    [SerializeField]
    private TextMeshProUGUI levelText = null;

    void Start()
    {
        ball = GetComponent<BallController>();
    }

    void Update()
    {
        if (ball.collidedWithHole)
            levelText.text = (ball.currentLevel - 1).ToString();
    }
}
