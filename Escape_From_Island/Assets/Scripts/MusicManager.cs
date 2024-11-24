using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme; //���� ���� ���� ����
    public AudioClip menuTheme; //�޴� ���带 ������

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayMusic(mainTheme,2);
    }

    // Update is called once per frame
    void Update()
    {
        //�����̽��ٸ� �������� �޴� ���� ����(�ڿ� ���氡��)
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.instance.PlayMusic(mainTheme, 3);
        }
    }
}
