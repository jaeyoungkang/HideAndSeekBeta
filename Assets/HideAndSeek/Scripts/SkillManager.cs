using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HideAndSeek
{
    public enum SKILL_TYPE { HEAL_A, HEAL_B, DESTROY_A, DESTROY_B, DESTROY_C, DESTROY_D, SHOW_A, SHOW_B, SHOW_C, SHOW_D };
    public class SkillManager : MonoBehaviour
    {
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

        public List<SKILL_TYPE> mySkills = new List<SKILL_TYPE>();

        public void SetMySkills()
        {
            mySkills.Add(SKILL_TYPE.DESTROY_A);
            mySkills.Add(SKILL_TYPE.DESTROY_B);
            mySkills.Add(SKILL_TYPE.SHOW_A);
            mySkills.Add(SKILL_TYPE.SHOW_B);
            mySkills.Add(SKILL_TYPE.HEAL_A);
            mySkills.Add(SKILL_TYPE.HEAL_B);
        }

        public List<SKILL_TYPE> GetMySkills() { return mySkills;  }

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
}