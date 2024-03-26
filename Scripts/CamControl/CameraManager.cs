using Cinemachine;
using SnowIsland.Scripts.Character;
using UnityEngine;

namespace SnowIsland.Scripts.CamControl
{
    public class CameraManager: MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera topdownCam;

        private void Start()
        {
            topdownCam.Follow = PlayerCharacter.Local.transform;
            topdownCam.LookAt = null;
        }
    }
}