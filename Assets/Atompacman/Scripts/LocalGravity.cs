﻿// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: LocalGravity.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;

using UnityStandardAssets.Characters.FirstPerson;

namespace Metabolism
{
   public sealed class LocalGravity : MonoBehaviour
   {
      #region Compile-time constants

      private const int GRAVITATION_COLLIDERS_LAYER_MASK = 16;

      #endregion

      #region Private fields

      // Rigidbody component on which we need to apply local gravity
      [UsedImplicitly, SerializeField]
      private Rigidbody m_RigidbodyToControl;

      // Size of the spherical zone around this object where we look for gravitation colliders to
      // evaluate local gravity
      [UsedImplicitly, SerializeField]
      private float m_GravitationCollidersSphereRadius;

      // Last frame's gravitation colliders
      private Collider[] m_GravitationColliders;

      #endregion

      #region Static methods

      private static Vector3 EvaluateLocalGravityForce(ICollection<Collider> a_GravitationColliders)
      {
         var localGravity = Vector3.zero;

         foreach (var gravCollider in a_GravitationColliders)
         {
            var colliderNormal = gravCollider.transform.forward;
            localGravity += colliderNormal;

            Debug.DrawRay(gravCollider.transform.position, colliderNormal * -3, Color.black);
         }

         return localGravity.normalized * Physics.gravity.magnitude;
      }

      #endregion

      #region Methods

      [UsedImplicitly]
      private void Update()
      {
         // Update debug collider spherical zone visibility
         GetComponent<Renderer>().enabled = GlobalState.Instance.EnableLocalGravity;

         // Reset collider coloring
         if (m_GravitationColliders != null)
         {
            SetGravitationCollidersColor(Color.white);
         }

         // If not activated, standard gravity is used
         if (!GlobalState.Instance.EnableLocalGravity)
         {
            SetLocalGravity(Physics.gravity);
            return;
         }

         // Adjust the size of the collider zone
         transform.localScale = 2 * m_GravitationCollidersSphereRadius * Vector3.one;

         // Update the list of colliders inside the spherical zone
         m_GravitationColliders = Physics.OverlapSphere(transform.position,
            m_GravitationCollidersSphereRadius,
            GRAVITATION_COLLIDERS_LAYER_MASK);

         // No need to change gravity if no collider was found
         if (m_GravitationColliders.Length == 0)
         {
            return;
         }

         // Change collider colors for debug
         SetGravitationCollidersColor(Color.green);

         // Evaluate and set local gravity using quad colliders average normal vector
         SetLocalGravity(EvaluateLocalGravityForce(m_GravitationColliders));
      }

      private void SetGravitationCollidersColor(Color a_Color)
      {
         foreach (var gravCollider in m_GravitationColliders)
         {
            gravCollider.gameObject.GetComponent<Renderer>().material.color = a_Color;
         }
      }

      private void SetLocalGravity(Vector3 a_LocalGravity)
      {
         // Manual add a force on the rigidbody to simulate gravity
         m_RigidbodyToControl.AddForce(a_LocalGravity, ForceMode.Acceleration);
         
         // RBFPC is a standard asset that needs to know the value to position camera according to
         // gravity
         var rbfpc = m_RigidbodyToControl.gameObject.GetComponent<RigidbodyFirstPersonController>();
         if (rbfpc != null)
         {
            rbfpc.LocalGravity = a_LocalGravity;
         }
      }

      #endregion
   }
}
