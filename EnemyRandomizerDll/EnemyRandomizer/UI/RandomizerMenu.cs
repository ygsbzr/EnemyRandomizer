using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using ModType = EnemyRandomizerMod.EnemyRandomizer;
using Object = UnityEngine.Object;
using ModCommon;

namespace EnemyRandomizerMod.Menu
{
    //TODO: refactor menu class so the menu loading is cleaner
    public class RandomizerMenu
    {
        public static RandomizerMenu Instance { get; private set; }

        CommunicationNode comms;
        static UIManager rootUIManager;
        static public MenuScreen optionsMenuScreen;
        static Selectable[] modOptions;
        static Selectable backButton;

        GameObject loadingRoot;
        GameObject loadingButton;
        GameObject loadingBar;
        Text loadingBarText;

        MenuButton enterOptionsMenuButton;
        GameObject menuTogglePrefab;
        GameObject uiManagerCanvasRoot;

        //GameObject scrollBar;
        //GameObject scrollRect;
        //GameObject scrollUIRoot;

        bool loadingButtonPressed;

        public static string MainMenuSceneName {
            get {
                return "Menu_Title";
            }
        }

        public static string OptionsUIManagerName {
            get {
                return "RandoOptionsUIManager";
            }
        }

        public RandomizerMenu()
        {
            rootUIManager = UIManager.instance;
        }

        public void Setup()
        {
            Dev.Where();

            Instance = this;
            comms = new CommunicationNode();
            comms.EnableNode( this );

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;

            SceneLoaded (UnityEngine.SceneManagement.SceneManager.GetActiveScene (), LoadSceneMode.Single);

            Dev.Log( "Menu Loaded!" );
        }

        public void AddLoadingButtonCallback( UnityEngine.Events.UnityAction loadingCallback )
        {
            if ( loadingButton == null )
                return;

            Dev.Where();
            
            Button enableButton = loadingButton.GetComponentInChildren<Button>();
            enableButton.onClick.RemoveListener( loadingCallback );
            enableButton.onClick.AddListener( loadingCallback );
        }

        void SetLoadingBarProgress(float progress)
        {
            if( progress < 1f )
                loadingBar.SetActive( true );
            else
                loadingBar.SetActive( false );

            loadingBarText.text = "Loading Progress: " +(int)(progress * 100.0f)+"%";
        }

        public void Unload()
        {
            GameObject.Destroy( loadingBar );
            GameObject.Destroy( loadingButton );
            GameObject.Destroy( loadingRoot );

            //destroys all menu items (ModOptions)
            GameObject.Destroy( optionsMenuScreen );
            modOptions = null;
            backButton = null;

   
            loadingBar = null;
            loadingButton = null;
            loadingRoot = null;

            rootUIManager = null;

            loadingButtonPressed = false;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneLoaded;

            comms.DisableNode();
            comms = null;
            Instance = null;
        }


        void CreateMainMenuUIElements()
        {
            Modding.Logger.Log("Create Menu");

            //Vector2 buttonPos = new Vector2( -1400f, 400f );
            //Vector2 barPos = new Vector2( -1400f, 200f );

            //Vector2 buttonSize = new Vector2( 400f, 200f );
            //Vector2 barSize = new Vector2( 400f, 40f );

            loadingRoot = new GameObject( "Loading Root" );

            Canvas canvas = loadingRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.GetOrAddComponent<RectTransform>().sizeDelta = new Vector2( 1920f, 1080f );
            CanvasScaler canvasScaler = loadingRoot.AddComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2( 1920f, 1080f );

            //rootUIManager.gameObject.PrintSceneHierarchyTree();

             rootUIManager = UIManager.instance;
            Vector3 pos = rootUIManager.gameObject.FindGameObjectInChildren( "StartGameButton" ).transform.position;

            loadingButton = GameObject.Instantiate(rootUIManager.gameObject.FindGameObjectInChildren( "StartGameButton" ) );
            loadingButton.transform.SetParent( rootUIManager.gameObject.FindGameObjectInChildren( "MainMenuScreen" ).transform );
            loadingButton.gameObject.SetActive( true );
            loadingButton.transform.localScale = Vector3.one;
            loadingButton.transform.position = pos;
            loadingButton.transform.Translate( new Vector3( -5.0f, 4f ) );

            //Dev.Log( "Finished loader ui elements" );

            GameObject.DestroyImmediate( loadingButton.GetComponent<MenuButton>() );
            GameObject.DestroyImmediate( loadingButton.GetComponent<EventTrigger>() );
            GameObject.DestroyImmediate( loadingButton.GetComponentInChildren<AutoLocalizeTextUI>() );

            Text loadingButtonText = loadingButton.GetComponentInChildren<Text>();
            loadingButtonText.text = "[Load Enemy Randomizer]";
            loadingButtonText.color = Color.white;
            loadingButtonText.horizontalOverflow = HorizontalWrapMode.Overflow;
            loadingButtonText.verticalOverflow = VerticalWrapMode.Overflow;        

            Button b = loadingButton.AddComponent<Button>();
            b.targetGraphic = loadingButtonText;
            b.interactable = true;
            ColorBlock cb = new ColorBlock
            {
                highlightedColor = Color.yellow,
                pressedColor = Color.red,
                disabledColor = Color.black,
                normalColor = Color.white,
                colorMultiplier = 2f
            };
            b.colors = cb;
            b.onClick.AddListener( LoadingButtonClicked );

            try {
                AddLoadingButtonCallback (EnemyRandomizerLoader.Instance.BuildEnemyRandomizerDatabase);
            }
            catch (Exception e) {
                Dev.LogError (e.ToString());
            }

            loadingBar = GameObject.Instantiate( rootUIManager.gameObject.FindGameObjectInChildren( "StartGameButton" ) );
            loadingBar.transform.SetParent( rootUIManager.gameObject.FindGameObjectInChildren( "MainMenuScreen" ).transform );
            loadingBar.gameObject.SetActive( true );
            loadingBar.transform.localScale = Vector3.one;
            loadingBar.transform.position = pos;
            loadingBar.transform.Translate( new Vector3( -5.1f, 3.7f ) );

            GameObject.DestroyImmediate( loadingBar.GetComponent<MenuButton>() );
            GameObject.DestroyImmediate( loadingBar.GetComponent<EventTrigger>() );
            GameObject.DestroyImmediate( loadingBar.GetComponentInChildren<AutoLocalizeTextUI>() );

            loadingBarText = loadingBar.GetComponentInChildren<Text>();
            loadingBarText.text = "Loading Progress: ";
            loadingBarText.color = Color.white;
            loadingBarText.horizontalOverflow = HorizontalWrapMode.Overflow;
            loadingBarText.verticalOverflow = VerticalWrapMode.Overflow;
            loadingBar.gameObject.SetActive( false );

            GameObject.DontDestroyOnLoad( loadingRoot );
        }

        void LoadingButtonClicked()
        {
            loadingButtonPressed = true;
            loadingButton.gameObject.SetActive( false );
        }

        Contractor showDatabaseUIWhenReady = new Contractor();

        void ShowRandoDatabaseUI( bool show )
        {
            if( loadingButtonPressed )
                return;

            if( loadingRoot != null )
            {
                loadingRoot.SetActive( show );
            }
            else
            {
                showDatabaseUIWhenReady.OnUpdate = DoShowRandoDatabaseUI;
                showDatabaseUIWhenReady.Looping = true;
                showDatabaseUIWhenReady.Start();
            }
        }

        void DoShowRandoDatabaseUI()
        {
            if( loadingRoot == null )
            {
                CreateMainMenuUIElements();
            }

            if( loadingRoot != null )
            {
                showDatabaseUIWhenReady.Reset();
                loadingRoot.SetActive( true );
            }
        }

        //called by a contractor at the end of the SceneLoaded function
        void ShowOptionsUI()
        {
            uiManagerCanvasRoot.SetActive( true );
        }

        void SceneLoaded( Scene scene, LoadSceneMode lsm )
        {
            bool isTitleScreen = (string.Compare( scene.name, MainMenuSceneName ) == 0);

            if( !isTitleScreen )
            {
               
                return;
            }

            ShowRandoDatabaseUI( isTitleScreen );

            //don't try to "enable" the UI after it's already "enabled"
           
        }

        
        

        //TODO: add params for button text and position
        void AddModMenuButtonToOptionMenu()
        {
            //ADD MODS TO OPTIONS MENU
            MenuButton defButton = (MenuButton)rootUIManager.optionsMenuScreen.defaultHighlight;
            enterOptionsMenuButton = Object.Instantiate( defButton.gameObject ).GetComponent<MenuButton>();

            Navigation nav = enterOptionsMenuButton.navigation;

            nav.selectOnUp = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();
            nav.selectOnDown = defButton.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown();

            enterOptionsMenuButton.navigation = nav;

            nav = enterOptionsMenuButton.FindSelectableOnUp().navigation;
            nav.selectOnDown = enterOptionsMenuButton;
            enterOptionsMenuButton.FindSelectableOnUp().navigation = nav;

            nav = enterOptionsMenuButton.FindSelectableOnDown().navigation;
            nav.selectOnUp = enterOptionsMenuButton;
            enterOptionsMenuButton.FindSelectableOnDown().navigation = nav;

            enterOptionsMenuButton.name = ModType.Instance.GetName();

            enterOptionsMenuButton.transform.SetParent( enterOptionsMenuButton.FindSelectableOnUp().transform.parent );

            enterOptionsMenuButton.transform.localPosition = new Vector2( 0, -240 );
            enterOptionsMenuButton.transform.localScale = enterOptionsMenuButton.FindSelectableOnUp().transform.localScale;

            Object.Destroy( enterOptionsMenuButton.gameObject.GetComponent<AutoLocalizeTextUI>() );
            enterOptionsMenuButton.gameObject.FindGameObjectInChildren( "Text" ).GetComponent<Text>().text = "Enemy Randomizer";
            //ADD MODS TO OPTIONS MENU
        }

        void InstantiateModMenuScreenGameObject()
        {
            if( optionsMenuScreen != null )
                return;

            GameObject go = Object.Instantiate( rootUIManager.optionsMenuScreen.gameObject );
            optionsMenuScreen = go.GetComponent<MenuScreen>();
            optionsMenuScreen.title = optionsMenuScreen.gameObject.FindGameObjectInChildren( "Title" ).GetComponent<CanvasGroup>();
            optionsMenuScreen.topFleur = optionsMenuScreen.gameObject.FindGameObjectInChildren( "TopFleur" ).GetComponent<Animator>();
            optionsMenuScreen.content = optionsMenuScreen.gameObject.FindGameObjectInChildren( "Content" ).GetComponent<CanvasGroup>();
        }

        void SetMenuTitle( string title )
        {
            if( optionsMenuScreen == null )
                return;

            if( optionsMenuScreen.title == null )
                optionsMenuScreen.title = optionsMenuScreen.gameObject.FindGameObjectInChildren( "Title" ).GetComponent<CanvasGroup>();

            optionsMenuScreen.title.gameObject.GetComponent<Text>().text = title;

            //remove the localization component for our custom mod menu
            if( optionsMenuScreen.title.gameObject.GetComponent<AutoLocalizeTextUI>() != null )
                GameObject.Destroy( optionsMenuScreen.title.gameObject.GetComponent<AutoLocalizeTextUI>() );
        }

        void InsertMenuIntoGameHierarchy()
        {
            optionsMenuScreen.transform.SetParent( rootUIManager.optionsMenuScreen.gameObject.transform.parent );
            optionsMenuScreen.transform.localPosition = rootUIManager.optionsMenuScreen.gameObject.transform.localPosition;
            optionsMenuScreen.transform.localScale = rootUIManager.optionsMenuScreen.gameObject.transform.localScale;
        }

        void RemoveGarbageMenuOptions()
        {
            //Log( "Deleting "+  ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            Dev.Log( "Deleting " + optionsMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject.transform.parent.gameObject );

            //GameObject.Destroy( ModMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().FindSelectableOnDown().gameObject.transform.parent.gameObject );
            GameObject.Destroy( optionsMenuScreen.defaultHighlight.FindSelectableOnDown().gameObject.transform.parent.gameObject );
        }

        void SetupMenuTogglePrefab()
        {
            if( menuTogglePrefab != null )
                return;

            //find it
            menuTogglePrefab = GameObject.Instantiate( rootUIManager.gameObject.FindGameObjectInChildren( "VSyncOption" ) );

            //remove default menu behaviors
            GameObject.DestroyImmediate( menuTogglePrefab.GetComponent<MenuOptionHorizontal>() );
            GameObject.DestroyImmediate( menuTogglePrefab.GetComponent<MenuSetting>() );
        }


        void OnCustomSeed( string seed )
        {
            if( seed.Length <= 0 )
                return;

            //EnemyRandomizer.Instance.GlobalSettings.IntValues[ "CustomSeed" ] = Int32.Parse( seed );
            EnemyRandomizer.Instance.OptionsMenuSeed = Int32.Parse( seed );
        }


       

        static Sprite NullSprite(Vector2 size, Color32 c)
        {
            Texture2D tex = new Texture2D( (int)size.x, (int)size.y );
            tex.LoadRawTextureData( new byte[] { c.r, c.g, c.b, c.a } );
            tex.Apply();
            return Sprite.Create( tex, new Rect( 0, 0, size.x, size.y ), Vector2.zero );
        }

        static Sprite NullSprite()
        {
            Texture2D tex = new Texture2D( 1, 1 );
            tex.LoadRawTextureData( new byte[] { 0xFF, 0xFF, 0xFF, 0xFF } );
            tex.Apply();
            return Sprite.Create( tex, new Rect( 0, 0, 1, 1 ), Vector2.zero );
        }

        static Sprite CreateSprite( byte[] data, int x, int y, int w, int h )
        {
            Texture2D tex = new Texture2D( 1, 1 );
            tex.LoadImage( data );
            tex.anisoLevel = 0;
            return Sprite.Create( tex, new Rect( x, y, w, h ), Vector2.zero );
        }

        static void DataDump( GameObject go, int depth )
        {
            Dev.Log( new string( '-', depth ) + go.name );
            foreach( Component comp in go.GetComponents<Component>() )
            {
                switch( comp.GetType().ToString() )
                {
                    case "UnityEngine.RectTransform":
                    Dev.Log( new string( '+', depth ) + comp.GetType() + " : " + ((RectTransform)comp).sizeDelta + ", " + ((RectTransform)comp).anchoredPosition + ", " + ((RectTransform)comp).anchorMin + ", " + ((RectTransform)comp).anchorMax );
                    break;
                    case "UnityEngine.UI.Text":
                    Dev.Log( new string( '+', depth ) + comp.GetType() + " : " + ((Text)comp).text );
                    break;
                    default:
                    Dev.Log( new string( '+', depth ) + comp.GetType() );
                    break;
                }
            }
            foreach( Transform child in go.transform )
            {
                DataDump( child.gameObject, depth + 1 );
            }
        }

        [CommunicationCallback]
        public void HandleLoadingProgressEvent( LoadingProgressEvent e, object randoDatabase )
        {
            SetLoadingBarProgress( e.progress );
        }
    }
}
