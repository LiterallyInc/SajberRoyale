using Photon.Pun;
using SajberRoyale.Game;
using System.Collections;
using UnityEngine;

namespace SajberRoyale.Map
{
    public class ElevatorNode : RoomNode
    {
        public bool isActive = false;
        public Animator Door;
        public AudioClip DoorOpen;
        public AudioClip DoorClose;
        public AudioClip Music;
        public AudioSource Source;
        public void Update()
        {
            if (Game.Game.Instance.CurrentRoom.GetType() == typeof(ElevatorNode) && Game.Game.Instance.IsAlive) //you're in an elevator & alive
            {
                ElevatorNode elevator = (ElevatorNode)Game.Game.Instance.CurrentRoom;
                if (Input.GetKeyDown(KeyCode.X) && !elevator.isActive)
                {
                    Core.Instance.photonView.RPC(nameof(Activate), RpcTarget.All, elevator.roomName);
                }
            }
        }
        [PunRPC]
        public void Activate(string elevatorID)
        {
            StartCoroutine(((ElevatorNode)Get(elevatorID)).Use());
        }
        private IEnumerator Use()
        {
            bool imIn = Game.Game.Instance.CurrentRoom.GetType() == typeof(ElevatorNode);
            isActive = true;
            Source.clip = DoorClose;
            Source.Play();

            //Play music at source if player is outside, else 
            if (imIn)
            {
                Core.Instance.DamageController.PlayAudioAtPlayer(PhotonNetwork.LocalPlayer.ActorNumber, 10, DoorClose);
            }
            else
            {
               
            }
            Door.Play("Close", 0, 0);
            yield return new WaitForSeconds(2);
        }
    }
}