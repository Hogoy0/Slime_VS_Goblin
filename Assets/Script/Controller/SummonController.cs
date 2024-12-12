using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SummonController : MonoBehaviour
{
    private Transform target;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;
    [HideInInspector] public Vector3 pos { get { return transform.position; } }

    public GameObject prefabToSpawn;
    public GameObject prefabToSpawnMurge;
    public string targetLayerName = "Ground";
    private float health;
    private int attackPower;
    private CData cData;
    public bool IsLaunched { get; set; } = false;
    public SlimeData SlimeData { get; private set; }

    public enum SlimeType
    {
        None,
        Emerald_Battle,
        Emerald_Battle_Murge,
        Sapphire_Battle,
        Sapphire_Battle_Murge,
        Topaz_Battle,
        Topaz_Battle_Murge,
        Diamond_Battle,
        Diamond_Battle_Murge,
        Ruby_Battle,
        Ruby_Battle_Murge,

    }
    public SlimeType slimeType;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        if (rb != null)
        {
            DeActivateRb();
        }
    }

    // SummonController.cs
    public void Initialize(CData data)
    {
        cData = data;
        SlimeData = null;
    }

    public void Initialize(SlimeData data)
    {
        SlimeData = data;
    }

    public void Push(Vector2 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void ActivateRb()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    public void DeActivateRb()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
            Vector3 MurgespawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            bool CheckRight = false;
            bool CheckLeft = false;
            SlimeType RightRayslimeType = SlimeType.None;
            SlimeType LeftRayslimeType = SlimeType.None;
            GameObject spawnedSlime = null;

            if (prefabToSpawn == null)
            {
                return;
            }

            Vector3 rayOrigin = transform.position;
            Vector3 rayDirection = Vector3.right;
            float rayDistance = 1f;
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red, 2f);


            RaycastHit2D Rightray = Physics2D.Raycast(rayOrigin, rayDirection, 1f, LayerMask.GetMask("Slime"));
            RaycastHit2D Leftray = Physics2D.Raycast(rayOrigin, Vector3.left, 1f, LayerMask.GetMask("Slime"));

            if (Rightray.collider != null)
            {
                Debug.Log("오른쪽에서 충돌 감지!");

                // Ray에 맞은 오브젝트에서 SummonController를 가져오기
                SummonController summonController = Rightray.collider.GetComponent<SummonController>();
                if (summonController != null)
                {
                    // SummonController에서 slimeType 가져오기
                    RightRayslimeType = summonController.slimeType;
                    Debug.Log("충돌한 오브젝트의 SlimeType: " + RightRayslimeType);
                    Debug.Log(Rightray.collider.name);
                    CheckRight = true;
                }
                else
                {
                    Debug.LogWarning("충돌한 오브젝트에 SummonController가 없습니다.");
                }
            }

            if (Leftray.collider != null)
            {
                Debug.Log("왼쪽에서 충돌 감지!");

                // Ray에 맞은 오브젝트에서 SummonController를 가져오기
                SummonController summonController = Leftray.collider.GetComponent<SummonController>();
                if (summonController != null)
                {
                    // SummonController에서 slimeType 가져오기
                    LeftRayslimeType = summonController.slimeType;
                    Debug.Log("충돌한 오브젝트의 SlimeType: " + LeftRayslimeType);
                    Debug.Log(Leftray.collider.name);
                    CheckLeft = true;
                }
                else
                {
                    Debug.LogWarning("충돌한 오브젝트에 SummonController가 없습니다.");
                }
            }

            if (CheckRight == true)
            {
                Debug.Log("던진 슬라임의 타입은" + slimeType);
                Debug.Log("합쳐질 슬라임의 타입은" + RightRayslimeType.ToString());
                if (slimeType == RightRayslimeType)
                {
                    Destroy(Rightray.collider.gameObject);
                    spawnedSlime = Instantiate(prefabToSpawnMurge, MurgespawnPosition, Quaternion.identity);
                    SoundManager.instance.PlaySlimeESfx(SoundManager.SlimeESfx.Slime_Merge);
                    Destroy(gameObject);
                }

                else if (CheckLeft == true)
                {
                    if (slimeType == LeftRayslimeType)
                    {
                        Destroy(Leftray.collider.gameObject);
                        spawnedSlime = Instantiate(prefabToSpawnMurge, MurgespawnPosition, Quaternion.identity);
                        SoundManager.instance.PlaySlimeESfx(SoundManager.SlimeESfx.Slime_Merge);
                        Destroy(gameObject);
                    }
                }
            }

            if (spawnedSlime == null)
            {
                SoundManager.instance.PlaySlimeESfx(SoundManager.SlimeESfx.Slime_Landing);
                spawnedSlime = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                Destroy(gameObject);
            }
            // 소환된 슬라임 초기화

            SummonController newSlimeController = spawnedSlime.GetComponent<SummonController>();
            if (newSlimeController != null && cData != null)
            {
                newSlimeController.Initialize(cData);
            }
        }
    }
}