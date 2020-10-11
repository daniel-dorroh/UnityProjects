using System.Linq;
using UnityEngine;

public class RocketMotion : MonoBehaviour
{
    [SerializeField]
    private float ThrustForce = 20.0f;
    [SerializeField]
    private float ManeuverAngle = 50.0f;
    [SerializeField]
    private Rigidbody Rocket;
    [SerializeField]
    private AudioSource RocketPulse;
    [SerializeField]
    private AudioSource ManeuverBlast;


    public int Health = 3;

    private bool _hasManeuveredLeft = false;
    private bool _hasManeuveredRight = false;
    private bool _isDestroyed = false;

    public void Start()
    {
        if (Rocket == null)
        {
            Rocket = GetComponent<Rigidbody>();
        }
        if (RocketPulse == null)
        {
            RocketPulse = GetComponent<AudioSource>();
        }
        if (ManeuverBlast == null)
        {
            ManeuverBlast = GetComponent<AudioSource>();
        }
    }

    internal void Update()
    {
        if (_isDestroyed)
        {
            return;
        }
        
        Thrust();
        Maneuver();
    }

    internal void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            default:
                Damage();
                break;
        }
    }

    private void Damage()
    {
        if (Health > 0)
        {
            Health -= 1;
        }
        if (Health <= 0)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        Rocket.velocity.Scale(Vector3.up);
        _isDestroyed = true;
    }

    private void Thrust()
    { 
        if (Input.GetKey(KeyCode.Space))
        {
            Rocket.AddRelativeForce(new Vector3(0, ThrustForce, 0));

            if (!RocketPulse.isPlaying)
            {
                RocketPulse.time = Random.value * 2.0f;
                RocketPulse.loop = true;
                RocketPulse.Play();
            }
        }
        else
        {
            RocketPulse.Stop();
        }
    }

    private void Maneuver()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Rocket.freezeRotation = true;
            _hasManeuveredRight = false;
            transform.Rotate(Vector3.forward, ManeuverAngle * Time.deltaTime);
            _hasManeuveredLeft = PlayManeuverSound(_hasManeuveredLeft);
            Rocket.freezeRotation = false;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rocket.freezeRotation = true;
            _hasManeuveredLeft = false;
            transform.Rotate(Vector3.back, ManeuverAngle * Time.deltaTime);
            _hasManeuveredRight = PlayManeuverSound(_hasManeuveredRight);
            Rocket.freezeRotation = false;
        }
        else
        {
            ManeuverBlast.Stop();
            _hasManeuveredRight = false;
            _hasManeuveredLeft = false;
        }
    }

    private bool PlayManeuverSound(bool hasManeuvered)
    {
        if (!hasManeuvered && !ManeuverBlast.isPlaying)
        {
            hasManeuvered = true;
            ManeuverBlast.time = 0.4f;
            ManeuverBlast.loop = false;
            ManeuverBlast.Play();
        }
        return hasManeuvered;
    }
}
