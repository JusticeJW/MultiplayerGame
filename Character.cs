using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

namespace Photon.Pun.MyGames.MultiplayerGame
{
    public class Character : MonoBehaviourPunCallbacks
    {
        [SerializeField]private Canvas canvas;
        public TMP_Text HpText;
        private PhotonView view;
        public PhotonView GetView() { return view; }

        public GameObject SkillTest;

        private new Collider collider;
        private new Renderer renderer;

        public NavMeshAgent agent;

        private Camera cam;

        private bool controllable = true;

        public int Health;

        private float shootingTimer = 0.0f;

        private void Awake()
        {
            view = GetComponent<PhotonView>();

            collider = GetComponent<Collider>();
            renderer = GetComponent<Renderer>();

            agent = GetComponent<NavMeshAgent>();

        }

        // Start is called before the first frame update
        void Start()
        {
            cam = Camera.main.GetComponent<Camera>();
        }

        [PunRPC]
        public void UpdateCharProps(Player p)
        {
            object playerHP;
            if (p.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterHPProp, out playerHP))
            {
                Health = (int)playerHP;
                HpText.text = playerHP.ToString();
            }

        }

        // Update is called once per frame
        void Update()
        {
            canvas.transform.LookAt(cam.transform);

            if (!view.IsMine || !controllable)
            {
                return;
            }

            // rotation = Input.GetAxis("Horizontal");
            //acceleration = Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Move"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    //Debug.Log("Techies has set new destination: " + hitInfo.point);
                    //Instantiate(GameManagerMG.Instance.NavigateClickParticle, new Vector3(hitInfo.point.x, hitInfo.point.y + 0.5f, hitInfo.point.z), Quaternion.Euler(-90, 0, 0));
                    agent.SetDestination(hitInfo.point);
                }
            }

            if (Input.GetButton("Q") && shootingTimer <= 0.0)
            {
                Debug.Log("Q pressed");
                shootingTimer = 0.2f;
                view.RPC("Q", RpcTarget.AllViaServer, transform.position, transform.rotation, PhotonNetwork.LocalPlayer);
            }

            if (shootingTimer > 0.0f)
            {
                shootingTimer -= Time.deltaTime;
            }
        }

        [PunRPC]
        public void Q(Vector3 position, Quaternion rotation, Player caller, PhotonMessageInfo info)
        {
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            Debug.Log("Beginning SkillTest");
            GameObject skill;

            /** Use this if you want to fire one bullet at a time **/
            skill = Instantiate(SkillTest, transform.position, Quaternion.identity) as GameObject;
            skill.SetActive(true);
            skill.GetComponent<Skill>().InitializeSkill(view.Owner, (rotation * Vector3.forward), Mathf.Abs(lag), caller);


            /** Use this if you want to fire two bullets at once **/
            //Vector3 baseX = rotation * Vector3.right;
            //Vector3 baseZ = rotation * Vector3.forward;

            //Vector3 offsetLeft = -1.5f * baseX - 0.5f * baseZ;
            //Vector3 offsetRight = 1.5f * baseX - 0.5f * baseZ;

            //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetLeft, Quaternion.identity) as GameObject;
            //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
            //bullet = Instantiate(BulletPrefab, rigidbody.position + offsetRight, Quaternion.identity) as GameObject;
            //bullet.GetComponent<Bullet>().InitializeBullet(photonView.Owner, baseZ, Mathf.Abs(lag));
        }

        public void OnHitBySkill(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PunPlayerProps.PlayerCharacterHPProp))
            {
                Debug.Log("Has HP");
                object HP;
                if (targetPlayer.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterHPProp, out HP))
                {
                    Health = (int)HP;
                    Debug.Log(Health);
                    HpText.GetComponent<TMP_Text>().text = Health.ToString();
                    if (Health <= 0)
                    {
                        object lives;
                        if (view.Owner.CustomProperties.TryGetValue(PunPlayerProps.PlayerCharacterLivesProp, out lives))
                        {
                            lives = (int)lives - 1;

                            Hashtable props = new Hashtable
                            {
                                {PunPlayerProps.PlayerCharacterLivesProp, lives}
                            };
                            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                        }

                    }
                }
            }
        }
    }
}
