using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UserData
{
    public class UserDataManager : MonoBehaviour
    {
        public static UserDataManager Instance;

        void Start()
        {
            if(Instance is null)
            {
                Instance = this;
            }
        }
    }
}
