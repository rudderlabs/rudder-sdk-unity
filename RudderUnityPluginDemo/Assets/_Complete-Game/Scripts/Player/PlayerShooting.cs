using com.adjust.sdk;
using Rudderlabs;
using UnityEngine;

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.


        float timer;                                    // A timer to determine when to fire.
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
        public Light faceLight;							// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.


        private static RudderClient rudderClient;
        void Awake()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask("Shootable");

            // Set up the references.
            gunParticles = GetComponent<ParticleSystem>();
            gunLine = GetComponent<LineRenderer>();
            gunAudio = GetComponent<AudioSource>();
            gunLight = GetComponent<Light>();
            //faceLight = GetComponentInChildren<Light> ();

            RudderClient.SerializeSqlite();

            string writeKey = "1TSRSskqa15PG7F89tkwEbl5Td8";
            string endPointUrl = "https://76aeb180.ngrok.io";

            RudderConfigBuilder configBuilder = new RudderConfigBuilder()
            .WithEndPointUrl(endPointUrl)
            .WithFactory(RudderAdjustIntegrationFactory.GetFactory())
            .WithLogLevel(RudderLogLevel.DEBUG);
            rudderClient = RudderClient.GetInstance(
                writeKey,
                configBuilder.Build()
            );

            // generic track event before identify
            RudderMessage shootMessage = new RudderMessageBuilder()
                .WithEventName("player_shoot")
                .WithEventProperty("test_key_1", "test_value_1")
                .WithEventProperty("test_key_2", "test_value_2")
                .WithUserProperty("test_user_key_1", "test_user_value_1")
                .WithUserProperty("test_user_key_2", "test_user_value_2")
                .Build();
            rudderClient.Track(shootMessage);

            RudderMessage message1 = new RudderMessageBuilder()
                .WithEventName("daily_rewards_claim")
                .WithEventProperty("test_key_1", "test_value_1")
                .WithEventProperty("test_key_2", "test_value_2")
                .WithUserProperty("test_user_key_1", "test_user_value_1")
                .WithUserProperty("test_user_key_2", "test_user_value_2")
                .Build();
            rudderClient.Track(message1);

            // build a message 
            RudderMessage identifyMessage = new RudderMessageBuilder().Build();
            RudderTraits traits = new RudderTraits().PutEmail("some@example.com");
            rudderClient.Identify("some_user_id", traits, identifyMessage);

            RudderMessage message2 = new RudderMessageBuilder()
                .WithEventName("level_up")
                .WithEventProperty("test_key_1", "test_value_1")
                .WithEventProperty("test_key_2", "test_value_2")
                .WithUserProperty("test_user_key_1", "test_user_value_1")
                .WithUserProperty("test_user_key_2", "test_user_value_2")
                .Build();
            rudderClient.Track(message2);

            // reset identify
            rudderClient.Reset();

            RudderMessage message3 = new RudderMessageBuilder()
                .WithEventName("revenue")
                .WithEventProperty("test_key_1", "test_value_1")
                .WithEventProperty("test_key_2", "test_value_2")
                .WithEventProperty("price", 1.99)
                .WithEventProperty("currency", "USD")
                .WithUserProperty("test_user_key_1", "test_user_value_1")
                .WithUserProperty("test_user_key_2", "test_user_value_2")
                .Build();
            rudderClient.Track(message3);

            // fire another track event to check for reset 
            RudderMessage anotherShootMessage = new RudderMessageBuilder()
                .WithEventName("player_shoot_reset")
                .WithEventProperty("test_key_1", "test_value_1")
                .WithEventProperty("test_key_2", "test_value_2")
                .WithUserProperty("test_user_key_1", "test_user_value_1")
                .WithUserProperty("test_user_key_2", "test_user_value_2")
                .Build();
            rudderClient.Track(anotherShootMessage);
        }


        void Update()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
            if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot();
            }
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects();
            }
        }


        public void DisableEffects()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
            faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot()
        {
            // Reset the timer.
            timer = 0f;

            // Play the gun shot audioclip.
            gunAudio.Play();

            // Enable the lights.
            gunLight.enabled = true;
            faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = transform.position;
            shootRay.direction = transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                // Try and find an EnemyHealth script on the gameobject hit.
                EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                // If the EnemyHealth component exist...
                if (enemyHealth != null)
                {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition(1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }
    }
}