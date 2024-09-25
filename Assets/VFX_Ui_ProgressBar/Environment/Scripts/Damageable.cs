using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
namespace Damageable
{


    class Damageable : MonoBehaviour
    {
        public float Health;

        public Renderer ProgressBar;
        public float ProgressBarMin;
        public float ProgressBarMax;
        public Color ZeroHealthColor;
        private float ProgressbarDelta;
        private float HealthMax;
        private Color InitialColor;
        public Renderer Renderer;
        public float DamageDelay;
        public string ProgressBarMaterialVarName = "_ProgressBar";

        private void Start()
        {
            ProgressbarDelta = ProgressBarMax - ProgressBarMin;
            ToHealth = HealthMax = Health;
            InitialColor = Renderer.material.color;
            UpdateProgressBar();
        }

        private void OnCollisionEnter(Collision collision)
        {
            DamageBringer.DamageBringer dmg = collision.gameObject.GetComponent<DamageBringer.DamageBringer>();
            if (dmg == null)
                return;

            TakeDamage(dmg.Damage);
            DestroyObject(dmg.gameObject);
        }

        public void TakeDamage(float dmg)
        {
            FromHealth = Health;
            ToHealth = Health - dmg;
            HealthAnimTime = 0;
        }

        private float FromHealth;
        private float ToHealth;
        private float HealthAnimTime;
        public void UpdateProgressBar()
        {
            float perc = Health / HealthMax;
            float progress = perc * ProgressbarDelta + ProgressBarMin;
            int propId = Shader.PropertyToID(ProgressBarMaterialVarName);
            ProgressBar.material.SetFloat(propId, progress);
            Renderer.material.color = Color.Lerp(ZeroHealthColor, InitialColor, perc);
        }

        void Update()
        {
            if (Math.Abs(Health - ToHealth) < 0.0001f)
                return;
            HealthAnimTime += Time.deltaTime;
            Health = Mathf.Lerp(FromHealth, ToHealth, HealthAnimTime / DamageDelay);
            if (Health < 0)
            {
                ToHealth = Health = HealthMax;
                HealthAnimTime = 0;
            }
            UpdateProgressBar();
        }
    }
}
