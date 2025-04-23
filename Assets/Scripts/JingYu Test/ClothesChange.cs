using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ClothesChange : MonoBehaviour
{
    public List<SpriteResolver>spriteResolvers = new List<SpriteResolver>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var resolver in FindObjectsOfType<SpriteResolver>()){
            spriteResolvers.Add(resolver);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)){
            foreach (var resolver in spriteResolvers)
            {
                resolver.SetCategoryAndLabel(resolver.GetCategory(),"clo1");
            }
        }
    }
}
