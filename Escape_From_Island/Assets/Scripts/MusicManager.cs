using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme; //메인 음악 사운드 설정
    public AudioClip menuTheme; //메뉴 사운드를 설정함

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayMusic(mainTheme,2);
    }

    // Update is called once per frame
    void Update()
    {
        //스페이스바를 눌렀을때 메뉴 사운드 실행(뒤에 변경가능)
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.instance.PlayMusic(mainTheme, 3);
        }
    }
}
