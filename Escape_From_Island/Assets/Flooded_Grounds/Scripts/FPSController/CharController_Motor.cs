using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController_Motor : MonoBehaviour {

	//캐릭터 이동속도 설정
	public float movespeed = 10.0f;
	//캐릭터 감도 설정
	public float sensitivity = 30.0f;
	//캐릭터가 물에 있을때 수력설정
	public float WaterHeight = 15.5f;
	//캐릭터 컨트롤러 캐릭터변수지정
	CharacterController character;
	//캐릭터 게임오브젝트 지정
	public GameObject cam;
	//캐릭터의 상하좌우 값을 지정
	float moveFB, moveLR;
	//캐릭터의 시점을 지정
	float rotX, rotY;
	//캐릭터 점프
	private bool is_Jumped;
	//그라운드 여부 확인
	private bool grounded = false;
	//마우스의 왼쪽버튼을 클릭했을때 
	public bool webGLRightClickRotation = true;
	//중력 설정
	float gravity = -9.8f;
	//점프력 설정
	public float JumpForce = 5.0f;
	public Rigidbody pRigidbody;
	public Transform pTransform;
	public GameObject InventoryManager;//인벤토리 호출 기능
	//public GameObject MenuManager;//메뉴기능 아직 창안만듬
	void Start(){
		//LockCursor ();
		character = GetComponent<CharacterController> ();
		if (Application.isEditor) {
			webGLRightClickRotation = false;
			sensitivity = sensitivity * 1.5f;
		}
		//충돌체를 가져옴
		pRigidbody = GetComponent<Rigidbody>();
		//트랜스폼을 가져옴
		pTransform = GetComponent<Transform>();
		InventoryManager.SetActive(false);
		//MenuManager.SetActive(false);
	}

	//캐릭터가 물에 있는지 없는지 여부를 확인하고 반환함
	void CheckForWaterHeight(){
		if (transform.position.y < WaterHeight) {
			gravity = 0f;			
		} else {
			gravity = -9.8f;
		}
	}

	void InventoryCheck()//인벤토리 창을 열수 있는 기능
    {
        if(Input.GetKeyUp(KeyCode.I)){
			InventoryManager.SetActive(true);
        }
    }
	/*
	void MenuCheck()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
			MenuManager.SetActive(false);
        }
    }
	*/
	void Update(){

		Move();
		Jump();
		CheckGround();
		InventoryCheck();

		rotX = Input.GetAxis ("Mouse X") * sensitivity;
		rotY = Input.GetAxis ("Mouse Y") * sensitivity;

		CheckForWaterHeight ();
		
		Vector3 movement = new Vector3 (moveFB, gravity, moveLR);

		if (webGLRightClickRotation) {
			if (Input.GetKey (KeyCode.Mouse0)) {
				CameraRotation (cam, rotX, rotY);
			}
		} else if (!webGLRightClickRotation) {
			CameraRotation (cam, rotX, rotY);
		}

		movement = transform.rotation * movement;
		character.Move (movement * Time.deltaTime);
	}


	void CameraRotation(GameObject cam, float rotX, float rotY){		
		transform.Rotate (0, rotX * Time.deltaTime, 0);
		cam.transform.Rotate (-rotY * Time.deltaTime, 0, 0);
	}
	//점프 관련 스크립트
	private void Jump()
    {
		//스페이스바를 눌렀을때
        if (Input.GetKeyUp(KeyCode.Space)&&grounded)
        {
			is_Jumped = true;
			if(is_Jumped)
            {
				Vector3 jumpvelocity = Vector3.up * Mathf.Sqrt(JumpForce * -Physics.gravity.y);

				pRigidbody.AddForce(jumpvelocity, ForceMode.Impulse);
				is_Jumped = false;
            }
        }
    }
	//
    private void CheckGround()
    {
		RaycastHit hit;
		Debug.DrawRay(pTransform.position, Vector3.down * 0.1f, Color.red);

		if(Physics.Raycast(pTransform.position,Vector3.down,out hit,0.1f))
        {
			if(hit.transform.tag !=null)
            {
				grounded = true;
				return;
            }
        }
    }
    private void Move()
    {
		moveFB = Input.GetAxisRaw("Horizontal") * movespeed;
		moveLR = Input.GetAxisRaw("Vertical") * movespeed;
		Vector3 moveDir = (Vector3.forward * moveFB) + (Vector3.right * moveLR);
		pTransform.Translate(moveDir.normalized * Time.deltaTime * movespeed);
	}

}
