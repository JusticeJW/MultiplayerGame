using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    public class Skill : MonoBehaviourPunCallbacks
    {
        int damage = 200;
        public float SkillSpeed = 100.0f;
        Player Caller;
        public Player Owner { get; private set; }

        public void Start()
        {
            Destroy(gameObject, 3.0f);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.transform.parent.GetComponent<PhotonView>())
            {
                if (collision.gameObject.transform.parent.GetComponent<PhotonView>().Owner.GetTeam() != Caller.GetTeam())
                {
                    object HP;
                    if (collision.gameObject.transform.parent.GetComponent<PhotonView>().Owner.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterHPProp, out HP))
                    {
                        int hp;
                        hp = (int)HP;
                        hp -= damage;

                        Hashtable newHP = new Hashtable();
                        newHP[PunPlayerProps.PlayerCharacterHPProp] = hp;
                        collision.gameObject.transform.parent.GetComponent<PhotonView>().Owner.SetCustomProperties(newHP);

                        collision.gameObject.transform.parent.GetComponent<Character>().OnHitBySkill(collision.gameObject.transform.parent.GetComponent<PhotonView>().Owner, newHP);
                    }
                    Destroy(gameObject);
                }
            }
        }

        public void InitializeSkill(Player owner, Vector3 originalDirection, float lag, Player caller)
        {
            Caller = caller;
            Owner = owner;

            transform.forward = originalDirection;

            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = originalDirection * SkillSpeed;
            rigidbody.position += rigidbody.velocity * lag;
        }
    }
}
