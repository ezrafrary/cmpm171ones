using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class RebindActionUI : MonoBehaviour
    {
        public InputActionReference actionReference
        {
            get => m_Action;
            set
            {
                m_Action = value;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

        public string bindingId
        {
            get => m_BindingId;
            set
            {
                m_BindingId = value;
                UpdateBindingDisplay();
            }
        }

        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set
            {
                m_DisplayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        public TextMeshProUGUI actionLabel
        {
            get => m_ActionLabel;
            set
            {
                m_ActionLabel = value;
                UpdateActionLabel();
            }
        }

        public TextMeshProUGUI bindingText
        {
            get => m_BindingText;
            set
            {
                m_BindingText = value;
                UpdateBindingDisplay();
            }
        }

        public TextMeshProUGUI rebindPrompt
        {
            get => m_RebindText;
            set => m_RebindText = value;
        }

        public GameObject rebindOverlay
        {
            get => m_RebindOverlay;
            set => m_RebindOverlay = value;
        }

        public UpdateBindingUIEvent updateBindingUIEvent
        {
            get
            {
                if (m_UpdateBindingUIEvent == null)
                    m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
                return m_UpdateBindingUIEvent;
            }
        }

        public InteractiveRebindEvent startRebindEvent
        {
            get
            {
                if (m_RebindStartEvent == null)
                    m_RebindStartEvent = new InteractiveRebindEvent();
                return m_RebindStartEvent;
            }
        }

        public InteractiveRebindEvent stopRebindEvent
        {
            get
            {
                if (m_RebindStopEvent == null)
                    m_RebindStopEvent = new InteractiveRebindEvent();
                return m_RebindStopEvent;
            }
        }

        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;

        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;
            action = m_Action?.action;
            if (action == null || string.IsNullOrEmpty(m_BindingId))
                return false;

            var bindingGuid = new Guid(m_BindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingGuid);
            return bindingIndex != -1;
        }

        public void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            var action = m_Action?.action;
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
            }

            if (m_BindingText != null)
                m_BindingText.text = displayString;

            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }

            var key = GetBindingPlayerPrefKey(action, bindingIndex);
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();

            UpdateBindingDisplay();
        }

        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            m_RebindOperation?.Cancel();

            void CleanUp()
            {
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
                action.Enable();
            }

            action.Disable();

            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(operation =>
                {
                    m_RebindStopEvent?.Invoke(this, operation);
                    if (m_RebindOverlay != null) m_RebindOverlay.SetActive(false);
                    UpdateBindingDisplay();
                    CleanUp();
                })
                .OnComplete(operation =>
                {
                    if (m_RebindOverlay != null) m_RebindOverlay.SetActive(false);
                    m_RebindStopEvent?.Invoke(this, operation);
                    SaveBindingOverride(); // Save the override here
                    UpdateBindingDisplay();
                    CleanUp();

                    if (allCompositeParts)
                    {
                        var nextBindingIndex = bindingIndex + 1;
                        if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                            PerformInteractiveRebind(action, nextBindingIndex, true);
                    }
                });

            var partName = action.bindings[bindingIndex].isPartOfComposite ? $"Binding '{action.bindings[bindingIndex].name}'. " : "";

            m_RebindOverlay?.SetActive(true);
            if (m_RebindText != null)
            {
                m_RebindText.text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                    ? $"{partName}Waiting for {m_RebindOperation.expectedControlType} input..."
                    : $"{partName}Waiting for input...";
            }

            if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
                m_BindingText.text = "<Waiting...>";

            m_RebindStartEvent?.Invoke(this, m_RebindOperation);
            m_RebindOperation.Start();
        }

        private void SaveBindingOverride()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex)) return;

            var overridePath = action.bindings[bindingIndex].overridePath;
            var key = GetBindingPlayerPrefKey(action, bindingIndex);
            if (!string.IsNullOrEmpty(overridePath))
                PlayerPrefs.SetString(key, overridePath);
            else
                PlayerPrefs.DeleteKey(key);

            PlayerPrefs.Save();
        }

        private void LoadBindingOverride()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex)) return;

            var key = GetBindingPlayerPrefKey(action, bindingIndex);
            if (PlayerPrefs.HasKey(key))
            {
                var overridePath = PlayerPrefs.GetString(key);
                action.ApplyBindingOverride(bindingIndex, overridePath);
            }
        }




        private string GetBindingPlayerPrefKey(InputAction action, int bindingIndex)
        {
            return $"rebind_{action.actionMap.name}_{action.name}_{bindingIndex}";
        }

        protected void OnEnable()
        {
            if (s_RebindActionUIs == null)
                s_RebindActionUIs = new List<RebindActionUI>();
            s_RebindActionUIs.Add(this);
            LoadBindingOverride(); // Load saved override
            if (s_RebindActionUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;
        }

        protected void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;
            s_RebindActionUIs.Remove(this);
            if (s_RebindActionUIs.Count == 0)
            {
                s_RebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged) return;

            var action = obj as InputAction;
            var actionMap = action?.actionMap ?? obj as InputActionMap;
            var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

            foreach (var component in s_RebindActionUIs)
            {
                var referencedAction = component.actionReference?.action;
                if (referencedAction == null) continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

        [SerializeField] private InputActionReference m_Action;
        [SerializeField] private string m_BindingId;
        [SerializeField] private InputBinding.DisplayStringOptions m_DisplayStringOptions;
        [SerializeField] private TextMeshProUGUI m_ActionLabel;
        [SerializeField] private TextMeshProUGUI m_BindingText;
        [SerializeField] private GameObject m_RebindOverlay;
        [SerializeField] private TextMeshProUGUI m_RebindText;
        [SerializeField] private UpdateBindingUIEvent m_UpdateBindingUIEvent;
        [SerializeField] private InteractiveRebindEvent m_RebindStartEvent;
        [SerializeField] private InteractiveRebindEvent m_RebindStopEvent;

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;
        private static List<RebindActionUI> s_RebindActionUIs;

#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
#endif

        private void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
                m_ActionLabel.text = m_Action?.action?.name ?? string.Empty;
        }

        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string> { }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation> { }
    }
}