using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProjectileAbilityParticle : AbilityParticle
{
    [SerializeField] float speed = 2.0f;
    [SerializeField] float arcHeight = 1.0f;
    [SerializeField] GameObject hitVFX;
    [SerializeField] AudioClip hitSFX;

    public override void Setup(Unit targetUnit)
    {
        StartCoroutine(HandleMovement(new Vector3(targetUnit.transform.position.x, transform.position.y, targetUnit.transform.position.z)));
    }

    IEnumerator HandleMovement(Vector3 targetPos)
    {
        float t = 0.0f;
        var startPos = transform.position;

        while (t < 1.0f)
        {
            t += Time.deltaTime * speed;

            float parabola = 1.0f - 4.0f * (t - 0.5f) * (t - 0.5f);
            var pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += parabola * arcHeight;
            transform.position = pos;

            yield return null;
        }

        if (hitVFX != null)
        {
            Instantiate(hitVFX, transform.position, Quaternion.identity);
        }

        if (hitSFX != null)
        {
            AudioManager.Instance.PlaySFX(hitSFX);
        }

        Destroy(gameObject);
    }
}
