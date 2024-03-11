using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class BasicSoldier : TroopBase
{

    Transform self;


    void Start() {
        self = gameObject.transform;
    }


    public override void UpdateAppearance()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateAnimation()
    {
        throw new System.NotImplementedException();
    }
}
