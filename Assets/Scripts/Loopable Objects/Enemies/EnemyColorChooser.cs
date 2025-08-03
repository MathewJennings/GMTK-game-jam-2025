using System.Collections.Generic;
using UnityEngine;

public class EnemyColorChooser : MonoBehaviour
{
    [SerializeField]
    private bool suppressColorChooser = false;

    void Awake()
    {
        if (suppressColorChooser)
        {
            return;
        }
        List<Color> colorChoices = new() {
            ColorPalette.HotPink,
            ColorPalette.VividPurple,
            ColorPalette.BrightYellow
        };

        GetComponentInChildren<SpriteRenderer>().color = colorChoices[Random.Range(0, colorChoices.Count)];
    }
}
