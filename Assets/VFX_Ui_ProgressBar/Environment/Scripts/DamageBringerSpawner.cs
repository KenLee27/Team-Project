using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DamageBringerSpawner
{
    class DamageBringerSpawner : MonoBehaviour
    {
        public Vector2 OffsetFrom;
        public Vector2 OffsetTo;
        public float Delay;
        public DamageBringer.DamageBringer Prefab;

        public float DamageMax;
        public float ScaleMax;

        private float time = float.MaxValue;

        private void Update()
        {
            time += Time.deltaTime;
            if (time > Delay)
            {
                Vector2 offset = (OffsetTo - OffsetFrom) * UnityEngine.Random.value + OffsetFrom;
                DamageBringer.DamageBringer o = Instantiate(Prefab, transform);
                o.Damage = DamageMax * UnityEngine.Random.value;
                float scale = o.Damage / DamageMax * ScaleMax;
                o.transform.localScale += new Vector3(scale, scale, scale);
                o.transform.Translate(offset);
                time = 0;
            }
        }
    }
}