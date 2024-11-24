using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 5;//캐릭터 이속 설정
    public GameObject InventoryManager;//인벤토리 관련 게임오브젝트 생성
    public GameObject MenuManager;//메뉴 호출 관련 게임오브젝트 
    Camera viewCamera;
    PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
        InventoryManager.SetActive(false);
        MenuManager.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        //수평값(가로값),수직(상하값)방향에 대한 입력을 받아옴
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //normalized: 백터값을 가져옴
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //화면 상에서 마우스의 위치값을 반환함(나중에 1인칭으로 바꿀수있음)
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if(groundPlane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }
    }
}
