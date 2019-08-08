using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellLifetimeExpiratoin : MonoBehaviour
{
    public Transform shellOrigin;
    public float lifetime = 5;
    // Start is called before the first frame update
    
    private void OnEnable()
    {
        Invoke("DestroyShell", lifetime);
    }
    void DestroyShell()
    {
        Destroy(this.gameObject);
    }

}
