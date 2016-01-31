using UnityEngine;
using System.Collections;

public class SwordThrow : MonoBehaviour
{
    private AOE _aoe;
    private float backBy;

    protected void Awake()
    {
        _aoe = GetComponent<AOE>();
    }

    protected void Start()
    {
        StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        while (true)
        {
            Vector3 targetPoint = Random.onUnitSphere * 4;
            targetPoint.z = 0;
            while ((targetPoint - transform.position).sqrMagnitude > 0.5f)
            {
                transform.position += (targetPoint - transform.position).normalized * Time.deltaTime * 10;

                yield return null;

                if (backBy > 0)
                    targetPoint = PlayerInput.Instance.transform.position;
            }

            if (backBy > 0)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
        
    }

    protected void Update()
    {
        _aoe.DealDamage((int)(Time.deltaTime * 100));

        transform.Rotate(0, 0, Time.deltaTime * 360);
    }

    public void ComeBack(float f)
    {
        backBy = f;
    }
}
