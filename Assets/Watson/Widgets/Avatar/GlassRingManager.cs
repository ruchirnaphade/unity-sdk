﻿using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// GlassRingManager manages the Avatar's 3D models' ring material color according to mood and behavior change.
    /// </summary>
	public class GlassRingManager : AvatarModelManager
    {

        #region Private Variables
		[SerializeField]
		float m_AnimationTime = 1.0f;

        Material m_GlassRingMaterial;
        Color m_InitialColorOfGlassRingMaterial;

		LTDescr m_ColorAnimationOnGlass = null;
		LTDescr m_ColorAnimationOnGlassLoop = null;
		Color m_LastColorUsedInAnimation = Color.white;
        #endregion

        #region OnEnable / OnDisable / OnApplicationQuit / Awake

        void OnApplicationQuit()
        {
            if (m_GlassRingMaterial != null)
            {
                m_GlassRingMaterial.SetColor("_SpecColor", m_InitialColorOfGlassRingMaterial);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_AvatarWidgetAttached = this.transform.GetComponentInParent<AvatarWidget>();
            if (m_AvatarWidgetAttached != null)
            {
                MeshRenderer childMeshRenderer = transform.GetComponentInChildren<MeshRenderer>();
                if (childMeshRenderer != null)
                {
                    m_GlassRingMaterial = childMeshRenderer.sharedMaterial;
                    m_InitialColorOfGlassRingMaterial = m_GlassRingMaterial.GetColor("_SpecColor");
                }
                else
                {
                    Log.Error("GlassRingManager", "There is not mesh renderer in any child object");
                    this.enabled = false;
                }
            }
            else
            {
                Log.Error("GlassRingManager", "There is no Avatar Widget on any parent.");
                this.enabled = false;
            }

        }

        #endregion

        #region Changing Mood / Avatar State

        public override void ChangedBehavior(Color color, float timeModifier)
        {
            if (m_GlassRingMaterial != null)
            {
				if(m_ColorAnimationOnGlass != null){
					LeanTween.cancel(m_ColorAnimationOnGlass.uniqueId);
				}

                if (m_ColorAnimationOnGlassLoop != null)
                {
                    LeanTween.cancel(m_ColorAnimationOnGlassLoop.uniqueId);
                }

				m_ColorAnimationOnGlass = LeanTween.value(gameObject, m_LastColorUsedInAnimation, color, m_AnimationTime).setOnUpdateColor(
					(Color colorToFadeIn)=>{
						m_LastColorUsedInAnimation = colorToFadeIn;
						m_GlassRingMaterial.SetColor("_SpecColor", colorToFadeIn);
					}).setOnComplete(
					()=>{

					m_ColorAnimationOnGlassLoop = LeanTween.value(gameObject, color, Color.white, m_AnimationTime * timeModifier).setLoopPingPong().setOnUpdateColor(
						(Color colorToLoop) =>
						{
							m_GlassRingMaterial.SetColor("_SpecColor", colorToLoop);
							m_LastColorUsedInAnimation = colorToLoop;
						});	

					});

            }
        }

        #endregion
    }

}