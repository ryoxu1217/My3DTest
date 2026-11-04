using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    public float maxEnergy = float.MaxValue;
    public float recoveryDelay = 5f;
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
            if (recoveryTimer <= 0)
            {
                CurrentEnergy = maxEnergy;
            }
        }
    }

    public bool Consume(float amount)
    {
        if (CurrentEnergy < amount)
        {
            CurrentEnergy = 0;
            recoveryTimer = recoveryDelay;
            return false;
        }

        CurrentEnergy -= amount;

        if (CurrentEnergy <= 0)
        {
            CurrentEnergy = 0;
            recoveryTimer = recoveryDelay;
        }

        return true;
    }

    public void Recover(float amount)
    {
        CurrentEnergy =
            Mathf.Clamp(CurrentEnergy + amount, 0, maxEnergy);
    }

    public bool HasEnergy(float amount)
    {
        return CurrentEnergy >= amount;
    }
}
