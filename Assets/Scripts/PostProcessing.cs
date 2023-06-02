using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable] public class WirlNoise {
    public Material mat;
    public Texture2D noiseTex;
    public float speed = 0.02f;
    public float amount = 0.02f;
}

[System.Serializable] public class Pixelate {
    public Material mat;
    [Range(64.0f, 512.0f)] public float blockCount = 128;
}

[System.Serializable] public class Vignette {
    public Material mat;
    [Range(0.0f, 1.0f)] public float amount = 0.1f;
}

public class PostProcessing : MonoBehaviour {
    [SerializeField] private WirlNoise whirlNoise = null;
    [SerializeField] private Pixelate pixelate = null;
    [SerializeField] private Vignette vignette = null;

    public WirlNoise globalWhirlNoise {
        get => whirlNoise;
        set => whirlNoise = value;
    }
    public Vignette globalVignette {
        get => vignette;
        set => vignette = value;
    }
    
    private void Start() {
        whirlNoise.mat.SetTexture("_NoiseTex", whirlNoise.noiseTex);
    }

    private void FixedUpdate() {
        whirlNoise.mat.SetFloat("_NoiseOffset", Time.time * whirlNoise.speed);
        whirlNoise.mat.SetFloat("_NoiseAmount", whirlNoise.amount);
        vignette.mat.SetFloat("_VignetteAmount", vignette.amount);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst) {
        //Pixelate calculations
        if (Camera.main == null) throw new Exception("Main Camera is null!");

        float k = Camera.main.aspect;
        Vector2 count = new Vector2(pixelate.blockCount, pixelate.blockCount / k);
        Vector2 size = new Vector2(1.0f / count.x, 1.0f / count.y);
        pixelate.mat.SetVector("BlockCount", count);
        pixelate.mat.SetVector("BlockSize", size);

        //apply all shaders
        RenderTexture tex = RenderTexture.GetTemporary(src.width, src.height);
        RenderTexture tex2 = RenderTexture.GetTemporary(src.width, src.height);
        Graphics.Blit(src, tex, whirlNoise.mat);
        Graphics.Blit(tex, tex2, vignette.mat);
        Graphics.Blit(tex2, dst, pixelate.mat);

        RenderTexture.ReleaseTemporary(tex);
        RenderTexture.ReleaseTemporary(tex2);
    }
}
