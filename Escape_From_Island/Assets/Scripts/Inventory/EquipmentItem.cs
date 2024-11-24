using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InventorySelection.InventorySystem
{
    /// <summary> ��� ������</summary>
    public abstract class EquipmentItem : Item
    {
        public EquipmentItemData EquipmentData { get; private set; }

        /// <summary> ���� ������ </summary>
        public int Durability
        {
            get => _durability;
            set
            {
                if (value < 0) value = 0;
                if (value > EquipmentData.MaxDurability)
                    value = EquipmentData.MaxDurability;

                _durability = value;
            }
        }
        private int _durability;

        public EquipmentItem(EquipmentItemData data) : base(data)
        {
            EquipmentData = data;
            Durability = data.MaxDurability;
        }

        // Item Data ���� �ʵ尪�� ���� �Ű������� ���� �����ڴ� �߰��� �������� ����
        // �ڽĵ鿡�� ��� �߰������ �ϹǷ� ���������鿡�� ����
    }
}