using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenEffectFeature : ScriptableRendererFeature {
	class FullScreenEffectPass : ScriptableRenderPass {
		Material effectMaterial;

		public FullScreenEffectPass(Material material) {
			this.effectMaterial = material;
			renderPassEvent = RenderPassEvent.AfterRendering; // 最後に描画
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
			CommandBuffer cmd = CommandBufferPool.Get("FullScreenEffect");

			RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

			cmd.Blit(source, source, effectMaterial);
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
	}

	public enum EffectType {
		Off,
		GritchShader,
		BrightnessContrastSaturation,
		Displacement,
		KaleidoscopeImageEffect,
		Shaker,
		ColorControl,
		DotMatrix,
		Outline,
		Slitscan,
		CutSlider,
		DotScreen,
		Poster,
		Squres,
		DigitalGlitch,
		Film,
		RGBShift,
		Twist,
	}
	public Material gritchShader;
	public Material brightnessContrastSaturation;
	public Material displacement;
	public Material kaleidoscopeImageEffect;
	public Material shaker;
	public Material colorControl;
	public Material dotMatrix;
	public Material outline;
	public Material slitscan;
	public Material cutSlider;
	public Material dotScreen;
	public Material poster;
	public Material squres;
	public Material digitalGlitch;
	public Material film;
	public Material rgbShift;
	public Material twist;

	private EffectType curType = EffectType.Off;
 	private Material effectMaterial;
	FullScreenEffectPass effectPass;

	public EffectType GetEffectType()
	{
		return curType;
	}
	public void SetMatrial(EffectType type)
	{
		switch (type) {
		case EffectType.GritchShader:
			curType = type;
			effectMaterial = gritchShader;
			break;
		case EffectType.BrightnessContrastSaturation:
			curType = type;
			effectMaterial = brightnessContrastSaturation;
			break;
		case EffectType.Displacement:
			curType = type;
			effectMaterial = displacement;
			break;
		case EffectType.KaleidoscopeImageEffect:
			curType = type;
			effectMaterial = kaleidoscopeImageEffect;
			break;
		case EffectType.Shaker:
			curType = type;
			effectMaterial = shaker;
			break;
		case EffectType.ColorControl:
			curType = type;
			effectMaterial = colorControl;
			break;
		case EffectType.DotMatrix:
			curType = type;
			effectMaterial = dotMatrix;
			break;
		case EffectType.Outline:
			curType = type;
			effectMaterial = outline;
			break;
		case EffectType.Slitscan:
			curType = type;
			effectMaterial = slitscan;
			break;
		case EffectType.CutSlider:
			curType = type;
			effectMaterial = cutSlider;
			break;
		case EffectType.DotScreen:
			curType = type;
			effectMaterial = dotScreen;
			break;
		case EffectType.Poster:
			curType = type;
			effectMaterial = poster;
			break;
		case EffectType.Squres:
			curType = type;
			effectMaterial = squres;
			break;
		case EffectType.DigitalGlitch:
			curType = type;
			effectMaterial = digitalGlitch;
			break;
		case EffectType.Film:
			curType = type;
			effectMaterial = film;
			break;
		case EffectType.RGBShift:
			curType = type;
			effectMaterial = rgbShift;
			break;
		case EffectType.Twist:
			curType = type;
			effectMaterial = twist;
			break;
		default:
			curType = EffectType.Off;
			effectMaterial = null;
			break;
		}
		Create();
	}
	public override void Create() {
		effectPass = new FullScreenEffectPass(effectMaterial);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
		if (effectMaterial != null) {
			renderer.EnqueuePass(effectPass);
		}
	}
}