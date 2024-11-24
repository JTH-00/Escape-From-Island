using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
    [��� - ������ ����]
    - ���� ���� �� �������� ������ ���� �̸�����(����, ũ�� �̸����� ����)
    [��� - ���� �������̽�]
    - ���Կ� ���콺 �ø���
      - ��� ���� ���� : ���̶���Ʈ �̹��� ǥ��
      - ������ ���� ���� : ������ ���� ���� ǥ��
    - �巡�� �� ���
      - ������ ���� ���� -> ������ ���� ���� : �� ������ ��ġ ��ȯ
      - ������ ���� ���� -> ������ ������ ���� : ������ ��ġ ����
        - Shift �Ǵ� Ctrl ���� ������ ��� : �� �� �ִ� ������ ���� ������
      - ������ ���� ���� -> UI �ٱ� : ������ ������
    - ���� ��Ŭ��
      - ��� ������ �������� ��� : ������ ���
    - ��� ��ư(���� ���)
      - Trim : �տ������� �� ĭ ���� ������ ä���
      - Sort : ������ ����ġ��� ������ ����
    - ���� ��ư(���� ���)
      - [A] : ��� ������ ���͸�
      - [E] : ��� ������ ���͸�
      - [P] : �Һ� ������ ���͸�
      * ���͸����� ���ܵ� ������ ���Ե��� ���� �Ұ�
    [��� - ��Ÿ]
    - InvertMouse(bool) : ���콺 ��Ŭ��/��Ŭ�� ���� ���� ����
*/
namespace InventorySelection.InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {

        /***********************************************************************
        *                               Option Fields
        ***********************************************************************/
        #region .
        [Header("Options")]
        [Range(0, 10)]
        [SerializeField] private int _horizontalSlotCount = 5;  // ���� ���� ����
        [Range(0, 10)]
        [SerializeField] private int _verticalSlotCount = 6;      // ���� ���� ����
        [SerializeField] private float _slotMargin = 5f;          // �� ������ �����¿� ����
        [SerializeField] private float _contentAreaPadding = 7f; // �κ��丮 ������ ���� ����
        [Range(32, 64)]
        [SerializeField] private float _slotSize = 44f;      // �� ������ ũ��

        [Space]
        [SerializeField] private bool _showTooltip = true;
        [SerializeField] private bool _showHighlight = true;
        [SerializeField] private bool _showRemovingPopup = true;

        [Header("Connected Objects")]
        [SerializeField] private RectTransform _contentAreaRT; // ���Ե��� ��ġ�� ����
        [SerializeField] private GameObject _slotUiPrefab;     // ������ ���� ������
        [SerializeField] private ItemTooltipUI _itemTooltip;   // ������ ������ ������ ���� UI
        [SerializeField] private InventoryPopupUI _popup;      // �˾� UI ���� ��ü

        [Header("Buttons")]
        [SerializeField] private Button _trimButton;
        [SerializeField] private Button _sortButton;

        [Header("Filter Toggles")]
        [SerializeField] private Toggle _toggleFilterAll;
        [SerializeField] private Toggle _toggleFilterEquipments;
        [SerializeField] private Toggle _toggleFilterPortions;


        [Space(16)]
        [SerializeField] private bool _mouseReversed = false; // ���콺 Ŭ�� ���� ����

        #endregion
        /***********************************************************************
        *                               Private Fields
        ***********************************************************************/
        #region .

        /// <summary> ����� �κ��丮 </summary>
        private Inventory _inventory;
        private List<ItemSlotUI> _slotUIList = new List<ItemSlotUI>();
        private GraphicRaycaster _gr;
        private PointerEventData _ped;
        private List<RaycastResult> _rrList;

        private ItemSlotUI _pointerOverSlot; // ���� �����Ͱ� ��ġ�� ���� ����
        private ItemSlotUI _beginDragSlot; // ���� �巡�׸� ������ ����
        private Transform _beginDragIconTransform; // �ش� ������ ������ Ʈ������

        private int _leftClick = 0;
        private int _rightClick = 1;

        private Vector3 _beginDragIconPoint;   // �巡�� ���� �� ������ ��ġ
        private Vector3 _beginDragCursorPoint; // �巡�� ���� �� Ŀ���� ��ġ
        private int _beginDragSlotSiblingIndex;

        /// <summary> �κ��丮 UI �� ������ ���͸� �ɼ� </summary>
        private enum FilterOption
        {
            All, Equipment, Portion
        }
        private FilterOption _currentFilterOption = FilterOption.All;

        #endregion
        /***********************************************************************
        *                               Unity Events
        ***********************************************************************/
        #region .
        private void Awake()
        {
            Init();
            InitSlots();
            InitButtonEvents();
            InitToggleEvents();
            
        }
        
        
       
        private void Update()
        {
            _ped.position = Input.mousePosition;

            OnPointerEnterAndExit();
            if (_showTooltip) ShowOrHideItemTooltip();
            OnPointerDown();
            OnPointerDrag();
            OnPointerUp();
            
        }

        #endregion
        /***********************************************************************
        *                               Init Methods
        ***********************************************************************/
        #region .
        private void Init()
        {
            TryGetComponent(out _gr);
            if (_gr == null)
                _gr = gameObject.AddComponent<GraphicRaycaster>();

            // Graphic Raycaster
            _ped = new PointerEventData(EventSystem.current);
            _rrList = new List<RaycastResult>(10);

            // Item Tooltip UI
            if (_itemTooltip == null)
            {
                _itemTooltip = GetComponentInChildren<ItemTooltipUI>();
                EditorLog("�ν����Ϳ��� ������ ���� UI�� ���� �������� �ʾ� �ڽĿ��� �߰��Ͽ� �ʱ�ȭ�Ͽ����ϴ�.");
            }
        }

        /// <summary> ������ ������ŭ ���� ���� ���� ���Ե� ���� ���� </summary>
        private void InitSlots()
        {
            // ���� ������ ����
            _slotUiPrefab.TryGetComponent(out RectTransform slotRect);
            slotRect.sizeDelta = new Vector2(_slotSize, _slotSize);

            _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
            if (itemSlot == null)
                _slotUiPrefab.AddComponent<ItemSlotUI>();

            _slotUiPrefab.SetActive(false);

            // --
            Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
            Vector2 curPos = beginPos;

            _slotUIList = new List<ItemSlotUI>(_verticalSlotCount * _horizontalSlotCount);

            // ���Ե� ���� ����
            for (int j = 0; j < _verticalSlotCount; j++)
            {
                for (int i = 0; i < _horizontalSlotCount; i++)
                {
                    int slotIndex = (_horizontalSlotCount * j) + i;

                    var slotRT = CloneSlot();
                    slotRT.pivot = new Vector2(0f, 1f); // Left Top
                    slotRT.anchoredPosition = curPos;
                    slotRT.gameObject.SetActive(true);
                    slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                    var slotUI = slotRT.GetComponent<ItemSlotUI>();
                    slotUI.SetSlotIndex(slotIndex);
                    _slotUIList.Add(slotUI);

                    // Next X
                    curPos.x += (_slotMargin + _slotSize);
                }

                // Next Line
                curPos.x = beginPos.x;
                curPos.y -= (_slotMargin + _slotSize);
            }

            // ���� ������ - �������� �ƴ� ��� �ı�
            if (_slotUiPrefab.scene.rootCount != 0)
                Destroy(_slotUiPrefab);

            // -- Local Method --
            RectTransform CloneSlot()
            {
                GameObject slotGo = Instantiate(_slotUiPrefab);
                RectTransform rt = slotGo.GetComponent<RectTransform>();
                rt.SetParent(_contentAreaRT);

                return rt;
            }
        }
        private void InitButtonEvents()
        {
            _trimButton.onClick.AddListener(() => _inventory.TrimAll());
            _sortButton.onClick.AddListener(() => _inventory.SortAll());
        }

        private void InitToggleEvents()
        {
            _toggleFilterAll.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.All));
            _toggleFilterEquipments.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Equipment));
            _toggleFilterPortions.onValueChanged.AddListener(flag => UpdateFilter(flag, FilterOption.Portion));

            // Local Method
            void UpdateFilter(bool flag, FilterOption option)
            {
                if (flag)
                {
                    _currentFilterOption = option;
                    UpdateAllSlotFilters();
                }
            }
        }

        #endregion
        /***********************************************************************
        *                               Mouse Event Methods
        ***********************************************************************/
        #region .
        private bool IsOverUI()
            => EventSystem.current.IsPointerOverGameObject();

        /// <summary> ����ĳ��Ʈ�Ͽ� ���� ù ��° UI���� ������Ʈ ã�� ���� </summary>
        private T RaycastAndGetFirstComponent<T>() where T : Component
        {
            _rrList.Clear();

            _gr.Raycast(_ped, _rrList);

            if (_rrList.Count == 0)
                return null;

            return _rrList[0].gameObject.GetComponent<T>();
        }
        /// <summary> ���Կ� �����Ͱ� �ö󰡴� ���, ���Կ��� �����Ͱ� ���������� ��� </summary>
        private void OnPointerEnterAndExit()
        {
            // ���� �������� ����
            var prevSlot = _pointerOverSlot;

            // ���� �������� ����
            var curSlot = _pointerOverSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (prevSlot == null)
            {
                // Enter
                if (curSlot != null)
                {
                    OnCurrentEnter();
                }
            }
            else
            {
                // Exit
                if (curSlot == null)
                {
                    OnPrevExit();
                }

                // Change
                else if (prevSlot != curSlot)
                {
                    OnPrevExit();
                    OnCurrentEnter();
                }
            }

            // ===================== Local Methods ===============================
            void OnCurrentEnter()
            {
                if (_showHighlight)
                    curSlot.Highlight(true);
            }
            void OnPrevExit()
            {
                prevSlot.Highlight(false);
            }
        }
        /// <summary> ������ ���� ���� �����ְų� ���߱� </summary>
        private void ShowOrHideItemTooltip()
        {
            // ���콺�� ��ȿ�� ������ ������ ���� �ö�� �ִٸ� ���� �����ֱ�
            bool isValid =
                _pointerOverSlot != null && _pointerOverSlot.HasItem && _pointerOverSlot.IsAccessible
                && (_pointerOverSlot != _beginDragSlot); // �巡�� ������ �����̸� �������� �ʱ�

            if (isValid)
            {
                UpdateTooltipUI(_pointerOverSlot);
                _itemTooltip.Show();
            }
            else
                _itemTooltip.Hide();
        }
        /// <summary> ���Կ� Ŭ���ϴ� ��� </summary>
        private void OnPointerDown()
        {
            // Left Click : Begin Drag
            if (Input.GetMouseButtonDown(_leftClick))
            {
                _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

                // �������� ���� �ִ� ���Ը� �ش�
                if (_beginDragSlot != null && _beginDragSlot.HasItem && _beginDragSlot.IsAccessible)
                {
                    EditorLog($"Drag Begin : Slot [{_beginDragSlot.Index}]");

                    // ��ġ ���, ���� ���
                    _beginDragIconTransform = _beginDragSlot.IconRect.transform;
                    _beginDragIconPoint = _beginDragIconTransform.position;
                    _beginDragCursorPoint = Input.mousePosition;

                    // �� ���� ���̱�
                    _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                    _beginDragSlot.transform.SetAsLastSibling();

                    // �ش� ������ ���̶���Ʈ �̹����� �����ܺ��� �ڿ� ��ġ��Ű��
                    _beginDragSlot.SetHighlightOnTop(false);
                }
                else
                {
                    _beginDragSlot = null;
                }
            }

            // Right Click : Use Item
            else if (Input.GetMouseButtonDown(_rightClick))
            {
                ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();

                if (slot != null && slot.HasItem && slot.IsAccessible)
                {
                    TryUseItem(slot.Index);
                }
            }
        }
        /// <summary> �巡���ϴ� ���� </summary>
        private void OnPointerDrag()
        {
            if (_beginDragSlot == null) return;

            if (Input.GetMouseButton(_leftClick))
            {
                // ��ġ �̵�
                _beginDragIconTransform.position =
                    _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
            }
        }
        /// <summary> Ŭ���� �� ��� </summary>
        private void OnPointerUp()
        {
            if (Input.GetMouseButtonUp(_leftClick))
            {
                // End Drag
                if (_beginDragSlot != null)
                {
                    // ��ġ ����
                    _beginDragIconTransform.position = _beginDragIconPoint;

                    // UI ���� ����
                    _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                    // �巡�� �Ϸ� ó��
                    EndDrag();

                    // �ش� ������ ���̶���Ʈ �̹����� �����ܺ��� �տ� ��ġ��Ű��
                    _beginDragSlot.SetHighlightOnTop(true);

                    // ���� ����
                    _beginDragSlot = null;
                    _beginDragIconTransform = null;
                }
            }
        }

        private void EndDrag()
        {
            ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            // ������ ���Գ��� ������ ��ȯ �Ǵ� �̵�
            if (endDragSlot != null && endDragSlot.IsAccessible)
            {
                // ���� ������ ����
                // 1) ���콺 Ŭ�� ���� ���� ���� Ctrl �Ǵ� Shift Ű ����
                // 2) begin : �� �� �ִ� ������ / end : ����ִ� ����
                // 3) begin �������� ���� > 1
                bool isSeparatable =
                    (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                    (_inventory.IsCountableItem(_beginDragSlot.Index) && !_inventory.HasItem(endDragSlot.Index));

                // true : ���� ������, false : ��ȯ �Ǵ� �̵�
                bool isSeparation = false;
                int currentAmount = 0;

                // ���� ���� Ȯ��
                if (isSeparatable)
                {
                    currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.Index);
                    if (currentAmount > 1)
                    {
                        isSeparation = true;
                    }
                }

                // 1. ���� ������
                if (isSeparation)
                    TrySeparateAmount(_beginDragSlot.Index, endDragSlot.Index, currentAmount);
                // 2. ��ȯ �Ǵ� �̵�
                else
                    TrySwapItems(_beginDragSlot, endDragSlot);

                // ���� ����
                UpdateTooltipUI(endDragSlot);
                return;
            }

            // ������(Ŀ���� UI ����ĳ��Ʈ Ÿ�� ���� ���� ���� ���)
            if (!IsOverUI())
            {
                // Ȯ�� �˾� ���� �ݹ� ����
                int index = _beginDragSlot.Index;
                string itemName = _inventory.GetItemName(index);
                int amount = _inventory.GetCurrentAmount(index);

                // �� �� �ִ� �������� ���, ���� ǥ��
                if (amount > 1)
                    itemName += $" x{amount}";

                if (_showRemovingPopup)
                    _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
                else
                    TryRemoveItem(index);
            }
            // ������ �ƴ� �ٸ� UI ���� ���� ���
            else
            {
                EditorLog($"Drag End(Do Nothing)");
            }
        }

        #endregion
        /***********************************************************************
        *                               Private Methods
        ***********************************************************************/
        #region .

        /// <summary> UI �� �κ��丮���� ������ ���� </summary>
        private void TryRemoveItem(int index)
        {
            EditorLog($"UI - Try Remove Item : Slot [{index}]");

            _inventory.Remove(index);
        }

        /// <summary> ������ ��� </summary>
        private void TryUseItem(int index)
        {
            EditorLog($"UI - Try Use Item : Slot [{index}]");

            _inventory.Use(index);
        }

        /// <summary> �� ������ ������ ��ȯ </summary>
        private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
        {
            if (from == to)
            {
                EditorLog($"UI - Try Swap Items: Same Slot [{from.Index}]");
                return;
            }

            EditorLog($"UI - Try Swap Items: Slot [{from.Index} -> {to.Index}]");

            from.SwapOrMoveIcon(to);
            _inventory.Swap(from.Index, to.Index);
        }

        /// <summary> �� �� �ִ� ������ ���� ������ </summary>
        private void TrySeparateAmount(int indexA, int indexB, int amount)
        {
            if (indexA == indexB)
            {
                EditorLog($"UI - Try Separate Amount: Same Slot [{indexA}]");
                return;
            }

            EditorLog($"UI - Try Separate Amount: Slot [{indexA} -> {indexB}]");

            string itemName = $"{_inventory.GetItemName(indexA)} x{amount}";

            _popup.OpenAmountInputPopup(
                amt => _inventory.SeparateAmount(indexA, indexB, amt),
                amount, itemName
            );
        }

        /// <summary> ���� UI�� ���� ������ ���� </summary>
        private void UpdateTooltipUI(ItemSlotUI slot)
        {
            if (!slot.IsAccessible || !slot.HasItem)
                return;

            // ���� ���� ����
            _itemTooltip.SetItemInfo(_inventory.GetItemData(slot.Index));

            // ���� ��ġ ����
            _itemTooltip.SetRectPosition(slot.SlotRect);
        }

        #endregion
        /***********************************************************************
        *                               Public Methods
        ***********************************************************************/
        #region .

        /// <summary> �κ��丮 ���� ��� (�κ��丮���� ���� ȣ��) </summary>
        public void SetInventoryReference(Inventory inventory)
        {
            _inventory = inventory;
        }

        /// <summary> ���콺 Ŭ�� �¿� ������Ű�� (true : ����) </summary>
        public void InvertMouse(bool value)
        {
            _leftClick = value ? 1 : 0;
            _rightClick = value ? 0 : 1;

            _mouseReversed = value;
        }

        /// <summary> ���Կ� ������ ������ ��� </summary>
        public void SetItemIcon(int index, Sprite icon)
        {
            EditorLog($"Set Item Icon : Slot [{index}]");

            _slotUIList[index].SetItem(icon);
        }

        /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
        public void SetItemAmountText(int index, int amount)
        {
            EditorLog($"Set Item Amount Text : Slot [{index}], Amount [{amount}]");

            // NOTE : amount�� 1 ������ ��� �ؽ�Ʈ ��ǥ��
            _slotUIList[index].SetItemAmount(amount);
        }

        /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
        public void HideItemAmountText(int index)
        {
            EditorLog($"Hide Item Amount Text : Slot [{index}]");

            _slotUIList[index].SetItemAmount(1);
        }

        /// <summary> ���Կ��� ������ ������ ����, ���� �ؽ�Ʈ ����� </summary>
        public void RemoveItem(int index)
        {
            EditorLog($"Remove Item : Slot [{index}]");

            _slotUIList[index].RemoveItem();
        }

        /// <summary> ���� ������ ���� ���� ���� </summary>
        public void SetAccessibleSlotRange(int accessibleSlotCount)
        {
            for (int i = 0; i < _slotUIList.Count; i++)
            {
                _slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
            }
        }

        /// <summary> Ư�� ������ ���� ���� ������Ʈ </summary>
        public void UpdateSlotFilterState(int index, ItemData itemData)
        {
            bool isFiltered = true;

            // null�� ������ Ÿ�� �˻� ���� ���� Ȱ��ȭ
            if (itemData != null)
                switch (_currentFilterOption)
                {
                    case FilterOption.Equipment:
                        isFiltered = (itemData is EquipmentItemData);
                        break;

                    case FilterOption.Portion:
                        isFiltered = (itemData is PortionItemData);
                        break;
                }

            _slotUIList[index].SetItemAccessibleState(isFiltered);
        }

        /// <summary> ��� ���� ���� ���� ������Ʈ </summary>
        public void UpdateAllSlotFilters()
        {
            int capacity = _inventory.Capacity;

            for (int i = 0; i < capacity; i++)
            {
                ItemData data = _inventory.GetItemData(i);
                UpdateSlotFilterState(i, data);
            }
        }

        #endregion
        /***********************************************************************
        *                               Editor Only Debug
        ***********************************************************************/
        #region .

        [Header("Editor Options")]
        [SerializeField] private bool _showDebug = true;
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void EditorLog(object message)
        {
            if (!_showDebug) return;
            UnityEngine.Debug.Log($"[InventoryUI] {message}");
        }

        #endregion
        /***********************************************************************
        *                               Editor Preview
        ***********************************************************************/
        #region .
#if UNITY_EDITOR
        [SerializeField] private bool __showPreview = false;

        [Range(0.01f, 1f)]
        [SerializeField] private float __previewAlpha = 0.1f;

        private List<GameObject> __previewSlotGoList = new List<GameObject>();
        private int __prevSlotCountPerLine;
        private int __prevSlotLineCount;
        private float __prevSlotSize;
        private float __prevSlotMargin;
        private float __prevContentPadding;
        private float __prevAlpha;
        private bool __prevShow = false;
        private bool __prevMouseReversed = false;

        private void OnValidate()
        {
            if (__prevMouseReversed != _mouseReversed)
            {
                __prevMouseReversed = _mouseReversed;
                InvertMouse(_mouseReversed);

                EditorLog($"Mouse Reversed : {_mouseReversed}");
            }

            if (Application.isPlaying) return;

            if (__showPreview && !__prevShow)
            {
                CreateSlots();
            }
            __prevShow = __showPreview;

            if (Unavailable())
            {
                ClearAll();
                return;
            }
            if (CountChanged())
            {
                ClearAll();
                CreateSlots();
                __prevSlotCountPerLine = _horizontalSlotCount;
                __prevSlotLineCount = _verticalSlotCount;
            }
            if (ValueChanged())
            {
                DrawGrid();
                __prevSlotSize = _slotSize;
                __prevSlotMargin = _slotMargin;
                __prevContentPadding = _contentAreaPadding;
            }
            if (AlphaChanged())
            {
                SetImageAlpha();
                __prevAlpha = __previewAlpha;
            }

            bool Unavailable()
            {
                return !__showPreview ||
                        _horizontalSlotCount < 1 ||
                        _verticalSlotCount < 1 ||
                        _slotSize <= 0f ||
                        _contentAreaRT == null ||
                        _slotUiPrefab == null;
            }
            bool CountChanged()
            {
                return _horizontalSlotCount != __prevSlotCountPerLine ||
                       _verticalSlotCount != __prevSlotLineCount;
            }
            bool ValueChanged()
            {
                return _slotSize != __prevSlotSize ||
                       _slotMargin != __prevSlotMargin ||
                       _contentAreaPadding != __prevContentPadding;
            }
            bool AlphaChanged()
            {
                return __previewAlpha != __prevAlpha;
            }
            void ClearAll()
            {
                foreach (var go in __previewSlotGoList)
                {
                    Destroyer.Destroy(go);
                }
                __previewSlotGoList.Clear();
            }
            void CreateSlots()
            {
                int count = _horizontalSlotCount * _verticalSlotCount;
                __previewSlotGoList.Capacity = count;

                // ������ �ǹ��� Left Top���� ����
                RectTransform slotPrefabRT = _slotUiPrefab.GetComponent<RectTransform>();
                slotPrefabRT.pivot = new Vector2(0f, 1f);

                for (int i = 0; i < count; i++)
                {
                    GameObject slotGo = Instantiate(_slotUiPrefab);
                    slotGo.transform.SetParent(_contentAreaRT.transform);
                    slotGo.SetActive(true);
                    slotGo.AddComponent<PreviewItemSlot>();

                    slotGo.transform.localScale = Vector3.one; // ���� �ذ�

                    HideGameObject(slotGo);

                    __previewSlotGoList.Add(slotGo);
                }

                DrawGrid();
                SetImageAlpha();
            }
            void DrawGrid()
            {
                Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
                Vector2 curPos = beginPos;

                // Draw Slots
                int index = 0;
                for (int j = 0; j < _verticalSlotCount; j++)
                {
                    for (int i = 0; i < _horizontalSlotCount; i++)
                    {
                        GameObject slotGo = __previewSlotGoList[index++];
                        RectTransform slotRT = slotGo.GetComponent<RectTransform>();

                        slotRT.anchoredPosition = curPos;
                        slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);
                        __previewSlotGoList.Add(slotGo);

                        // Next X
                        curPos.x += (_slotMargin + _slotSize);
                    }

                    // Next Line
                    curPos.x = beginPos.x;
                    curPos.y -= (_slotMargin + _slotSize);
                }
            }
            void HideGameObject(GameObject go)
            {
                go.hideFlags = HideFlags.HideAndDontSave;

                Transform tr = go.transform;
                for (int i = 0; i < tr.childCount; i++)
                {
                    tr.GetChild(i).gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
            }
            void SetImageAlpha()
            {
                foreach (var go in __previewSlotGoList)
                {
                    var images = go.GetComponentsInChildren<Image>();
                    foreach (var img in images)
                    {
                        img.color = new Color(img.color.r, img.color.g, img.color.b, __previewAlpha);
                        var outline = img.GetComponent<Outline>();
                        if (outline)
                            outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, __previewAlpha);
                    }
                }
            }
        }

        private class PreviewItemSlot : MonoBehaviour { }

        [UnityEditor.InitializeOnLoad]
        private static class Destroyer
        {
            private static Queue<GameObject> targetQueue = new Queue<GameObject>();

            static Destroyer()
            {
                UnityEditor.EditorApplication.update += () =>
                {
                    for (int i = 0; targetQueue.Count > 0 && i < 100000; i++)
                    {
                        var next = targetQueue.Dequeue();
                        DestroyImmediate(next);
                    }
                };
            }
            public static void Destroy(GameObject go) => targetQueue.Enqueue(go);
        }
#endif

        #endregion
    }
}