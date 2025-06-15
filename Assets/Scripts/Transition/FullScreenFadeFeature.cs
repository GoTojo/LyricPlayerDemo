using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenFadeFeature : ScriptableRendererFeature {
	class FullScreenFadePass : ScriptableRenderPass {
		Material fadeMaterial;

		public FullScreenFadePass(Material material) {
			this.fadeMaterial = material;
			renderPassEvent = RenderPassEvent.AfterRendering; // 最後に描画
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
			CommandBuffer cmd = CommandBufferPool.Get("FullScreenFade");

			RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

			cmd.Blit(source, source, fadeMaterial);
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}
	}

	public Material fadeMaterial;
	FullScreenFadePass fadePass;

	public override void Create() {
		fadePass = new FullScreenFadePass(fadeMaterial);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
		if (fadeMaterial != null) {
			renderer.EnqueuePass(fadePass);
		}
	}
}