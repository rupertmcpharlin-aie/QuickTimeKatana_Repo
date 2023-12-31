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
            Vector3 pos = new Vector3(collisionEvents[0].intersection.x, collisionEvents[0].intersection.y, collisionEvents[0].intersection.z);
            Instantiate(bloodSplatterPrefabs[Random.Range(0, bloodSplatterPrefabs.Length)], pos, Quaternion.identity);
        }
    }
}
