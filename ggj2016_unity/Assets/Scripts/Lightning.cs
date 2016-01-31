using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class Lightning : MonoBehaviour
{
    public GameObject lightningEffects;

    private float _timeForLIghtning;

    public AudioClip lightningSound;
    public AudioSource audio;

//    public UISprite lightning1;
//    public UISprite lightning2;

    public GameObject lightningStrikes;

    void Start()
    {
        _timeForLIghtning = Time.time + 2;//Random.Range(5, 10);
    }

	// Update is called once per frame
	void Update () {
	    if (Time.time > _timeForLIghtning)
	    {
            LightningEffect();
	    }
	}

    public void LightningEffect()
    {
        _timeForLIghtning = Time.time + Random.Range(3, 8);        
        StartCoroutine(LightningCoroutine());
    }

    private IEnumerator LightningCoroutine()
    {
        lightningStrikes.SetActive(true);

        yield return new WaitForSeconds(0.1f);

//        lightning1.transform.position = new Vector3(Random.Range(-2,2), 0,0);
//        lightning2.transform.position = new Vector3(Random.Range(-2, 2), 0, 0);
        
        audio.PlayOneShot(lightningSound);
        lightningEffects.SetActive(true);

        yield return new WaitForSeconds(0.05f);

        lightningEffects.SetActive(false);

        yield return new WaitForSeconds(0.05f);

        lightningEffects.SetActive(true);
        audio.PlayOneShot(lightningSound);

        yield return new WaitForSeconds(0.05f);

        lightningEffects.SetActive(false);

        yield return new WaitForSeconds(0.05f);


        audio.PlayOneShot(lightningSound);
        lightningEffects.SetActive(true);

        yield return new WaitForSeconds(0.05f);

        lightningEffects.SetActive(false);

        yield return new WaitForSeconds(0.05f);

        lightningEffects.SetActive(true);
        audio.PlayOneShot(lightningSound);

        yield return new WaitForSeconds(0.05f);

        lightningEffects.SetActive(false);

        yield return new WaitForSeconds(0.2f);


        lightningStrikes.SetActive(false);

    }
}
