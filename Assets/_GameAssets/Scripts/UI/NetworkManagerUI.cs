using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
   [SerializeField] private Button _starthostbutton;
   [SerializeField] private Button _startclientbutton;


   private void Awake()
   {
      _starthostbutton.onClick.AddListener(() =>
      {
          NetworkManager.Singleton.StartHost();
          Hide();
      });
      _startclientbutton.onClick.AddListener(() =>
      {
          NetworkManager.Singleton.StartClient();
          Hide();
      });
   }
   private void Hide()
   {
       gameObject.SetActive(false);
   }
}
