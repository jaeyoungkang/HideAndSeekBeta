using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour {
    public Toggle destroyA;
    public Toggle destroyB;
    public Toggle destroyC;
    public Toggle destroyD;

    public Toggle showA;
    public Toggle showB;
    public Toggle showC;
    public Toggle showD;

    public int destoryType = 0;
    public int showType = 0;

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

        if (showA.isOn)
        {
            showType = 0;
        }
        else if (showB.isOn)
        {
            showType = 1;
        }
        else if (showC.isOn)
        {
            showType = 2;
        }
        else if (showD.isOn)
        {
            showType = 3;
        }
    }
}
