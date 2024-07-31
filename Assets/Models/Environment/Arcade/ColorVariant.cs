using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorVariant : MonoBehaviour
{
    public Color baseColor;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", baseColor); // "_BaseColor" � o nome do par�metro para a cor base em muitos shaders
        renderer.SetPropertyBlock(propBlock);
    }
}

