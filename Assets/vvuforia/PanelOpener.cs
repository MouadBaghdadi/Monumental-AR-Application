using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class PanelOpener : MonoBehaviour {
//     public GameObject panel;  // Panel d'information
//     public GameObject earth;  // Terre 3D avec pin

//     private bool showPanel = true;

//     void Start() {
//         if (panel != null && earth != null) {
//             panel.SetActive(true);    // Affiche le panneau
//             earth.SetActive(false);   // Cache la Terre au début
//         }
//     }

//     public void OpenPanel() {
//         showPanel = !showPanel;

//         if (panel != null && earth != null) {
//             panel.SetActive(showPanel);
//             earth.SetActive(!showPanel);
//         }
//     }
// }

public class PanelOpener : MonoBehaviour {
    public GameObject Panel;
    public void OpenPanel(){
        if (Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
