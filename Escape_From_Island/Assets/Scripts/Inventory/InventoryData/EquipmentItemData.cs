using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySelection.InventorySystem
{
    /// <summary> ��� ������ </summary>
    public abstract class EquipmentItemData : ItemData
    {
        /// <summary> �ִ� ������ </summary>
        public int MaxDurability => _maxDurability;

        [SerializeField] private int _maxDurability = 100;
    }
}