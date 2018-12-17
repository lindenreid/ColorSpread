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
    public FloatParameter k = new FloatParameter { value = 1.0f };
    public FloatParameter p = new FloatParameter { value = 1.0f };
    public FloatParameter growthSpeed = new FloatParameter { value = 1.0f };
    public FloatParameter startTime = new FloatParameter { value = 0.0f };
    public Vector4Parameter clickLocation = new Vector4Parameter { value = new Vector4(0,0,0,0) };
}
 
public sealed class ColorSpreadRenderer : PostProcessEffectRenderer<ColorSpread>
{
    public override void Render(PostProcessRenderContext context)
    {
        var projectionMatrix = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);

        var sheet = context.propertySheets.Get(Shader.Find("Custom/ColorGrowth"));

        sheet.properties.SetFloat("_Blend", settings.blend);
        sheet.properties.SetFloat("_MaxSize", settings.size);
        sheet.properties.SetFloat("_K", settings.k);
        sheet.properties.SetFloat("_P", settings.p);
        sheet.properties.SetFloat("_StartTime", settings.startTime);
        sheet.properties.SetFloat("_GrowthSpeed", settings.growthSpeed);
        sheet.properties.SetMatrix("unity_ViewToWorldMatrix", context.camera.cameraToWorldMatrix);
        sheet.properties.SetMatrix("unity_InverseProjectionMatrix", projectionMatrix.inverse);
        sheet.properties.SetVector("_Center", settings.clickLocation);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
