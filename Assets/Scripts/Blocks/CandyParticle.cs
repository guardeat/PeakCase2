using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PCase
{
    [RequireComponent(typeof(Image))]
    public class CandyParticle : MonoBehaviour
    {
        private Vector3 _velocity;
        private float _lifetime = 1.0f;
        private Image _image;
        private Vector3 gravity = new Vector3(0, -9.8f, 0);

        void Start()
        {
            float x = Random.Range(-20f, 20f);
            float y = Random.Range(25f, 50f);
            _velocity = new Vector3(x, y, 0);
            _image = GetComponent<Image>();
            Destroy(gameObject, _lifetime);
        }

        void FixedUpdate()
        {
            _velocity += gravity * Time.fixedDeltaTime * 25;
            transform.position += _velocity * Time.fixedDeltaTime * 25;

            Color c = _image.color;
            c.a = Mathf.Lerp(c.a, 0, 5f * Time.fixedDeltaTime);
            _image.color = c;
        }
    }
}