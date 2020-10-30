using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera creatureCamera;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject creaturesParent;

    private bool _isPlayerCamera = true;
    private Creature _creatureTarget;

    // Start is called before the first frame update
    void Start()
    {
        creatureCamera.enabled = false;
        creatureCamera.gameObject.SetActive(false);
        creatureCamera.transform.localEulerAngles = new Vector3(45, 0, 0);
        var toggleFollow = inputActions.FindAction("ToggleFollow");
        var nextCreature = inputActions.FindAction("NextCameraTarget");
        toggleFollow.performed += OnToggleCamera;
        nextCreature.performed += OnNextCreature;
        toggleFollow.Enable();
        nextCreature.Enable();
    }

    private void OnNextCreature(InputAction.CallbackContext obj)
    {
        var creatures = creaturesParent.GetComponent<ProceduralCreature>().Creatures;
        var current = creatures.IndexOf(_creatureTarget);
        if (current < 0)
        {
            _creatureTarget = creatures.FirstOrDefault();
        }
        else
        {
            _creatureTarget = creatures.ElementAt(++current % creatures.Count);
        }
    }

    private void OnToggleCamera(InputAction.CallbackContext obj)
    {
        _isPlayerCamera = !_isPlayerCamera;
        if (_isPlayerCamera)
        {
            creatureCamera.enabled = false;
            creatureCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            playerCamera.enabled = true;
        }
        else
        {
            OnNextCreature(default);
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
            creatureCamera.gameObject.SetActive(true);
            creatureCamera.enabled = true;
        }
    }

    void Update()
    {
        if (_isPlayerCamera)
        {
            return;
        }

        creatureCamera.transform.position =
            _creatureTarget.Position
                + Vector3.back * 10.0f
                + Vector3.up * 10.0f;
    }
}
