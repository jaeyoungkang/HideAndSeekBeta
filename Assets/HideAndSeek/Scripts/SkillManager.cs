using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour {
    public Toggle destroyA;
    public Toggle destroyB;
    public Toggle destroyC;
    public Toggle destroyD;

    public int destoryType = 0;

    public void ActiveToggle()
    {
        if (destroyA.isOn)
        {
            destoryType = 0;
        }
        else if (destroyB.isOn)
        {
            destoryType = 1;
        }
        else if (destroyC.isOn)
        {
            destoryType = 2;
        }
        else if (destroyD.isOn)
        {
            destoryType = 3;
        }
    }
}
