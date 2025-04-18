using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingTarget : MonoBehaviour
{
    [SerializeField]
    private Transform meshTF, textTF;
    [SerializeField]
    private Text damageText;

    private Vector3 wantedPos;
    private Transform myTF;
    private Coroutine textShowCor;
    private bool shaking, textShowing;

    private void Start()
    {
        myTF = transform;
        wantedPos = myTF.position;
    }

    private void Update()
    {
        //target movement
        if (myTF.position == wantedPos)
        {
            wantedPos = new Vector3(Random.Range(-15, 15), Random.Range(0, 9f), 3);
        }

        myTF.position = Vector3.MoveTowards(myTF.position, wantedPos, Time.deltaTime * 8f);
    }

    //assigned to the Invoked OnProjectileImpact event on the Projectile_Impact script on the Target.
    public void Damaged(Transform _attacker, float _damageAmount)
    {
        if (!shaking)
            StartCoroutine(ShakeTarget());

        if (textShowing)
            StopCoroutine(textShowCor);

        damageText.text = _damageAmount.ToString();
        textShowCor = StartCoroutine(ShowDamageText());
    }

    private IEnumerator ShowDamageText()
    {
        textShowing = true;

        float timer = 0.5f;
        textTF.localPosition = Vector3.zero;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            textTF.Translate(Vector3.up * (Time.deltaTime * 3f));

            yield return null;
        }

        damageText.text = "";

        textShowing = false;
    }

    private IEnumerator ShakeTarget()
    {
        shaking = true;

        int duration = 4;

        while (duration > 0)
        {
            duration -= 1;
            meshTF.localPosition = Random.insideUnitSphere * 0.3f;

            yield return new WaitForSeconds(0.02f);
        }

        meshTF.localPosition = Vector3.zero;

        shaking = false;
    }
}
