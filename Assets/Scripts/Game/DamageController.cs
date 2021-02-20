//docs: https://github.com/LiterallyInc/SajberRoyale/issues/9

using Photon.Pun;
using SajberRoyale.Game;
using SajberRoyale.Items;
using UnityEngine;

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
            Game.Instance.HP -= damage;
            GetComponent<AudioSource>().clip = damageSounds[Random.Range(0, damageSounds.Length)];
            GetComponent<AudioSource>().Play();
            if (Game.Instance.HP <= 0) photonView.RPC("Die", RpcTarget.All, player, weapon);
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

    [PunRPC]
    private void Die()
    {
    }
}