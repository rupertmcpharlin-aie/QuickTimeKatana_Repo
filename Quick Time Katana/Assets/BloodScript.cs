using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class BloodScript : MonoBehaviour
{
    [SerializeField] GameObject[] bloodSplatterPrefabs;
    [SerializeField] ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        if(numCollisionEvents > 0)
        {
            Collision col = collisionEvents[0].colliderComponent.GetComponent<Collision>();
            Instantiate(bloodSplatterPrefabs[Random.Range(0, bloodSplatterPrefabs.Length)], collisionEvents[0].intersection, Quaternion.LookRotation(col.contacts[0].normal));
        }
    }
}
