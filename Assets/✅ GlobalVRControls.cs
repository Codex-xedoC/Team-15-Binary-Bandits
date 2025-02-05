using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities; 

public class GlobalVRControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }

    public GlobalVRControls()
    {
        asset = InputActionAsset.FromJson(@"{
            ""name"": ""GlobalVRControls"",
            ""maps"": [
                {
                    ""name"": ""Player"",
                    ""actions"": [
                        { ""name"": ""Move"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                        { ""name"": ""Rotate"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                        { ""name"": ""Jump"", ""type"": ""Button"", ""expectedControlType"": ""Button"" },
                        { ""name"": ""Sprint"", ""type"": ""Button"", ""expectedControlType"": ""Button"" },
                        { ""name"": ""Climb"", ""type"": ""Button"", ""expectedControlType"": ""Button"" },
                        { ""name"": ""Interact"", ""type"": ""Button"", ""expectedControlType"": ""Button"" },
                        { ""name"": ""SelectA"", ""type"": ""Button"", ""expectedControlType"": ""Button"" },
                        { ""name"": ""SelectB"", ""type"": ""Button"", ""expectedControlType"": ""Button"" },
                        { ""name"": ""SelectC"", ""type"": ""Button"", ""expectedControlType"": ""Button"" }
                    ],
                    ""bindings"": [
                        { ""path"": ""<XRController>{RightHand}/primary2DAxis"", ""action"": ""Move"" },
                        { ""path"": ""<XRController>{RightHand}/primary2DAxis"", ""action"": ""Rotate"" },
                        { ""path"": ""<XRController>{RightHand}/primaryButton"", ""action"": ""Jump"" },
                        { ""path"": ""<XRController>{LeftHand}/primaryButton"", ""action"": ""Sprint"" },
                        { ""path"": ""<XRController>{RightHand}/secondaryButton"", ""action"": ""Climb"" },
                        { ""path"": ""<XRController>{RightHand}/trigger"", ""action"": ""Interact"" },
                        { ""path"": ""<XRController>{RightHand}/buttonSouth"", ""action"": ""SelectA"" },
                        { ""path"": ""<XRController>{RightHand}/buttonWest"", ""action"": ""SelectB"" },
                        { ""path"": ""<XRController>{RightHand}/buttonNorth"", ""action"": ""SelectC"" }
                    ]
                }
            ],
            ""controlSchemes"": []
        }");

        // Setup Action Maps
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Rotate = m_Player.FindAction("Rotate", throwIfNotFound: true);
        m_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Climb = m_Player.FindAction("Climb", throwIfNotFound: true);
        m_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_SelectA = m_Player.FindAction("SelectA", throwIfNotFound: true);
        m_SelectB = m_Player.FindAction("SelectB", throwIfNotFound: true);
        m_SelectC = m_Player.FindAction("SelectC", throwIfNotFound: true);
    }

    public void Dispose() => UnityEngine.Object.Destroy(asset);
    public InputBinding? bindingMask { get; set; }
    public ReadOnlyArray<InputDevice>? devices { get; set; } // FIXED ReadOnlyArray<> ERROR
    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes; // FIXED

    public IEnumerator<InputAction> GetEnumerator() => asset.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public bool Contains(InputAction action) => asset.Contains(action);
    public void Enable() => asset.Enable();
    public void Disable() => asset.Disable();

    // Player Action Map
    private readonly InputActionMap m_Player;
    private readonly InputAction m_Move;
    private readonly InputAction m_Rotate;
    private readonly InputAction m_Jump;
    private readonly InputAction m_Sprint;
    private readonly InputAction m_Climb;
    private readonly InputAction m_Interact;
    private readonly InputAction m_SelectA;
    private readonly InputAction m_SelectB;
    private readonly InputAction m_SelectC;

    // Access Player Actions
    public struct PlayerActions
    {
        private GlobalVRControls m_Wrapper;
        public PlayerActions(GlobalVRControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Move;
        public InputAction @Rotate => m_Wrapper.m_Rotate;
        public InputAction @Jump => m_Wrapper.m_Jump;
        public InputAction @Sprint => m_Wrapper.m_Sprint;
        public InputAction @Climb => m_Wrapper.m_Climb;
        public InputAction @Interact => m_Wrapper.m_Interact;
        public InputAction @SelectA => m_Wrapper.m_SelectA;
        public InputAction @SelectB => m_Wrapper.m_SelectB;
        public InputAction @SelectC => m_Wrapper.m_SelectC;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
    }
    public PlayerActions @Player => new PlayerActions(this);
}
