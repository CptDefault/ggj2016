using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AOE : MonoBehaviour
{
    public static readonly List<AOE> ActiveAoes = new List<AOE>();

    public Collider2D Collider;

    public bool HeavyAoe;

    protected void Awake()
    {
        Collider = GetComponent<PolygonCollider2D>();
    }

    protected void OnEnable()
    {
        ActiveAoes.Add(this);
    }
    protected void OnDisable()
    {
        ActiveAoes.Remove(this);
    }
}
