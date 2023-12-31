using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;


namespace CR
{
    public enum CleanOutlineDebugMode
    {
        Off,
        Depth,
        Normal,
        DepthAndNormal
    }

    [Serializable]
    public sealed class CleanOutlineDebugModeParameter : VolumeParameter<CleanOutlineDebugMode>
    {
        public CleanOutlineDebugModeParameter(CleanOutlineDebugMode value, bool overrideState = false) : base(value, overrideState) { }
    }
    
    public enum CleanOutlineDepthSample
    {
        FiveTiles,
        NineTiles
    }
    
    [Serializable]
    public sealed class OutlineDepthSampleParameter : VolumeParameter<CleanOutlineDepthSample>
    {
        public OutlineDepthSampleParameter(CleanOutlineDepthSample value, bool overrideState = false) : base(value, overrideState) {}
    }

    [Serializable, VolumeComponentMenu("Post-processing/Custom/CleanOutline")]
	public sealed class CleanOutline : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [Tooltip("Controls the intensity of the effect.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public FloatParameter outlineThickness = new FloatParameter(1f, true);
        public ColorParameter outlineColor = new ColorParameter(Color.black, true);
        public BoolParameter enableClosenessBoost = new BoolParameter(false, true);
        public FloatParameter closenessBoostThickness = new FloatParameter(1f, true);
        [Tooltip("You might need to set the value very small to see the result, for example 0.001")]
        public FloatParameter boostNear = new FloatParameter(0.001f, true);
        [Tooltip("You might need to set the value very small to see the result, for example 0.02")]
        public FloatParameter boostFar = new FloatParameter(0.02f, true);
        public BoolParameter enableDistantFade = new BoolParameter(true, true);
        public FloatParameter fadeNear = new FloatParameter(0.15f, true);
        public FloatParameter fadeFar = new FloatParameter(0.5f, true);
        //public BoolParameter depthCheckMoreSample = new BoolParameter(false, true);
        public OutlineDepthSampleParameter depthSampleType =
            new OutlineDepthSampleParameter(value: CleanOutlineDepthSample.FiveTiles, true);
        public ClampedFloatParameter nineTilesThreshold = new ClampedFloatParameter(0.28f, 0f, 1f);
        public ClampedFloatParameter nineTileBottomFix = new ClampedFloatParameter(0.003f, 0.0f, 0.1f);
        public FloatParameter depthThickness = new FloatParameter(1f, true);
        public FloatParameter outlineDepthMultiplier = new FloatParameter(1f, true);
        public FloatParameter outlineDepthBias = new FloatParameter(1f, true);
        public ClampedFloatParameter depthThreshold = new ClampedFloatParameter(0.1f, 0f, 1f);

        public BoolParameter enableNormalOutline = new BoolParameter(true, true);
        public BoolParameter normalCheckDirection = new BoolParameter(false, true);
        public FloatParameter normalThickness = new FloatParameter(1f, true);
        public FloatParameter outlineNormalMultiplier = new FloatParameter(1f, true);
        public FloatParameter outlineNormalBias = new FloatParameter(1f, true);
        public ClampedFloatParameter normalThreshold = new ClampedFloatParameter(0.2f, 0f, 1f, true);

        public CleanOutlineDebugModeParameter debugMode = new CleanOutlineDebugModeParameter(CleanOutlineDebugMode.Off, true);
        public FloatParameter testHeight = new FloatParameter(0, true);

        Material m_Material;

        public bool IsActive() => m_Material != null && intensity.value > 0f;

        // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > HDRP Default Settings).
        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterOpaqueAndSky;

        const string kShaderName = "Hidden/Shader/CleanOutline";

        private int INTENSITY_ID = Shader.PropertyToID("_Intensity");
        private int OUTLINETHICKNESS_ID = Shader.PropertyToID("_OutlineThickness");
        private int OUTLINECOLOR_ID = Shader.PropertyToID("_OutlineColor");
        private int ENABLECLOSENESSBOOST_ID = Shader.PropertyToID("_EnableClosenessBoost");
        private int CLOSENESSBOOSTTHICKNESS_ID = Shader.PropertyToID("_ClosenessBoostThickness");
        private int BOOSTNEAR_ID = Shader.PropertyToID("_BoostNear");
        private int BOOSTFAR_ID = Shader.PropertyToID("_BoostFar");
        private int ENABLEDISTANTFADE_ID = Shader.PropertyToID("_EnableDistantFade");
        private int FADENEAR_ID = Shader.PropertyToID("_FadeNear");
        private int FADEFAR_ID = Shader.PropertyToID("_FadeFar");
        private int DEPTHCHECKMORESAMPLE_ID = Shader.PropertyToID("_DepthCheckMoreSample");
        private int NINETILESTHRESHOLD_ID = Shader.PropertyToID("_NineTilesThreshold");
        private int NINETILEBOTTOMFIX_ID = Shader.PropertyToID("_NineTileBottomFix");
        private int DEPTHTHICKNESS_ID = Shader.PropertyToID("_DepthThickness");
        private int OUTLINEDEPTHMULTIPLIER_ID = Shader.PropertyToID("_OutlineDepthMultiplier");
        private int OUTLINEDEPTHBIAS_ID = Shader.PropertyToID("_OutlineDepthBias");
        private int DEPTHTHRESHOLD_ID = Shader.PropertyToID("_DepthThreshold");
        private int ENABLENORMALOUTLINE_ID = Shader.PropertyToID("_EnableNormalOutline");
        private int NORMALCHECKDIRECTION_ID = Shader.PropertyToID("_NormalCheckDirection");
        private int NORMALTHICKNESS_ID = Shader.PropertyToID("_NormalThickness");
        private int OUTLINENORMALMULTIPLIER_ID = Shader.PropertyToID("_OutlineNormalMultiplier");
        private int OUTLINENORMALBIAS_ID = Shader.PropertyToID("_OutlineNormalBias");
        private int NORMALTHRESHOLD_ID = Shader.PropertyToID("_NormalThreshold");

        private int DEBUGMODE_ID = Shader.PropertyToID("_DebugMode");

        //private int Outline_Depth_Bias_ID = Shader.PropertyToID("_OutlineDepthBias");

        public override void Setup()
        {
            if (Shader.Find(kShaderName) != null)
                m_Material = new Material(Shader.Find(kShaderName));
            else
                Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume CleanOutline is unable to load.");
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            if (m_Material == null)
                return;

            m_Material.SetFloat(INTENSITY_ID, intensity.value);
            m_Material.SetTexture("_InputTexture", source);

            m_Material.SetFloat(OUTLINETHICKNESS_ID, outlineThickness.value);
            m_Material.SetColor(OUTLINECOLOR_ID, outlineColor.value);
            m_Material.SetFloat(ENABLECLOSENESSBOOST_ID, enableClosenessBoost.value ? 1 : 0);
            m_Material.SetFloat(CLOSENESSBOOSTTHICKNESS_ID, closenessBoostThickness.value);
            m_Material.SetFloat(BOOSTNEAR_ID, boostNear.value);
            m_Material.SetFloat(BOOSTFAR_ID, boostFar.value);
            m_Material.SetFloat(ENABLEDISTANTFADE_ID, enableDistantFade.value ? 1 : 0);
            m_Material.SetFloat(FADENEAR_ID, fadeNear.value);
            m_Material.SetFloat(FADEFAR_ID, fadeFar.value);
            m_Material.SetFloat(DEPTHCHECKMORESAMPLE_ID, (float)depthSampleType.value);
            m_Material.SetFloat(NINETILESTHRESHOLD_ID, nineTilesThreshold.value);
            m_Material.SetFloat(NINETILEBOTTOMFIX_ID, nineTileBottomFix.value);
            m_Material.SetFloat(DEPTHTHICKNESS_ID, depthThickness.value);
            m_Material.SetFloat(OUTLINEDEPTHMULTIPLIER_ID, outlineDepthMultiplier.value);
            m_Material.SetFloat(OUTLINEDEPTHBIAS_ID, outlineDepthBias.value);
            m_Material.SetFloat(DEPTHTHRESHOLD_ID, depthThreshold.value);
            m_Material.SetFloat(ENABLENORMALOUTLINE_ID, enableNormalOutline.value ? 1f : 0f);
            m_Material.SetFloat(NORMALCHECKDIRECTION_ID, normalCheckDirection.value ? 1f : 0f);
            m_Material.SetFloat(NORMALTHICKNESS_ID, normalThickness.value);
            m_Material.SetFloat(OUTLINENORMALMULTIPLIER_ID, outlineNormalMultiplier.value);
            m_Material.SetFloat(OUTLINENORMALBIAS_ID, outlineNormalBias.value);
            m_Material.SetFloat(NORMALTHRESHOLD_ID, normalThreshold.value);
            m_Material.SetFloat(DEBUGMODE_ID, (float)debugMode.value);
            //m_Material.SetFloat("_TestHeight", testHeight.value);

            HDUtils.DrawFullScreen(cmd, m_Material, destination);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}
