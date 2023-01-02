using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell {
    [RequireComponent(typeof(Collider2D))]
    public sealed class ParticleEffector2D : MonoBehaviour
    {
        public string colliderTag;
        public GameObject fx_Prefab;
        public Vector3 offset;

        public string soundFileName;
        public float releaseTime = 2f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals(colliderTag)){
                GameObject go = CommonUtils.SpawnObject(fx_Prefab);
                if (go == null || collision == null)
                    return;

                go.transform.position = collision.transform.position+ offset;
                if (soundFileName.Length > 0)
                {
                    CommonUtils.PlaySound(soundFileName);
                }
                CommonUtils.InvokeAction(releaseTime, () =>
                {
                    CommonUtils.ReleaseObject(go);
                });
            }
        }
    }
}