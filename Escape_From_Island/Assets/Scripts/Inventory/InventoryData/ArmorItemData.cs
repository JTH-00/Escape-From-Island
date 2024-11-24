using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySelection.InventorySystem
{
    /// <summary> ��� - �� ������ </summary>
    [CreateAssetMenu(fileName = "Item_Armor_", menuName = "Inventory System/Item Data/Armor", order = 2)]
    public class ArmorItemData : EquipmentItemData
    {
        /// <summary> ���� </summary>
        public int Defence => _defence;

        [SerializeField] private int _defence = 1;
        public override Item CreateItem()
        {
            return new ArmorItem(this);
        }
    }
}
