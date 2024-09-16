using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CustomARBackgroundRenderFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Insert AR background render logic here
            // This could be a manual call to render the AR background before other objects
        }
    }

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();
        // Set this to render the AR background before rendering transparents or virtual objects
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingSkybox; // Or another earlier phase
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}