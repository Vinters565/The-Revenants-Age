﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        public Material outlineMaterial = null;
        RenderTargetHandle temporaryColorTexture;
        public UnityEngine.Rendering.Universal.RenderPassEvent Event;

        public LayerMask layerMask;

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public OutlinePass(Material outlineMaterial, UnityEngine.Rendering.Universal.RenderPassEvent Event, LayerMask layerMask)
        {
            this.outlineMaterial = outlineMaterial;
            this.renderPassEvent = Event;

            this.layerMask = layerMask;
        }



        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

            RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDescriptor.depthBufferBits = 0;


            Camera camera = renderingData.cameraData.camera;
            int outlineLayerMask = ~1 << camera.gameObject.layer; // Битовая маска текущего слоя камеры

            if ((outlineLayerMask & layerMask) != 0) // Проверка, входит ли текущий слой камеры в маску слоев контура
            {
                if (destination == RenderTargetHandle.CameraTarget)
                {
                    cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDescriptor, FilterMode.Point);
                    Blit(cmd, source, temporaryColorTexture.Identifier(), outlineMaterial, 0);
                    Blit(cmd, temporaryColorTexture.Identifier(), source);
                }
                else
                {
                    Blit(cmd, source, destination.Identifier(), outlineMaterial, 0);
                }
            }
            else
            {
                if (destination == RenderTargetHandle.CameraTarget)
                {
                    cmd.Blit(source, destination.Identifier());
                }
                else
                {
                    cmd.Blit(source, destination.Identifier());
                }
            }


            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destination == RenderTargetHandle.CameraTarget)
                cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
        }
    }

    [System.Serializable]
    public class Filter
    {
        public LayerMask layerMask;
    }

    public Filter filter = new Filter();


    [System.Serializable]
    public class OutlineSettings
    {
        public UnityEngine.Rendering.Universal.RenderPassEvent Event = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingTransparents;
        public Material outlineMaterial = null;
    }

    public OutlineSettings settings = new OutlineSettings();
    OutlinePass outlinePass;
    RenderTargetHandle outlineTexture;

    public override void Create()
    {
        outlinePass = new OutlinePass(settings.outlineMaterial, settings.Event, filter.layerMask);
        //outlinePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        outlineTexture.Init("_OutlineTexture");
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.outlineMaterial == null)
        {
            Debug.LogWarningFormat("Missing Outline Material");
            return;
        }
        outlinePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
        renderer.EnqueuePass(outlinePass);
    }
}



//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

//public class OutlineFeature : ScriptableRendererFeature
//{
//    class OutlinePass : ScriptableRenderPass
//    {
//        private RenderTargetIdentifier source { get; set; }
//        private RenderTargetHandle destination { get; set; }
//        public Material outlineMaterial = null;
//        RenderTargetHandle temporaryColorTexture;
//        public UnityEngine.Rendering.Universal.RenderPassEvent Event;

//        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
//        {
//            this.source = source;
//            this.destination = destination;
//        }

//        public OutlinePass(Material outlineMaterial, UnityEngine.Rendering.Universal.RenderPassEvent Event)
//        {
//            this.outlineMaterial = outlineMaterial;
//            this.renderPassEvent = Event;
//        }



//        // This method is called before executing the render pass.
//        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
//        // When empty this render pass will render to the active camera render target.
//        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
//        // The render pipeline will ensure target setup and clearing happens in an performance manner.
//        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
//        {

//        }

//        // Here you can implement the rendering logic.
//        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
//        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
//        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
//        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//        {
//            CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

//            RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
//            opaqueDescriptor.depthBufferBits = 0;

//            if (destination == RenderTargetHandle.CameraTarget)
//            {
//                cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDescriptor, FilterMode.Point);
//                Blit(cmd, source, temporaryColorTexture.Identifier(), outlineMaterial, 0);
//                Blit(cmd, temporaryColorTexture.Identifier(), source);

//            }
//            else Blit(cmd, source, destination.Identifier(), outlineMaterial, 0);

//            context.ExecuteCommandBuffer(cmd);
//            CommandBufferPool.Release(cmd);
//        }

//        /// Cleanup any allocated resources that were created during the execution of this render pass.
//        public override void FrameCleanup(CommandBuffer cmd)
//        {

//            if (destination == RenderTargetHandle.CameraTarget)
//                cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
//        }
//    }

//    [System.Serializable]
//    public class OutlineSettings
//    {
//        public UnityEngine.Rendering.Universal.RenderPassEvent Event = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingTransparents;
//        public Material outlineMaterial = null;
//    }

//    public OutlineSettings settings = new OutlineSettings();
//    OutlinePass outlinePass;
//    RenderTargetHandle outlineTexture;

//    public override void Create()
//    {
//        outlinePass = new OutlinePass(settings.outlineMaterial, settings.Event);
//        //outlinePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
//        outlineTexture.Init("_OutlineTexture");
//    }

//    // Here you can inject one or multiple render passes in the renderer.
//    // This method is called when setting up the renderer once per-camera.
//    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//    {
//        if (settings.outlineMaterial == null)
//        {
//            Debug.LogWarningFormat("Missing Outline Material");
//            return;
//        }
//        outlinePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
//        renderer.EnqueuePass(outlinePass);
//    }
//}


