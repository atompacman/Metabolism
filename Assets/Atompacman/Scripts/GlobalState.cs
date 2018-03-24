// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: DebugShortcuts.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;

namespace Metabolism
{
   public sealed class GlobalState : MonoBehaviour
   {
      #region Private fields

      [UsedImplicitly, SerializeField]
      public bool EnableLocalGravity;

      #endregion

      public static GlobalState Instance;

      #region Methods

      [UsedImplicitly]
      private void Awake()
      {
         Debug.Assert(Instance == null);
         Instance = this;
      }

      [UsedImplicitly]
      private void Update()
      {
         if (Input.GetKeyUp(KeyCode.G))
         {
            EnableLocalGravity = !EnableLocalGravity;
         }
      }

      #endregion
   }
}
