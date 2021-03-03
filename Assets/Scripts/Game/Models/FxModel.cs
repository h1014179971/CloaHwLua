using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System;

namespace Delivery
{
    public class FxModel : MonoBehaviour
    {

        private ParticleSystem[] _particles;
        private float _maxt;//粒子总时间
        private float _t;
        private Action _callback;
        private bool _isLoop;

        // Use this for initialization
        void Start()
        {
            InitData();
        }
        void InitData()
        {
            if (_particles == null || _particles.Length <= 0)
            {
                _maxt = 0;
                _particles = this.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < _particles.Length; i++)
                {
                    float dunration = 0f;
                    if (_particles[i].emission.rateOverTime.constantMax > 0)
                    {
                        dunration = _particles[i].main.startDelay.constantMax + _particles[i].main.duration + _particles[i].main.startLifetime.constantMax;

                    }
                    else
                    {
                        dunration = _particles[i].main.startDelay.constantMax + Mathf.Max(_particles[i].main.duration, _particles[i].main.startLifetime.constantMax);
                    }
                    _maxt = Mathf.Max(_maxt, dunration);
                }
            }


        }
        public void Init(Transform trans = null, Action back = null, bool loop = false, float dunration = -1)
        {
            _callback = back;
            _isLoop = loop;
            if (trans != null)
            {
                transform.SetParent(trans);
                //transform.localEulerAngles = Vector3.zero;
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
            }
            RectTransform rectTrans = this.GetComponent<RectTransform>();
            if (rectTrans)
            {
                rectTrans.anchoredPosition = Vector3.zero;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector3.zero;
                rectTrans.localScale = Vector3.one;
            }                   
            InitData();
            if (dunration == -1)
                _t = _maxt;
            else
                _t = Math.Max(0, dunration);
            Play();
            DelayStop(_t);
        }                                          
        public void Play()
        {
            for (int i = 0; i < _particles.Length; i++)
                _particles[i].Play();
        }
        //void Update()
        //{
        //    if (_isLoop) return;
        //    if (_t > 0)
        //    {
        //        _t -= Time.deltaTime;
        //        if (_t < 0)
        //        {
        //            Stop();
        //        }
        //    }
        //}
        public void DelayStop(float delay = 1f)
        {
            Timer.Instance.Register(delay, (param) => { Stop(); });
        }
        public void Stop()
        {
            if (_callback != null)
            {
                _callback();
                _callback = null;
            }
            SG.ResourceManager.Instance.ReturnObjectToPool(gameObject);
        }

        private void OnDestroy()
        {
            //Stop();
        }

    }
}

