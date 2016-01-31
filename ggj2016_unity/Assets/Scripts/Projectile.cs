using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public float Speed = 8;

    // Use this for initialization
	void Start ()
	{
	    StartCoroutine(FlyToBoss());
	}

    private IEnumerator FlyToBoss()
    {
        var startPos = transform.position;
        float dist = Vector3.Distance(startPos, PlayerInput.Instance.transform.position);

        var offset = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(.6f, 1.4f), -1);

        for (float t = 0; t < 1; t += Time.deltaTime / dist * Speed )
        {
            var bossPos = PlayerInput.Instance.transform.position + offset;
            transform.position = Vector3.Lerp(startPos, bossPos, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * dist * 0.1f;

            yield return null;
        }

        Destroy(gameObject);
    }
}
