using System.Collections;
using System.Collections.Generic;
using ServiceLocator.Map;
using ServiceLocator.Player;
using ServiceLocator.Sound;
using ServiceLocator.UI;
using ServiceLocator.Utilities;
using ServiceLocator.Wave;
using UnityEngine;

public class GameService : GenericMonoSingleton<GameService> {

    public PlayerService playerService { get; private set; }
    [Header("Player Service")]
    [SerializeField] private PlayerScriptableObject playerScriptableObject;

    public SoundService soundService { get; private set; }
    [Header("Sound Service")]
    [SerializeField] private SoundScriptableObject soundScriptableObject;
    [SerializeField] private AudioSource audioEffects;
    [SerializeField] private AudioSource backgroundMusic;

    [Header("UI Service")]
    [SerializeField] private UIService UIService;
    public UIService uiService => UIService;

    public MapService mapService { get; private set; }
    [Header("Map Service")]
    [SerializeField] private MapScriptableObject mapScriptableObject;

    public WaveService waveService { get; private set; }
    [Header("Wave Service")]
    [SerializeField] private WaveScriptableObject waveScriptableObject;

    private void Start() {
        playerService = new PlayerService(playerScriptableObject);
        soundService = new SoundService(soundScriptableObject, audioEffects, backgroundMusic);
        mapService = new MapService(mapScriptableObject);
        waveService = new WaveService(waveScriptableObject);
    }

    private void Update() {
        playerService.Update();
    }

}
