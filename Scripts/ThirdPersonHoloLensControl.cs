/*
 * Third Person controller - takes input from the controller and controlls caharacter movement
 * 
 * author: Klaudia Fajtova 
 * login: xfajto00
 * 
 */

using UnityEngine;
using HoloLensXboxController;
using UnityStandardAssets.Characters.ThirdPerson;
using HoloToolkit.Unity.SpatialMapping;

public class ThirdPersonHoloLensControl : MonoBehaviour
{
    private ControllerInput controllerInput;
    private ThirdPersonCharacter m_Character;
    private Transform m_Cam;
    private Vector3 m_CamForward;
    private Vector3 m_Move;    

    public float RotateAroundYSpeed = 2.0f;
    public float RotateAroundXSpeed = 2.0f;
    public float RotateAroundZSpeed = 2.0f;
    public float MoveHorizontalSpeed = 1f;
    public float MoveVerticalSpeed = 1f;
    public float ScaleSpeed = 1f;

    public bool chosen = false;
    public bool again = false;
    public bool endScan = false;
    private bool m_Jump;

    void Start()
    {
        controllerInput = new ControllerInput(0, 0.19f);

        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }

        m_Character = GetComponent<ThirdPersonCharacter>();
    }

    void Update()
    {
        controllerInput.Update();
        
        if (!m_Jump)
        {
            m_Jump = controllerInput.GetButton(ControllerButton.A);
        }

        bool button = controllerInput.GetButton(ControllerButton.RightShoulder);
        if (button)
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = !SpatialMappingManager.Instance.DrawVisualMeshes;
        }

        //debug purpose only
        /*bool und = controllerInput.GetButton(ControllerButton.LeftShoulder);
        if (und)
        {
            SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>().DrawProcessedMesh = !SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>().DrawProcessedMesh;
        }*/

        if (again)
        {
            if (controllerInput.GetButton(ControllerButton.X))
            {
                again = false;
                chosen = true;
            }
        }

        if (controllerInput.GetButton(ControllerButton.X))
            endScan = true;

    }


    private void FixedUpdate()
    {
        float h = MoveHorizontalSpeed * controllerInput.GetAxisLeftThumbstickX();
        float v = MoveVerticalSpeed * controllerInput.GetAxisLeftThumbstickY();
        bool crouch = controllerInput.GetButton(ControllerButton.B);

        if (m_Cam != null)
        {
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
        }

        m_Character.Move(m_Move, crouch, m_Jump);
        m_Jump = false;        
    }
}