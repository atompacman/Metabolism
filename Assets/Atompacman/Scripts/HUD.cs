// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
// MIT License
// Copyright (c) 2018 Stained Glass Guild
// See file "LICENSE.txt" at project root for complete license
// ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~   ~
// Project: Metabolism
// File: HUD.cs
// Creation: 2018-03
// Author: Jérémie Coulombe
// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once InconsistentNaming
public class HUD : MonoBehaviour
{
   [UsedImplicitly, SerializeField]
   private GameObject m_Player;

   #region Methods

   [UsedImplicitly]
   private void Update()
   {
      var velocityField = transform.Find("Velocity Field").GetComponent<Text>();
      velocityField.text = string.Format("{0:0.00}", m_Player.GetComponent<Rigidbody>().velocity.magnitude);
   }

   #endregion
}
