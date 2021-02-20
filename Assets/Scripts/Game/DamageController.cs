//docs: https://github.com/LiterallyInc/SajberRoyale/issues/9

using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Items;
using System.Collections;
using UnityEngine;

namespace SajberRoyale.Player
{
    public class DamageController : MonoBehaviourPun
    {
        public AudioClip[] damageSounds;

        /// <summary>
        /// Runs for everyone when someone gets hit
        /// </summary>
        /// <param name="player">Actor ID of player who got hit</param>
        /// <param name="damage">How much damage the hit did</param>
        /// <param name="weapon">Weapon ID of weapon that hit player</param>
        /// <param name="info">Info about the player who hit</param>
        [PunRPC]
        private void Hit(int player, int damage, string weapon, PhotonMessageInfo info)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == player) //i got hit
            {
                Game.Game.Instance.HP -= damage;
                GetComponent<AudioSource>().clip = damageSounds[Random.Range(0, damageSounds.Length)];
                GetComponent<AudioSource>().Play();
                if (Game.Game.Instance.HP <= 0)
                {
                    Game.Game.Instance.IsAlive = false;
                    PhotonNetwork.Destroy(Core.Instance.Player.gameObject); //destroy avatar
                    photonView.RPC("Die", RpcTarget.All, info.Sender.ActorNumber, weapon);
                }
            }
            else //someone else got hit
            {
            }
        }

        /// <summary>
        /// Runs for everyone when someone uses their weapon
        /// </summary>
        /// <param name="weaponID">Weapon ID of weapon that shot</param>
        /// <param name="info">Info about the player who shot</param>
        [PunRPC]
        private void Fire(string weaponID, PhotonMessageInfo info)
        {
            GameObject player = GameObject.Find($"Player{info.Sender.ActorNumber}");
            Weapon weapon = (Weapon)ItemDatabase.Instance.GetItem(weaponID);
            player.GetComponent<AudioSource>().maxDistance = weapon.range * 1.25f;
            player.GetComponent<AudioSource>().clip = weapon.shootSFX;
            player.GetComponent<AudioSource>().Play();
        }

        /// <summary>
        /// Plays a death effect for everyone. F
        /// </summary>
        /// <param name="killer">Player actor ID who killed player</param>
        /// <param name="weaponID">Weapon ID that killed player</param>
        /// <param name="info">Player who dies</param>
        [PunRPC]
        private void Die(int killer, string weaponID, PhotonMessageInfo info)
        {
            Debug.Log($"{info.Sender.NickName} got killed by {PhotonNetwork.CurrentRoom.GetPlayer(killer).NickName} using {ItemDatabase.Instance.GetItem(weaponID).name}");
            Transform dead = Core.Instance.GetPlayer(info.Sender.ActorNumber);
            dead.GetComponent<PlayerSync>().DeathParticles.Play();
            if (info.Sender.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) Destroy(dead.GetComponent<CharacterController>());
            if (PhotonNetwork.LocalPlayer.ActorNumber == killer) Game.Game.Instance.Kills++;
            Game.Game.Instance.AlivePlayers--;
        }
    }
}