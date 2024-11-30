using UnityEngine;
using UnityEngine.Rendering.Universal;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [FoldoutGroup("Movement Settings")]
    [SerializeField] private float speed;

    [FoldoutGroup("Light Settings")]
    [SerializeField] private Light2D _light;

    [FoldoutGroup("Sound Settings")]
    [SerializeField] private float soundInterval;

    [FoldoutGroup("Audio Settings")]
    [SerializeField] private AudioClip whistleClip;
    [FoldoutGroup("Audio Settings")]
    [SerializeField] private AudioClip deathSound;

    [FoldoutGroup("Particle Settings")]
    [SerializeField] private GameObject particlePrefab;

    private FootSteps footSteps;
    private float runMultiplier = 2.2f;
    private float movementSpeed;
    private Rigidbody2D rb;
    private float lastSoundTime = -1f;
    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;

    public bool isDead { get; set; }
    public bool controlsEnabled { get; set; }
    public bool isHiding { get; set; }
	public bool isInteracting { get; set; }


    void Start()
    {
        isDead = false;
        isHiding = false;
        isInteracting = false;
        controlsEnabled = true;
        rb = GetComponent<Rigidbody2D>();
        footSteps = GetComponent<FootSteps>();
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.E)){
            isInteracting = true;
        }
        else if(Input.GetKeyUp(KeyCode.E)){
            isInteracting = false;
        }


        if(Input.GetKeyDown(KeyCode.Space)){
            SoundManager.Instance.PlaySoundClip(whistleClip, transform, SoundManager.SoundType.LOUD_FX, SoundManager.SoundFXType.FX);
            ParticleSystem particle = particlePrefab.GetComponent<ParticleSystem>();
            particle.emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, 60)
            });
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }

        if(isDead || !controlsEnabled){return;}

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            movementSpeed = speed * runMultiplier;
            footSteps.speed = 0.6f;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            movementSpeed = speed * 0.5f;
            footSteps.speed = 1.4f;
        }
        else
        {
            movementSpeed = speed;
            footSteps.speed = 1f;
        }
        Vector2 movement = new Vector2(horizontal, vertical);

        if (movement.magnitude > 0 && Time.time - lastSoundTime > soundInterval)
        {
            // LightManager.Instance.AddSoundPosition(transform.position);
            lastSoundTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0)
        {
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        rb.velocity = new Vector2(horizontal * movementSpeed, vertical * movementSpeed);
    }


    public void Die(){
        isDead = true;
        SoundManager.Instance.PlaySoundClip(deathSound, transform, SoundManager.SoundType.LOUD_FX, SoundManager.SoundFXType.FX);
        ParticleSystem particle = particlePrefab.GetComponent<ParticleSystem>();
        particle.emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 60)
        });

        particlePrefab.GetComponent<AttachGameObjectsToParticles>().m_Prefab.GetComponent<Light2D>().color = Color.red;
        Instantiate(particlePrefab, transform.position, Quaternion.identity);

        GetComponent<SpriteRenderer>().color = Color.red;

        Invoke("MainMenu", 3f);
    }

    void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}
