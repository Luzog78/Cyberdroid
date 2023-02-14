using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public bool broken = false;

    public GameObject brokenObject;
    public AnimatorController animatorController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (broken)
        {
            Destroy(gameObject);
            GameObject brokenVersion = Instantiate(brokenObject, transform.position, transform.rotation);
            brokenVersion.transform.Rotate(new Vector3(0, 90, 0));
            if (animatorController != null)
            {
                if (brokenVersion.GetComponent<Animator>() == null)
                    brokenVersion.AddComponent<Animator>();
                brokenVersion.GetComponent<Animator>().runtimeAnimatorController = animatorController;
            }
            for(int i = 0; i < brokenVersion.transform.childCount; i++)
            {
                Transform child = brokenVersion.transform.GetChild(i);
                child.GetChild(0).gameObject.GetOrAddComponent<Rigidbody>().AddForce(child.localPosition * .12f, ForceMode.Impulse);
            }
        }
    }
}
