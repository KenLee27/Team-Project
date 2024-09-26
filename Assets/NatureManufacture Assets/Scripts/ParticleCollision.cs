using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public List<int> subEmmiterIds = new List<int>();
    private ParticleSystem _particleSystem;
    private List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem.Particle[] _particles;
    private float _lifetime;
    public float particleDistanceCheck = 0.5f;
    public Vector3 offset = new Vector3(0, 0.3f, 0);

    public bool debug = false;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _lifetime = _particleSystem.main.startLifetime.constant;
        //Debug.Log(lifetime);

        _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
    }

    private void OnParticleCollision(GameObject other)
    {
        _particleSystem.GetCollisionEvents(other, _collisionEvents);

        foreach (ParticleCollisionEvent coll in _collisionEvents)
        {
            if (coll.intersection != Vector3.zero)
            {
                int numParticlesAlive = _particleSystem.GetParticles(_particles);

                // Check only the particles that are alive
                for (int i = 0; i < numParticlesAlive; i++)
                {
                    if (debug)
                        Debug.Log($"{_particles[i].startLifetime} {_lifetime} {Mathf.Abs(_particles[i].startLifetime - _lifetime) < 0.0001f}");

                    if (Mathf.Abs(_particles[i].startLifetime - _lifetime) < 0.0001f)
                    {
                        // Debug.Log(transform.TransformPoint(particles[i].position) - coll.intersection);
                        if (Vector3.Magnitude(transform.TransformPoint(_particles[i].position) - coll.intersection) < particleDistanceCheck)
                        {
                            if (debug)
                                Debug.Log("collision particle " + i + " " + _particles[i].startLifetime);

                            _particles[i].startLifetime -= 0.01f;


                            _particles[i].position += offset;
                            foreach (int subEmmiterId in subEmmiterIds)
                            {
                                if (debug)
                                    Debug.Log($"Emitter triggered {subEmmiterId}");
                                _particleSystem.TriggerSubEmitter(subEmmiterId, ref _particles[i]);
                            }

                            break;
                        }
                    }
                }

                _particleSystem.SetParticles(_particles, numParticlesAlive);
            }
        }
    }
}