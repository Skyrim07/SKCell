using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
    [AddComponentMenu("SKCell/Movement/RandomWalk")]
    public sealed class RandomWalk : MonoBehaviour
    {
        public float speed;
        public float radius;
        public float turnInterval = 2f;

        private Vector3 oPos, dir;
        private float timer;
        private void Start()
        {
            oPos = transform.position;
            dir = new Vector3(Random.value, Random.value, 0);
        }
        private void Update()
        {
            timer += Time.deltaTime;
            float dist = oPos.SimpleDistance(transform.position);
            if (timer > turnInterval)
            {
                if (dist >= radius)
                {
                    dir = Vector3.Normalize(oPos - transform.position);
                }
                else
                {
                    dir = Vector3.Normalize(new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0));
                }

                timer = 0;
            }
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }
}
