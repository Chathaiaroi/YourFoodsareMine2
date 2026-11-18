using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;

    private Renderer rend;
    private bool unlocked;

    public bool IsUnlocked => unlocked;

    // Observer: Level สมัครรับ event นี้
    public event Action OnCatReached;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        GetComponent<Collider>().isTrigger = true;
        Lock();
    }

    public void Lock()
    {
        unlocked = false;
        SetColor(lockedColor);
    }

    public void Unlock()
    {
        unlocked = true;
        SetColor(unlockedColor);
        Debug.Log("Checkpoint unlocked!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        if (unlocked)
        {
            OnCatReached?.Invoke();
        }
        else
        {
            Debug.Log("Don't go yet! Im still hungry!");
        }
    }

    private void SetColor(Color color)
    {
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

}
