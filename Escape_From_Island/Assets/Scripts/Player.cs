using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 5;//ĳ���� �̼� ����
    public GameObject InventoryManager;//�κ��丮 ���� ���ӿ�����Ʈ ����
    public GameObject MenuManager;//�޴� ȣ�� ���� ���ӿ�����Ʈ 
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
        //����(���ΰ�),����(���ϰ�)���⿡ ���� �Է��� �޾ƿ�
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //normalized: ���Ͱ��� ������
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //ȭ�� �󿡼� ���콺�� ��ġ���� ��ȯ��(���߿� 1��Ī���� �ٲܼ�����)
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
