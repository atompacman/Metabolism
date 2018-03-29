// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: Utils.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using System.Collections;

using UnityEngine;

namespace Metabolism
{
   public static class Utils
   {
      #region Static methods

      public static bool IsUniformelyScaled(Transform a_Transform)
      {
         return Mathf.Approximately(a_Transform.localScale.x, a_Transform.localScale.y) &&
                Mathf.Approximately(a_Transform.localScale.x, a_Transform.localScale.z);
      }

      private static IEnumerator EditorDestroyAtEndOfFrame(Object a_Go)
      {
         yield return new WaitForEndOfFrame();
         Object.DestroyImmediate(a_Go);
      }

      public static void EditorCompatibleDestroy(this MonoBehaviour a_Component, Object a_Obj)
      {
         if (Application.isPlaying)
         {
            Object.Destroy(a_Obj);
         }
         else
         {
            a_Component.StartCoroutine(EditorDestroyAtEndOfFrame(a_Obj));
         }
      }

      public static void EditorCompatibleDestroyAllChildren(this MonoBehaviour a_Component)
      {
         foreach (Transform child in a_Component.transform)
         {
            a_Component.EditorCompatibleDestroy(child.gameObject);
         }
      }

      #endregion
   }
}
