using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(ColorSpreadRenderer), PostProcessEvent.AfterStack, "Custom/Grayscale")]
public sealed class ColorSpread : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
    public FloatParameter size = new FloatParameter { value = 0.5f };
    public FloatParameter k_color = new FloatParameter { value = 1.0f };
    public FloatParameter p_color = new FloatParameter { value = 1.0f };
    public FloatParameter growthSpeed = new FloatParameter { value = 1.0f };
    public FloatParameter startTime = new FloatParameter { value = 0.0f };
    public Vector4Parameter clickLocation = new Vector4Parameter { value = new Vector4(0,0,0,0) };
    public TextureParameter noiseTex = new TextureParameter { value = null };
    public FloatParameter noiseSize = new FloatParameter { value = 1.0f };
    public FloatParameter noiseTexScale = new FloatParameter { value = 1.0f };
    public ColorParameter startColor = new ColorParameter { value = Color.white };
    public ColorParameter endColor = new ColorParameter { value = Color.white };
    public FloatParameter wiggleStrength = new FloatParameter { value = 0.5f };
    public FloatParameter k_wiggle = new FloatParameter { value = 1.0f };
    public FloatParameter p_wiggle = new FloatParameter { value = 1.0f };
}
 
public sealed class ColorSpreadRenderer : PostProcessEffectRenderer<ColorSpread>
{
    public override void Render(PostProcessRenderContext context)
    {
        var projectionMatrix = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);

        var sheet = context.propertySheets.Get(Shader.Find("Custom/ColorGrowth"));

        sheet.properties.SetFloat("_Blend", settings.blend);
        sheet.properties.SetFloat("_MaxSize", settings.size);
        sheet.properties.SetFloat("_k_color", settings.k_color);
        sheet.properties.SetFloat("_p_color", settings.p_color);
        sheet.properties.SetFloat("_StartTime", settings.startTime);
        sheet.properties.SetFloat("_GrowthSpeed", settings.growthSpeed);
        sheet.properties.SetMatrix("unity_ViewToWorldMatrix", context.camera.cameraToWorldMatrix);
        sheet.properties.SetMatrix("unity_InverseProjectionMatrix", projectionMatrix.inverse);
        sheet.properties.SetVector("_Center", settings.clickLocation);
        sheet.properties.SetTexture("_NoiseTex", settings.noiseTex);
        sheet.properties.SetFloat("_NoiseSize", settings.noiseSize);
        sheet.properties.SetFloat("_NoiseTexScale", settings.noiseTexScale);
        sheet.properties.SetColor("_StartColor", settings.startColor);
        sheet.properties.SetColor("_EndColor", settings.endColor);
        sheet.properties.SetFloat("_WiggleStrength", settings.wiggleStrength);
        sheet.properties.SetFloat("_k_wiggle", settings.k_wiggle);
        sheet.properties.SetFloat("_p_wiggle", settings.p_wiggle);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
