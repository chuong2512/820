 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Share : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void share()
    {
       // new NativeShare().AddFile(null).SetSubject("WhozItz ").SetText("GAME LINK HERE").Share();
        new NativeShare().SetSubject("WhozItz ").SetText("GAME LINK HERE").Share();
    }
}
