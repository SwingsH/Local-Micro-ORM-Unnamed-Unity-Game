using UnityEngine;
using UniRx;
using TIZSoft.Utils.Log;
using System;
using TIZSoft;
using TIZSoft.UnityHTTP.Client;
using TIZSoft.Versioning;
using TIZSoft.Services;
using TIZSoft.UnknownGame;

public class ClientMain : MonoBehaviorSingleton<ClientMain>
{
    public enum State
    {
        Uninitialized = 0,
        Initializing = 1,
        StreamingInitialized = 2,
        Initialized = 3,

        //Debug Info
        FetchServerMaintenanceAsync = 10,
        FetchServerListAsync = 11,
        DownloadPatchAsync = 12,
        LoadDataAsync = 13,
        DownloadGameDataAsync = 14,
    }

    // Net
    public ClientHttpManager HttpManager { get; private set; }
    public ClientHostConfigure HostConfigure { get; private set; }
    public ClientHttpSender HttpSender { get; private set; }
    public GameApiServices ApiService { get; private set; }

    // Game Logic
    public Game Game { get; private set; }

    static bool isApplicationQuit;
    readonly ReactiveProperty<State> initializationState = new ReactiveProperty<State>();
    static readonly TIZSoft.Utils.Log.Logger logger = LogManager.Default.FindOrCreateCurrentTypeLogger();
    public VersionManager VersionManager { get; private set; }

    [SerializeField]
    [Tooltip("Log level.")]
    LogLevel logLevel = LogLevel.Debug;

    [SerializeField]
    [Tooltip("網路設定")]
    ClientHostConfigure.Settings hostSettings;

    void Start()
    {
        if (isApplicationQuit)
        {
            return;
        }

        if (Instance != this)
        {
            return;
        }

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Awake()
    {
        base.Awake();

        if (isApplicationQuit)
        {
            return;
        }

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            return;
        }

        if (_instance == this)
        {
            return;
        }

        logger.Warn(this, "Found a duplicated {0} object. Destroy it. Name={1}", typeof(ClientMain).FullName, name);
        Destroy(gameObject);
    }

    //foolproof check
    private void OnValidate()
    {
        //if (assetBundleManager == null)
        //{
        //    logger.Error("Field \"assetBundleManager\" is null.");
        //}
    }

    void Initialize()
    {
        if (initializationState.Value > State.Uninitialized)
        {
            return;
        }

        initializationState.Value = State.Initializing;

        foreach (LogLevel v in Enum.GetValues(typeof(LogLevel)))
        {
            LogManager.Default.SetEnabled(v, v >= logLevel);
        }

        Resolve();

        //var loadStreamingDataAsync = Observable.FromMicroCoroutine(LoadStreamingDataAsync);
        //var fetchServerMaintenanceAsync = Observable.FromMicroCoroutine(FetchServerMaintenanceAsync);
        //var fetchServerListAsync = Observable.FromMicroCoroutine(FetchServerListAsync);
        //var downloadPatchAsync = Observable.FromMicroCoroutine(DownloadPatchAsync);
        //var loadDataAsync = Observable.FromCoroutine(LoadDataAsync);

        //// Sequential execute.
        //var allOperations = loadStreamingDataAsync.Concat(
        //    fetchServerMaintenanceAsync,
        //    fetchServerListAsync,
        //    downloadPatchAsync,
        //    loadDataAsync);

        //allOperations.
        //    DoOnCompleted(() => initializationState.SetValueAndForceNotify(State.Initialized)).
        //    Subscribe();

        //uiClickEffect = GameObject.FindObjectOfType<UIClickEffect>();
    }

    void Resolve()
    {
        Game = new Game();

        // Networks
        HttpManager     = new ClientHttpManager();
        HostConfigure   = new ClientHostConfigure(hostSettings);
        HttpSender      = new ClientHttpSender(HttpManager, HostConfigure);
        ApiService      = new GameApiServices(HttpSender, Constants.APPLICATION_PROJECT_NAME, Game.Network.Value);

        //VersionManager = new VersionManager(versionSettings);

        //ResourceUrlProvider = new ResourceUrlProvider(resourceUrlProviderSettings, network, () => Environment);

        //// Asset Management
        //StreamingAssetManager = new AssetManager(streamingAssetManagerSettings, streamingAssetBundleManager);
        //StreamingAssetProvider = new AssetProvider(streamingAssetProviderSettings, StreamingAssetManager);
        //AssetManager = new AssetManager(assetManagerSettings, assetBundleManager);
        //AssetProvider = new AssetProvider(assetProviderSettings, AssetManager, StreamingAssetProvider);

        //SceneManager = new SceneManager(assetManagerSettings.ScenePath, assetBundleManager);

        //PersistentDataStorage = new PersistentDataStorage();
        //SaveDataStorage = new SaveDataStorage(PersistentDataStorage);
        //Save = new Save(SaveSettings, SaveDataStorage);

        //HttpManager.RequestHeadersCreator = new HttpHeaderCreator(Save, Game.LocalUser.Value).CreateHeaders;

        //Game.Network.Value.CurrentGroupId.Value = HostConfigure.CurrentHostId;
        //Game.Network.Value.SetDefaultCurrentServer(ServerType.Router, HostConfigure.CurrentHost);

        //Common.Dialog.DefaultCanvas = defaultUiCanvas;

        //AssetProvider.Initialize();
        //StreamingAssetProvider.Initialize();

        //PrefabLoader.AddAssetProvider(AssetProvider);
        //PrefabLoader.AddAssetProvider(StreamingAssetProvider);

        //// Game data
        //AudioDataRepository = new AudioDataRepository();
        //I18nDataRepository = new I18nDataRepository(I18nLanguage.TraditionalChinese);
        //ScriptDataRepository = new ScriptDataRepository();
        //CGDataRepository = new CGDataRepository();
        //SkillLevelDataRepository = new SkillLevelDataRepository();
        //SheetMusicDataRepository = new SheetMusicDataRepository();
        //ItemDataRepository = new ItemDataRepository();
        //MapDataRepository = new MapDataRepository();
        //CharacterDataRepository = new CharacterDataRepository(I18nDataRepository);
        //QuestDataRepository = new QuestDataRepository();
        //SMSDataRepository = new SMSDataRepository();
        //TerminologyDataRepository = new TerminologyDataRepository(I18nDataRepository);
        //CombineDataRepository = new CombineDataRepository();
        //NiaInfoDataRepository = new NiaInfoDataRepository();
        //ConstIntDataRepository = new ConstIntDataRepository();
        //ActivityDataRepository = new ActivityDataRepository();
        //ActRewardDataRepository = new ActRewardDataRepository();
        //ShopExchangeDataRepository = new ShopExchangeDataRepository();
        //InspirationDataRepository = new InspirationDataRepository(ConstIntDataRepository, ItemDataRepository);
        //RoleGenerator = new Role.RoleGenerator(Game, AssetProvider, ItemDataRepository);
        //RechargeDataRepository = new RechargeDataRepository();
        //RechargePackageDataRepository = new RechargePackageDataRepository();
        //RewardDataRepository = new RewardDataRepository();
        //SignInDataRepository = new SignInDataRepository();
        //GuideDataRepository = new GuideDataRepository();

        //SoundManager.Initialize(AssetProvider, StreamingAssetProvider, Save.GameSave.CurrentGroup.SystemSave);
        //BgmManager = new BgmManager(SoundManager, AudioDataRepository.SceneBgmDataMap, AudioDataRepository.BgmDataMap);
        //SfxManager = new SfxManager(SoundManager, AudioDataRepository.SfxDataMap);

        //GuideManager = new GuideManager(Game.LocalUser, GuideDataRepository, AssetProvider);

        //MessagingManager = new MessagingManager();
    }
}

