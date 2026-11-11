using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float recoveryDelay = 5f;
    public float recoveryRate = 20f;
    public float CurrentEnergy { get; private set; }
    private float recoveryTimer;
    private void Awake()
    {
        CurrentEnergy = maxEnergy;
    }

    private void Update()
    {
        if (recoveryTimer > 0)
        { 
            recoveryTimer -= Time.deltaTime;
        }
        else if (CurrentEnergy < maxEnergy)
        {
            CurrentEnergy += recoveryRate * Time.deltaTime;
            CurrentEnergy = Mathf.Min(CurrentEnergy, maxEnergy);
            Debug.Log(CurrentEnergy);
        }
    }

    public bool Consume(float amount)
    {
        Debug.Log(CurrentEnergy);
        if (CurrentEnergy < amount)
            return false;

        CurrentEnergy -= amount;

        // 消費したら回復待機時間をリセット
        recoveryTimer = recoveryDelay;

        return true;
    }

    public bool HasEnergy(float amount)
    {
        return CurrentEnergy >= amount;
    }
}