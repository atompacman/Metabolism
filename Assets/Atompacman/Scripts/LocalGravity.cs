// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: LocalGravity.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;

using UnityStandardAssets.Characters.FirstPerson;

public class LocalGravity : MonoBehaviour
{
   #region Private fields

   [UsedImplicitly, SerializeField]
   private float m_EvaluationSphereRadius;

   [UsedImplicitly, SerializeField]
   private int m_GravitationCollidersLayerMask;

   [UsedImplicitly, SerializeField]
   private bool m_EnableLocalGravity;

   private Collider[] m_GravitationColliders;

   #endregion

   #region Methods

   [UsedImplicitly]
   private void Update()
   {
      if (Input.GetKeyUp(KeyCode.G))
      {
         m_EnableLocalGravity = !m_EnableLocalGravity;
      }

      transform.localScale = 2 * m_EvaluationSphereRadius * Vector3.one;

      GetComponent<Renderer>().enabled = m_EnableLocalGravity;

      if (!m_EnableLocalGravity)
      {
         SetGravitationCollidersColor(Color.white);
         Physics.gravity = Vector3.down * 9.8f;
         GetComponentInParent<RigidbodyFirstPersonController>().LocalGravity = Physics.gravity;
         return;
      }

      UpdateGravitationColliders();
      UpdateLocalGravityForce();
   }

   private void SetGravitationCollidersColor(Color a_Color)
   {
      if (m_GravitationColliders == null)
      {
         return;
      }

      foreach (var gravCollider in m_GravitationColliders)
      {
         gravCollider.gameObject.GetComponent<Renderer>().material.color = a_Color;
      }
   }

   private void UpdateGravitationColliders()
   {
      SetGravitationCollidersColor(Color.white);
      
      m_GravitationColliders = Physics.OverlapSphere(transform.position, m_EvaluationSphereRadius,
         m_GravitationCollidersLayerMask);

      SetGravitationCollidersColor(Color.green);
   }

   private void UpdateLocalGravityForce()
   {
      if (m_GravitationColliders.Length == 0)
      {
         return;
      }

      var localGravity = Vector3.zero;
      foreach (var gravCollider in m_GravitationColliders)
      {
         localGravity += gravCollider.transform.forward;
         Debug.DrawRay(gravCollider.transform.position, gravCollider.transform.forward * -3, Color.black);
      }
      localGravity.Normalize();
      localGravity *= Physics.gravity.magnitude;

      GetComponentInParent<RigidbodyFirstPersonController>().LocalGravity = localGravity;
      // TODO Ideally we wouldn't set the global gravity
      Physics.gravity = localGravity;
   }

   #endregion
}
