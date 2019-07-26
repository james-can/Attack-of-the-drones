using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLogicForShader : MonoBehaviour
{
    private Renderer _r;
    //private MaterialPropertyBlock _propBlock;
    // Start is called before the first frame update
    void Awake()
    {
        _r = GetComponent<Renderer>();
        //_propBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateShaderDamage(Vector4 newValue)
    {
        //_r.GetPropertyBlock(_propBlock);
        //_propBlock.SetVector("_uvHitPoint", newValue);
        _r.material.SetVector("_uvHitPoint", newValue);
    }
}
