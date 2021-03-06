using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using _Core;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Sound
{
   public class SoundManager : Singleton<SoundManager>
   {
      private Dictionary<string, AudioClip> _audioDictionary;
      public string soundEffectPath = "Audio/SoundEffect/";
      public float GlobalVolume { get; set; } = 0.8f;

      public SoundManager()
      {
         GenericPool
            .Instance
            .AddManagedObject<AudioSource>(null,
                                          (source) => {
                                             source.playOnAwake = false;
                                          }
         );

         _audioDictionary = new Dictionary<string, AudioClip>();

         Resources.LoadAll<AudioClip>(soundEffectPath)
                  .ToList()
                  .ForEach(e => {
                     _audioDictionary.Add(e.name, e);
                  }
         );
      }

      /// <summary>
      /// 오디오를 플레이 합니다.
      /// </summary>
      /// <param name="name"></param>
      public void Play(string name)
      {
         if(!_audioDictionary.ContainsKey(name))
         {
            Logger.Log($"Cannot find audio {name}", LogLevel.Error);
            return;
         }

         AudioSource source = GenericPool
                                 .Instance
                                 .Get<AudioSource>(e => !e.isPlaying);
         source.gameObject.SetActive(true);
         source.clip = _audioDictionary[name];
         source.volume = GlobalVolume; // FIXME: TEMP
         source.Play();
      }
   }
}