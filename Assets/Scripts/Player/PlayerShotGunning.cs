﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotGunning : MonoBehaviour
{
    public int damagePerShot = 200;                  
    public float timeBetweenBullets = 0.15f;        
    public float range = 100f;
    public GameObject prefabEffect;

    float timer;                                    
    Ray shootRay;                                   
    RaycastHit shootHit;                            
    int shootableMask;                             
    ParticleSystem gunParticles;                    
    LineRenderer gunLine;                           
    AudioSource gunAudio;                           
    Light gunLight;                                 
    float effectsDisplayTime = 0.2f;

    int numBullet = 7;
    List<GameObject> effects = new List<GameObject>();

    void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();

        for (int i=0;i<numBullet;i++)
        {
            var instance = Instantiate(prefabEffect, transform);
            instance.transform.SetParent(transform, false);
            instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            effects.Add(instance);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
        {
            Shoot();
        }

        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects();
        }
    }

    public void DisableEffects()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;

        foreach (var effect in effects)
        {
            effect.GetComponent<Light>().enabled = false;
            effect.GetComponent<LineRenderer>().enabled = false;
        }
    }

    void Shoot()
    {
        timer = 0f;

        gunAudio.Play();

        gunParticles.Stop();
        gunParticles.Play();

        var rangeAngle = 30;

        for (int i = 0; i < numBullet; i++)
        {
            var gunLight = effects[i].GetComponent<Light>();
            var gunLine = effects[i].GetComponent<LineRenderer>();

            gunLight.enabled = true;
            gunLine.enabled = true;

            gunLine.SetPosition(0, transform.position);
            shootRay.origin = transform.position;

            var customDir = UnityEngine.Random.Range(-rangeAngle, rangeAngle);
            shootRay.direction = Quaternion.Euler(0, customDir, 0) * transform.forward;

            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                }

                gunLine.SetPosition(1, shootHit.point);
            }
            else
            {
                gunLine.SetPosition(1, shootRay.direction * range);
            }
        }
    }
}