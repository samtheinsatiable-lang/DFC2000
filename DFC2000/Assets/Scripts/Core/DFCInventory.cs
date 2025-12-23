using UnityEngine;
using System.Collections.Generic;

namespace DFC2000.Core
{
    public class DFCInventory : MonoBehaviour
    {
        [Header("Storage")]
        [SerializeField] private List<string> items = new List<string>();

        public List<string> Items => items;

        public void AddItem(string item)
        {
            items.Add(item);
            Debug.Log($"<color=cyan>INVENTORY >> ADDED: {item}</color>");
        }

        public bool HasItem(string item)
        {
            return items.Contains(item);
        }

        public void RemoveItem(string item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                Debug.Log($"<color=cyan>INVENTORY >> REMOVED: {item}</color>");
            }
        }
    }
}
