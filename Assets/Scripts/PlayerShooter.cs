using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Transform gunPivot;
    public AudioClip playerGunShot;
    private AudioSource playerAudioSource;
    private PlayerInput playerInput;
    private LineRenderer bulletLineRenderer;
    public ParticleSystem gunParticles;
    public Transform fireTransform;
    public float gunDamage = 50f;
    private float timeBetFire = 0.12f;
    private float lastFireTime;
    private float fireDistance = 50f;  // bulletLineRenderer 길이
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAudioSource = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        bulletLineRenderer.positionCount = 2;  // LineRenderer의 점을 2개 넣어준다. (끝점, 시작점)
        bulletLineRenderer.enabled = false;  // LineRenderer의 끈다.
    }

    private void Update()
    {
        if (playerInput.Fire)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shoot();
        }
    }

    public LayerMask targetLayer;

    public void Shoot()
    {
        Vector3 hitPosition;

        Ray ray = new Ray(fireTransform.position, fireTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fireDistance, targetLayer))
        {
            hitPosition = hit.point;
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(gunDamage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        StartCoroutine(CoShotEffect(hitPosition));
    }

    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        gunParticles.Play();
        playerAudioSource.PlayOneShot(playerGunShot);
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);
        bulletLineRenderer.enabled = false;
    }
}
