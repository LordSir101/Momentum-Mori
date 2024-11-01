using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class TelegraphedHitbox : MonoBehaviour
{
    
    public float WindupTime { get; set; }
    public float ActiveTime { get; set;}
    public float CooldownTime { get; set;}
    public float WindupTimer { get; set;} = 0f;
    public float CooldownTimer { get; set;} = 0f;

    public float Size { get; set;}
    public float StartingTelegaphPercentSize { get; set;} // is a percent of max size

    public bool attackStarted { get; set;} = false;

    GameObject telegraphSprite; 

    public void Init(float windupTime, float activeTime, float cooldownTime, float size, float startingTelegaphPercentSize = 0f)
    {
        WindupTime = windupTime;
        ActiveTime = activeTime;
        CooldownTime = cooldownTime;
        Size = size;
        StartingTelegaphPercentSize = startingTelegaphPercentSize;
        
        // The sprite that fills up the hitbox to show the player when the attack will come
        telegraphSprite = FindGameObjectInChildWithTag(gameObject, "TelegraphSprite");

        gameObject.GetComponent<Renderer>().enabled = false;
        telegraphSprite.GetComponent<Renderer>().enabled = false;

        transform.localScale = new Vector3(Size, Size, 0);
        telegraphSprite.transform.localScale = new Vector3(startingTelegaphPercentSize, startingTelegaphPercentSize,0);

        Setup();

        StartCoroutine(StartCooldown());
    }

    public abstract void Setup();

    public void Update()
    {
        if(attackStarted)
        {
            WindupTimer += Time.deltaTime;

            float interpolationRatio = WindupTimer / WindupTime;
            float interpolatedScale = Mathf.Lerp(StartingTelegaphPercentSize, 1, interpolationRatio);

            Transform windupSpriteTransform = telegraphSprite.transform;
            windupSpriteTransform.localScale = new Vector3(interpolatedScale, interpolatedScale, 0);

            if(WindupTimer >= WindupTime)
            {
                attackStarted = false;
                WindupTimer = 0f;

                StartCoroutine(ActiveAttackTime());
            }
        }
    }

    public IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(CooldownTime);
        StartAttack();
    }

    public void StartAttack()
    {
        attackStarted = true;
        gameObject.GetComponent<Renderer>().enabled = true;
        telegraphSprite.GetComponent<Renderer>().enabled = true;
    }

    public void EndAttack()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        telegraphSprite.GetComponent<Renderer>().enabled = false;
        StartCoroutine(StartCooldown());
    }

    public virtual IEnumerator ActiveAttackTime()
    {
        yield return new WaitForSeconds(ActiveTime);
        EndAttack();
    }

    public static GameObject FindGameObjectInChildWithTag (GameObject parent, string tag)
	{
		Transform t = parent.transform;

		for (int i = 0; i < t.childCount; i++) 
		{
			if(t.GetChild(i).gameObject.tag == tag)
			{
				return t.GetChild(i).gameObject;
			}
				
		}
			
		return null;
	}

    

    

    

   

    

    
}
