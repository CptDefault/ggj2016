using UnityEngine;
using System.Collections;

public class ScreenShakeManager : MonoBehaviour
{
    private static ScreenShakeManager _instance;
    private float amount;
    private Vector3 _startPos;
    private float _realAmount;

    public static void ScreenShake(float amount)
    {
        _instance.amount += amount / 20;
    }
    public static void ScreenShakeContinuous(float amount)
    {
        _instance.amount = amount / 20;
    }

    protected void Awake()
    {
        _instance = this;
        _startPos = transform.position;
    }

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _realAmount = Mathf.Lerp(_realAmount, amount, Time.deltaTime * 6);
	    amount = Mathf.Lerp(amount, 0, Time.deltaTime * 2);
	    transform.position = _startPos + new Vector3(Mathf.Sin(Time.time*12), Mathf.Sin(Time.time*7) * 0.3F)*_realAmount;
	}
}
