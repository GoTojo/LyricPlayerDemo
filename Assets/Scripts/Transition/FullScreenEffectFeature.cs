using UnityEngine;
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

	private Material effectMaterial;
	FullScreenEffectPass effectPass;

	public void SetMatrial(EffectType type) {
		switch (type) {
		case EffectType.GritchShader:
			effectMaterial = gritchShader;
			break;
		case EffectType.BrightnessContrastSaturation:
			effectMaterial = brightnessContrastSaturation;
			break;
		case EffectType.Displacement:
			effectMaterial = displacement;
			break;
		case EffectType.KaleidoscopeImageEffect:
			effectMaterial = kaleidoscopeImageEffect;
			break;
		case EffectType.Shaker:
			effectMaterial = shaker;
			break;
		case EffectType.ColorControl:
			effectMaterial = colorControl;
			break;
		case EffectType.DotMatrix:
			effectMaterial = dotMatrix;
			break;
		case EffectType.Outline:
			effectMaterial = outline;
			break;
		case EffectType.Slitscan:
			effectMaterial = slitscan;
			break;
		case EffectType.CutSlider:
			effectMaterial = cutSlider;
			break;
		case EffectType.DotScreen:
			effectMaterial = dotScreen;
			break;
		case EffectType.Poster:
			effectMaterial = poster;
			break;
		case EffectType.Squres:
			effectMaterial = squres;
			break;
		case EffectType.DigitalGlitch:
			effectMaterial = digitalGlitch;
			break;
		case EffectType.Film:
			effectMaterial = film;
			break;
		case EffectType.RGBShift:
			effectMaterial = rgbShift;
			break;
		case EffectType.Twist:
			effectMaterial = twist;
			break;
		default:
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