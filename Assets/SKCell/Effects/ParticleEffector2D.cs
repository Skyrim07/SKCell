using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell {
    [RequireComponent(typeof(Collider2D))]
    [AddComponentMenu("SKCell/Effects/ParticleEffector2D")]
    public sealed class ParticleEffector2D : MonoBehaviour
    {
        public ParticleEffector2DMode mode = ParticleEffector2DMode.SpawnRelease;
        public string colliderTag;

        [Header("Spawn & Release")]
        public GameObject fx_Prefab;
        public bool triggerEnter = true, triggerStay = false;

        public bool spawnAtSelfPosition;
        public Vector3 offset;
        public string soundFileName;
        public float releaseTime = 2f;

        [Header("Play & Stop")]
        public ParticleSystem fx_Particle;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!triggerEnter)
                return;
            if (collision.tag.Equals(colliderTag)){

                if (mode == ParticleEffector2DMode.SpawnRelease)
                {
                    GameObject go = SKUtils.SpawnObject(fx_Prefab);
                    if (go == null || collision == null)
                        return;

                    if (spawnAtSelfPosition)
                        go.transform.position = transform.position+offset;
                    else
                        go.transform.position = collision.transform.position + offset;
                    if (soundFileName.Length > 0)
                    {
                        SKUtils.PlaySound(soundFileName);
                    }
                    SKUtils.InvokeAction(releaseTime, () =>
                    {
                        SKUtils.ReleaseObject(go);
                    });
                }
                else if (mode == ParticleEffector2DMode.PlayStop)
                {
                    fx_Particle.Play();
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!triggerEnter)
                return;
            if (collision.tag.Equals(colliderTag))
            {

                if (mode == ParticleEffector2DMode.PlayStop)
                {
                    fx_Particle.Stop();
                }
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!triggerStay)
                return;

            if (collision.tag.Equals(colliderTag))
            {
                if (mode == ParticleEffector2DMode.SpawnRelease)
                {
                    GameObject go = SKUtils.SpawnObject(fx_Prefab);
                    if (go == null || collision == null)
                        return;

                    go.transform.position = collision.transform.position + offset;
                    if (soundFileName.Length > 0)
                    {
                        SKUtils.PlaySound(soundFileName);
                    }
                    SKUtils.InvokeAction(releaseTime, () =>
                    {
                        SKUtils.ReleaseObject(go);
                    });
                }
            }
        }
    }

    public enum ParticleEffector2DMode
    {
        SpawnRelease,
        PlayStop
    }
}