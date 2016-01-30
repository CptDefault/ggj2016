using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance;

    private UIRoot _uiRoot;

    public static Queue<DamageNumber> _pool;

    public GameObject damageNumberPrefab;

    public Color positiveColor;
    public Color negativeColor;

    public Camera mainCamera;
    public Camera guiCamera;

    // guild health
    public GameObject guildHealthPrefab;

    public void Awake()
    {
        Instance = this;
        _pool = new Queue<DamageNumber>();
        _uiRoot = FindObjectOfType<UIRoot>();

        WarmPool();
    }

    public static UISprite GetGuildHealthBar()
    {
        return ((GameObject) (Instantiate(Instance.guildHealthPrefab, Vector3.zero, Quaternion.identity))).GetComponent<UISprite>();
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

        // Convert the position from world to screen so we know where to poistion it
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);

        // need to remove half the width and half the height since our NGUI 0, 0 is in the middle of the screen
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;
        screenPos.x -= (screenWidth / 2.0f);
        screenPos.y -= (screenHeight / 2.0f);
//
        screenPos.x *= (1920f/(float)Screen.width);
        screenPos.y *= (1080f/(float)Screen.height);

        _pool.Dequeue().DisplayNumber(number, screenPos);
    }

    public void Repool(DamageNumber damageNumber)
    {
        _pool.Enqueue(damageNumber);
    }

//    public void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.K))
//            DisplayDamageNumber(Random.Range(-100000, 100000), PlayerInput.Instance.transform.position);
//    }
}
