using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLogicForShader : MonoBehaviour
{
    private Renderer _r;
    private int numberOfHits = 0;
    private Vector4[] hitPoints;
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
        //_r.material.SetVector("_uvHitPoint", newValue);
        Vector4[] newHitPoints = new Vector4[12];

        for(int i = 0; i < numberOfHits; i++)
        {
            newHitPoints[i] = hitPoints[i];
        }

        newHitPoints[numberOfHits] = newValue;

        hitPoints = newHitPoints;

        _r.material.SetVectorArray("_uvHitPoints", hitPoints);
        _r.material.SetVector("_uvHitPoint", newValue);
        _r.material.SetInt("_totalHits", numberOfHits + 1);

        if (numberOfHits < 11)
            numberOfHits++;
        
    }
}
