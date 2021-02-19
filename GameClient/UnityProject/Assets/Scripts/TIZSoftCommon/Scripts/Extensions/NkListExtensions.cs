using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace TIZSoft.Extensions
{
    /// <summary>
    /// Nick 20171016
    /// </summary>
    public class NkListGroupCtrl<T> : List<T>
    {
        protected       string          activeObjName;
        protected       int             activeIdx;


        public string ActiveObjName
        {
            get { return activeObjName; }
        }

        public int ActiveIdx
        {
            get { return activeIdx; }
        }

        //
        public void SwitchActive(int nIdx)
        {
            int nCount = this.Count;

            if (nIdx < 0 || nIdx >= nCount)
                return;

            for (int i = 0; i < nCount; i++)
            {
                GameObject GetObj = this[i] as GameObject;
                if (GetObj == null)
                    continue;

                //NGUITools.SetActive(GetObj, false);
                GetObj.SetActive(false);
            }

            GameObject GetIdxObj = this[nIdx] as GameObject;

            if (GetIdxObj != null)
            {
                //NGUITools.SetActive(GetIdxObj, true);
                GetIdxObj.SetActive(true);
                activeObjName = GetIdxObj.name;
                activeIdx     = nIdx;
            }
        }

        //
        public Type ActiveComponent<Type>(string findName)
        {
            int nIdx = ActiveIdx;
            GameObject GetIdxObj = this[nIdx] as GameObject;

            Type getCom = GameObjectExtensions.GetObjCompnent<Type>(GetIdxObj, findName);
            return getCom;
        }
    }
}