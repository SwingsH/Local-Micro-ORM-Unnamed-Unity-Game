using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace TIZSoft.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleCallFunction : MonoBehaviour
    {
        public UnityEvent toogleOnEvent;
        public UnityEvent toogleOffEvent;
        Toggle _cachedToggle;

        Toggle cachedToggle
        {
            get
            {
                if (!_cachedToggle)
                    _cachedToggle = gameObject.GetComponent<Toggle>();
                return _cachedToggle;
            }
        }

        private void Awake()
        {
            cachedToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            cachedToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void Reset()
        {
            cachedToggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            cachedToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void OnToggleValueChanged(bool value)
        {
            if(value)
            {
                toogleOnEvent.Invoke();
            }
            else
            {
                toogleOffEvent.Invoke();
            }
        }
    }
}
