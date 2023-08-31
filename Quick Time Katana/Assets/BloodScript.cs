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
            if(collisionEvents[0].colliderComponent.GetComponent<MeshCollider>() != null)
            {
                Instantiate(bloodSplatterPrefabs[Random.Range(0, bloodSplatterPrefabs.Length)], collisionEvents[0].intersection,
                    collisionEvents[0].);
            }
            
        }
    }
}
