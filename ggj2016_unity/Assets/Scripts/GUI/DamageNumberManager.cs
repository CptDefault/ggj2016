using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance;

    public static Queue<DamageNumber> _pool;

    public GameObject damageNumberPrefab;

    public void Awake()
    {
        Instance = this;
        _pool = new Queue<DamageNumber>();

        WarmPool();
    }

    private static void WarmPool()
    {
        for (int i = 0; i < 25; i++)
        {
            _pool.Enqueue(((GameObject) (Instantiate(Instance.damageNumberPrefab, Vector3.zero, Quaternion.identity))).GetComponent<DamageNumber>());
        }
    }

    public static void DisplayDamageNumber(int number, Vector3 position)
    {
        if (_pool.Count == 0)
        {
            WarmPool();
        }

        _pool.Dequeue().DisplayNumber(number, position);
    }

    public void Repool(DamageNumber damageNumber)
    {
        _pool.Enqueue(damageNumber);
    }

//    public void Update()
//    {
//        if(Input.GetKeyDown(KeyCode.K)) 
//            DisplayDamageNumber(1219871, Vector3.zero);
//    }
}
