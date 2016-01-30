using UnityEngine;
using System.Collections;

public class CooldownUI : MonoBehaviour
{
    public static CooldownUI Instance;

    public UILabel[] abilityCooldownLabels;

    protected void Awake()
    {
        Instance = this;
    }

}
