// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: MagneticCylinderSection.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;

public class MagneticCylinderSection : MonoBehaviour
{
   #region Private fields

   [UsedImplicitly, SerializeField]
   private int m_NumSides;

   [UsedImplicitly, SerializeField]
   private float m_Skewness;

   private float m_SkewnessPrevFrame;
   private int m_NumSidesPrevFrame;

   #endregion

   #region Methods

   [UsedImplicitly]
   private void Start()
   {
      m_SkewnessPrevFrame = 0;
      m_NumSidesPrevFrame = 0;
   }

   [UsedImplicitly]
   private void Update()
   {
      if (m_NumSides != m_NumSidesPrevFrame || !Mathf.Approximately(m_Skewness, m_SkewnessPrevFrame))
      {
         RegenerateQuads();
         m_NumSidesPrevFrame = m_NumSides;
         m_SkewnessPrevFrame = m_Skewness;
      }
   }

   private void RegenerateQuads()
   {
      for (int i = transform.childCount - 1; i >= 0; --i)
      {
         Destroy(transform.GetChild(i).gameObject);
      }

      var cylinderRotation = transform.rotation;
      var cylinderScale = transform.localScale;

      transform.rotation = Quaternion.identity;
      transform.localScale = Vector3.one;

      float angleRadDelta = 2 * Mathf.PI / m_NumSides;

      for (int i = 0; i < m_NumSides; ++i)
      {
         var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
         quad.transform.parent = transform;
         quad.layer = 4;

         float angleRad = i * angleRadDelta;
         var direction = new Vector3(Mathf.Cos(angleRad), 0, m_Skewness * Mathf.Sin(angleRad));

         quad.transform.position = transform.position + direction * 0.5f;
         quad.transform.forward = new Vector3(m_Skewness * Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
         float scale = Mathf.Lerp(0.2f, 1,
            Mathf.Cos(angleRad) * 0.5f + 0.5f) - 0.5f * 2f;
         quad.transform.localScale = new Vector3(m_Skewness * angleRadDelta * Mathf.Sqrt(Mathf.Max(m_Skewness * Mathf.Sin(angleRadDelta), Mathf.Abs(Mathf.Cos(angleRad)))), 1, 1);
      }

      transform.rotation = cylinderRotation;
      transform.localScale = cylinderScale;
   }

   #endregion
}
