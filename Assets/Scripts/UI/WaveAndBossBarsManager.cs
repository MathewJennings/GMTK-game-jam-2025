using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class WaveAndBossBarsManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> waveBarGameObjects;

    [SerializeField]
    List<GameObject> bossBarGameObjects;

    public void SetWaveBarActive()
    {
        foreach (var gameObject in waveBarGameObjects)
        {
            gameObject.SetActive(true);
        }
        foreach (var gameObject in bossBarGameObjects)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetBossBarActive()
    {
        foreach (var gameObject in waveBarGameObjects)
        {
            gameObject.SetActive(false);
        }
        foreach (var gameObject in bossBarGameObjects)
        {
            gameObject.SetActive(true);
        }
    }
}
