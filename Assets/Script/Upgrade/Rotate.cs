using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShopSystem
{
    public class Rotate : MonoBehaviour
    {


        [SerializeField] private float rotSpeed;

        private void Update()
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
        }
    }
}
