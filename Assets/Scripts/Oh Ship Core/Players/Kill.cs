using System;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kill : MonoBehaviour
{ 

    [Inject] ISceneTransitioner m_sceneTransitioner;


    private void Start()
    {
        FindAnyObjectByType<Injector>().Inject(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Kill")
        {
            m_sceneTransitioner.TransitionToScene("DrownedSequence", 0.5f);
        }
    }
}
