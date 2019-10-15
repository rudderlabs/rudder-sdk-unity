using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

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
        public Light faceLight;								// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.


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

            // Dictionary<string, object> eventProperty = new TrackPropertyBuilder()
            // .SetCategory("PlayerShooting_Shoot")
            // .Build();
            // eventProperty.Add("dummy_e_prop_1", "dummy_e_prop_1_value");
            // eventProperty.Add("dummy_e_prop_2", "dummy_e_prop_2_value");
            // eventProperty.Add("dummy_e_prop_3", "dummy_e_prop_3_value");
            // eventProperty.Add("score", ScoreManager.score);

            // Dictionary<string, object> userProperty = new Dictionary<string, object> ();
            // userProperty.Add("dummy_prop_1", "dummp_prop_1_value");
            // userProperty.Add("dummy_prop_2", "dummp_prop_2_value");

            // RudderElement element = new RudderElementBuilder()
            // .WithEventName("PlayerShooting_Shoot")
            // .WithUserId("test_user_id_sayan_android")
            // .WithEventProperties(eventProperty)
            // .WithUserProperties(userProperty)
            // .Build();

            // element.integrations = new Dictionary<string, object>();
            // element.integrations.Add("All", false);
            // element.integrations.Add("AM", true);
            // element.integrations.Add("GA", true);

            // PlayerMovement.rudderClient.Track(element);

            try
            {
                //Every event has an embedded properties structure
                //First we will build the Properties structure
                //Then we will build the encapsulating event structure
                TrackPropertyBuilder propertyBuilder = new TrackPropertyBuilder();
                propertyBuilder.SetCategory("revenue");

                Dictionary<string, object> recordPurchaseProperties = propertyBuilder.Build();

                recordPurchaseProperties.Add("productId", "test_product_id");
                recordPurchaseProperties.Add("price", 2.0099);
                recordPurchaseProperties.Add("quantity", 1);
                recordPurchaseProperties.Add("revenueType", "iOS");
                recordPurchaseProperties.Add("currency", null);

                //Add the FoolProofParams
                Dictionary<string, object> eventData = new Dictionary<string, object>(); //AnalyticsUtils.FoolProofParams(GetCommonEventData());
                foreach (var key in eventData.Keys)
                {
                    var value = eventData[key];
                    if (value != null)
                    {
                        recordPurchaseProperties.Add(key, value);
                    }
                }

                //Now build the event structure
                RudderElementBuilder elementBuilder = new RudderElementBuilder();
                elementBuilder.WithEventName("revenue");

                //Set user id if available
                if (true /* WynnEngine.PlayerId.HasValue() */)
                {
                    elementBuilder.WithUserId("test_player_id");
                }

                //Add the properties structure created to the event
                elementBuilder.WithEventProperties(recordPurchaseProperties);

                // Create the event object
                RudderElement element = elementBuilder.Build();

                // Set the integrations
                element.integrations = new Dictionary<string, object>();
                element.integrations.Add("All", true);

                //Invoke track method
                PlayerMovement.rudderClient.Track(element);

                // GameEngine.LogError("RudderAnalyticsManager: Track: revenue");
            }
            catch (Exception e)
            {
                Debug.Log(e);
                // GameEngine.LogError("RudderAnalyticsManager: Track: Error: " + e.Message);
            }
        }
    }
}