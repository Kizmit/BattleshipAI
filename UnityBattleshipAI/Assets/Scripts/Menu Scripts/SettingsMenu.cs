using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;

    public Resolution[] resolutions;

   void Start()
    {
        resolutions = Screen.resolutions; //gets all possible resolutions for user
        resolutionDropdown.ClearOptions(); //clears resolutions out of dropdown
        List<string> resolutionOptions = new List<string>(); //list to hold string of resolutions
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++) //creates resolution strings and stores them in list
        {
            string resolutionOption = resolutions[i].width + " X " + resolutions[i].height + ", " + resolutions[i].refreshRate + "hz";
            resolutionOptions.Add(resolutionOption); 
            if (resolutions[i].width == Screen.currentResolution.width && //finds default screen resolution
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions); //populates resolution dropdown with resolution strings
        resolutionDropdown.value = currentResolutionIndex; //sets default resolution
        resolutionDropdown.RefreshShownValue(); //updates dropdown to start at default resolution
    }

    public void SetResolution (int resolutionIndex) //sets the actual resolution of game
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetMasterVolume (float volume) //attaches volume slider to audio mixer
    {
        audioMixer.SetFloat("masterVolume", volume);
    }
    public void SetMusicVolume (float volume) //attaches volume slider to audio mixer
    {
        audioMixer.SetFloat("musicVolume", volume);
    }
    public void SetEffectsVolume (float volume) //attaches volume slider to audio mixer
    {
        audioMixer.SetFloat("effectsVolume", volume);
    }

    public void SetFullscreen (bool isFullscreen) //fullscreen toggle behavior set
    {
        Screen.fullScreen = isFullscreen;
    }
}
