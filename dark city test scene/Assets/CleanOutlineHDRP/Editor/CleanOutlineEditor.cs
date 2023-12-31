using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor;
using System;


namespace CR
{   

   
	[VolumeComponentEditor(typeof(CleanOutline))]
	public class CleanOutlineEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_Intensity;
        SerializedDataParameter m_OutlineThickness;
        SerializedDataParameter m_OutlineColor;
        SerializedDataParameter m_EnableClosenessBoost;
        SerializedDataParameter m_ClosenessBoostThickness;
        SerializedDataParameter m_BoostNear;
        SerializedDataParameter m_BoostFar;
        SerializedDataParameter m_EnableDistantFade;
        SerializedDataParameter m_FadeNear;
        SerializedDataParameter m_FadeFar;
        SerializedDataParameter m_DepthSampleType;
        SerializedDataParameter m_NineTilesThreshold;
        SerializedDataParameter m_NineTileBottomFix;
        SerializedDataParameter m_DepthThickness;
        SerializedDataParameter m_OutlineDepthMultiplier;
        SerializedDataParameter m_OutlineDepthBias;
        SerializedDataParameter m_DepthThreshold;
        SerializedDataParameter m_EnableNormalOutline;
        SerializedDataParameter m_NormalCheckDirection;
        SerializedDataParameter m_NormalThickness;
        SerializedDataParameter m_OutlineNormalMultiplier;
        SerializedDataParameter m_OutlineNormalBias;
        SerializedDataParameter m_NormalThreshold;

        SerializedDataParameter m_DebugMode;

        //public override bool hasAdvancedMode => false;
        public override void OnEnable()
        {

            base.OnEnable();

            var o = new PropertyFetcher<CleanOutline>(serializedObject);

            m_Intensity = Unpack(o.Find(x => x.intensity));

            m_OutlineThickness = Unpack(o.Find(x => x.outlineThickness));
            m_OutlineColor = Unpack(o.Find(x => x.outlineColor));
            m_EnableClosenessBoost = Unpack(o.Find(x => x.enableClosenessBoost));
            m_ClosenessBoostThickness = Unpack(o.Find(x => x.closenessBoostThickness));
            m_BoostNear = Unpack(o.Find(x => x.boostNear));
            m_BoostFar = Unpack(o.Find(x => x.boostFar));
            m_EnableDistantFade = Unpack(o.Find(x => x.enableDistantFade));
            m_FadeNear = Unpack(o.Find(x => x.fadeNear));
            m_FadeFar = Unpack(o.Find(x => x.fadeFar));
            m_DepthSampleType = Unpack(o.Find(x => x.depthSampleType));
            m_NineTilesThreshold = Unpack(o.Find(x => x.nineTilesThreshold));
            m_NineTileBottomFix = Unpack(o.Find(x => x.nineTileBottomFix));
            m_DepthThickness = Unpack(o.Find(x => x.depthThickness));
            m_OutlineDepthMultiplier = Unpack(o.Find(x => x.outlineDepthMultiplier));
            m_OutlineDepthBias = Unpack(o.Find(x => x.outlineDepthBias));
            m_DepthThreshold = Unpack(o.Find(x => x.depthThreshold));
            m_EnableNormalOutline = Unpack(o.Find(x => x.enableNormalOutline));
            m_NormalCheckDirection = Unpack(o.Find(x => x.normalCheckDirection));
            m_NormalThickness = Unpack(o.Find(x => x.normalThickness));
            m_OutlineNormalMultiplier = Unpack(o.Find(x => x.outlineNormalMultiplier));
            m_OutlineNormalBias = Unpack(o.Find(x => x.outlineNormalBias));
            m_NormalThreshold = Unpack(o.Find(x => x.normalThreshold));

            m_DebugMode = Unpack(o.Find(x => x.debugMode));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("General");
            PropertyField(m_Intensity);

            PropertyField(m_OutlineColor);
            PropertyField(m_OutlineThickness);
            
            EditorGUILayout.BeginVertical("box");
            PropertyField(m_EnableClosenessBoost);
            if (m_EnableClosenessBoost.value.boolValue)
            {
                EditorGUILayout.BeginVertical("box");
                PropertyField(m_ClosenessBoostThickness);
                PropertyField(m_BoostNear);
                PropertyField(m_BoostFar);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            PropertyField(m_EnableDistantFade);
            if (m_EnableDistantFade.value.boolValue)
            {
                EditorGUILayout.BeginVertical("box");
                PropertyField(m_FadeNear);
                PropertyField(m_FadeFar);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Depth");
            PropertyField(m_DepthSampleType);
            if (m_DepthSampleType.value.intValue == 0)
            {
                
            }
            else if (m_DepthSampleType.value.intValue == 1)
            {
                EditorGUILayout.BeginVertical("box");
                PropertyField(m_NineTilesThreshold);
                PropertyField(m_NineTileBottomFix);
                EditorGUILayout.EndVertical();
            }
            PropertyField(m_DepthThickness);
            PropertyField(m_OutlineDepthMultiplier);
            PropertyField(m_OutlineDepthBias);
            PropertyField(m_DepthThreshold);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Normal");
            PropertyField(m_EnableNormalOutline);
            if (m_EnableNormalOutline.value.boolValue)
            {
                EditorGUILayout.BeginVertical("box");
                PropertyField(m_NormalCheckDirection);
                PropertyField(m_NormalThickness);
                PropertyField(m_OutlineNormalMultiplier);
                PropertyField(m_OutlineNormalBias);
                PropertyField(m_NormalThreshold);    
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            PropertyField(m_DebugMode);
        }
    }
}
