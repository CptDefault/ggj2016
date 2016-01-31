using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance;

    private UIRoot _uiRoot;

    public static Queue<DamageNumber> _pool;
    public static Queue<UILabel> _messagePool = new Queue<UILabel>();

    public GameObject damageNumberPrefab;
    public GameObject messagePrefab;

    public Color positiveColor;
    public Color negativeColor;

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
    private static void WarmMessagePool()
    {
        for (int i = 0; i < 25; i++)
        {
            _messagePool.Enqueue(((GameObject)(Instantiate(Instance.messagePrefab, Vector3.zero, Quaternion.identity))).GetComponent<UILabel>());
        }
    }

    public static void DisplayDamageNumber(int number, Vector3 position, bool boss=false)
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

        _pool.Dequeue().DisplayNumber(number, screenPos, boss);
    }

    public static void DisplayMessage(string[] message, Transform track)
    {
        DisplayMessage(message[Random.Range(0, message.Length)], track);
    }

    public static void DisplayMessage(string message, Transform track)
    {
        if (_messagePool.Count == 0)
        {
            WarmMessagePool();
        }

        Instance.StartCoroutine(DisplayTextCoroutine(message, track));
    }
    private static IEnumerator DisplayTextCoroutine(string text, Transform track)
    {
        var uiLabel = _messagePool.Dequeue();
        if(uiLabel.transform.parent == null)
            yield return null;
        uiLabel.gameObject.SetActive(true);
        var color = Color.white;
        color.a = 1;
        uiLabel.text = text;

        for(float t = 0; t < 2.5f; t += Time.deltaTime)
        {
            // Convert the position from world to screen so we know where to poistion it
            Vector3 screenPos = Camera.main.WorldToScreenPoint(track.position);

            // need to remove half the width and half the height since our NGUI 0, 0 is in the middle of the screen
            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            screenPos.x -= (screenWidth/2.0f);
            screenPos.y -= (screenHeight/2.0f);
            //
            screenPos.x *= (1920f/(float) Screen.width);
            screenPos.y *= (1080f/(float) Screen.height);

            uiLabel.transform.localPosition = screenPos + Vector3.up*25;

            yield return null;
        }

        uiLabel.gameObject.SetActive(false);
        DamageNumberManager.Instance.Repool(uiLabel);
    }

    public void Repool(DamageNumber damageNumber)
    {
        _pool.Enqueue(damageNumber);
    }
    public void Repool(UILabel message)
    {
        _messagePool.Enqueue(message);
    }

//    public void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.K))
//            DisplayDamageNumber(Random.Range(-100000, 100000), PlayerInput.Instance.transform.position);
//    }
}
