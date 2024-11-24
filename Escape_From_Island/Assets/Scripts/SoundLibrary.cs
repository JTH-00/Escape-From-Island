using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//사운드 그룹화 스크립트
public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        foreach(SoundGroup soundGroup in soundGroups)
        {
            groupDictionary.Add(soundGroup.groupID, soundGroup.group);
        }
    }
    //이름에 해당되는 랜덤 사운드를 반환해줌
    public AudioClip GetClipFromName(string name)
    {
        if(groupDictionary.ContainsKey(name))
        {
            AudioClip[] sounds = groupDictionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }
    [System.Serializable]
    public class SoundGroup 
    {
        public string groupID;
        public AudioClip[] group;
    }

}
