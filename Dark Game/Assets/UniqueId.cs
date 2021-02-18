using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 
 public class UniqueId : MonoBehaviour
 {
     //I would suggest adding some kind of ReadOnly attribute/inspector to this such that you can see it in the inspector but can't edit it directly
     [SerializeField] private string m_ID = Guid.NewGuid().ToString();
     //If you need to access the ID, use this
     public string ID => m_ID;
 
     //This allows you to re-generate the GUID for this object by clicking the 'Generate New ID' button in the context menu dropdown for this script
     [ContextMenu("Generate new ID")]
     private void RegenerateGUID () => m_ID = Guid.NewGuid().ToString();
}
