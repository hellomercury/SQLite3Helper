﻿using UnityEngine;
using UnityEngine.UI;

namespace Framework.Tools
{
	public class ShowRayastTargetRect : MonoBehaviour 
	{
#if UNITY_EDITOR
        private static Vector3[] fourCorners = new Vector3[4];
        void OnDrawGizmos()
        {
            foreach (MaskableGraphic g in FindObjectsOfType<MaskableGraphic>())
            {
                if (g.raycastTarget)
                {
                    RectTransform rectTransform = g.transform as RectTransform;
                    rectTransform.GetWorldCorners(fourCorners);
                    Gizmos.color = Color.red;
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
                    }
                }
            }
        }
#endif
    }
}