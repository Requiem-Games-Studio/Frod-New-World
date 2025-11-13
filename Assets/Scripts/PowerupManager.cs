using System.Collections.Generic;
using UnityEngine;

public enum PowerType { Hand, Weapon, Leg, DoubleJump, Dash, Light, Balloon, Worm, SuperJump, WallClimb, DarkFire, Freeze }

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance; // Singleton
    private HashSet<PowerType> unlockedPowers = new HashSet<PowerType>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // opcional, si quieres persistir entre escenas
    }

    // Desbloquear un poder
    public void UnlockPower(PowerType power)
    {
        if (!unlockedPowers.Contains(power))
        {
            unlockedPowers.Add(power);
            Debug.Log("Power unlocked: " + power);
        }
    }

    // Verificar si ya tengo el poder
    public bool HasPower(PowerType power)
    {
        return unlockedPowers.Contains(power);
    }
}
