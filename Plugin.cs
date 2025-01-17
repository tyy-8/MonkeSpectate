using System;
using BepInEx;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilla;

namespace MonkeSpectate
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public bool enabled;
        public GameObject ShoulderCamera;
        public bool spectating;
        public GameObject spectatingPlayer;
        public float smoothness = 0.1f;
        public float fov = 60;

        public void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                enabled = !enabled;
            }
            
            if (ShoulderCamera == null)
            {
                ShoulderCamera = GameObject.Find("Shoulder Camera");
            }
            else
            {
                
                if (ShoulderCamera.GetComponent<Camera>() != null)
                {
                    ShoulderCamera.GetComponent<Camera>().fieldOfView = fov;
                }
                
            }
            if (spectating)
            {
                ShoulderCamera.GetComponent<CinemachineBrain>().enabled = false;
                if (spectatingPlayer != null)
                {
                    ShoulderCamera.transform.position = Vector3.Lerp(
                        ShoulderCamera.transform.position,
                        spectatingPlayer.transform.position + new Vector3(0, 0.1f, 0f) + spectatingPlayer.transform.forward * 0.2f,
                        Time.deltaTime / smoothness * 15f
                    );
                    ShoulderCamera.transform.rotation = Quaternion.Lerp(
                        ShoulderCamera.transform.rotation,
                        spectatingPlayer.transform.rotation,
                        Time.deltaTime / smoothness * 15f
                    );

                }
            }
            else
            {
                ShoulderCamera.GetComponent<CinemachineBrain>().enabled = true;
            }
        }
        
        public void OnGUI()
        {
            if (enabled)
            {
                GUILayout.Label("Ty's Spectator GUI (Toggle with 'TAB')");
                spectating = GUILayout.Toggle(spectating, "Spectate (IN ROOM ONLY!!)");
            
            

                if (spectating && NetworkSystem.Instance.InRoom)
                {
                    
                    GUILayout.Label("Smoothness");
                    smoothness = GUILayout.HorizontalSlider(smoothness, 0.1f, 1f);
            
            
                    GUILayout.Label("Field Of View");
                    fov = GUILayout.HorizontalSlider(fov, 40, 130);
                    
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (GUILayout.Button(rig.Creator.NickName))
                        {
                            spectatingPlayer = rig.headMesh;
                        }
                    }
                }
            }
        }
    }
}