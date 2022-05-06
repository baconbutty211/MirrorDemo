using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Save players preferred username (playerPrefs...)
/// </summary>
public class PlayerUsername : MonoBehaviour
{
    public static event System.Action<string> OnSubmitUsername;

    [Header("Username Objects")]
    public TMP_InputField usernameInput;

    public void SubmitUsername()
    {
        //if (! hasAuthority)
        //    return;

        string username = usernameInput.text;
        if (!string.IsNullOrWhiteSpace(username))
        {
            OnSubmitUsername?.Invoke(username);
        }
    }
}
