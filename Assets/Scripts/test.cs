using UnityEngine;

public class test : MonoBehaviour
{
    private Renderer rend;
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        Debug.Log(rend.material.color.a);
    }
}
