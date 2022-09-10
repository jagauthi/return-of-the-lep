using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour {
    
    bool showMenu, mainMenuToggle;
    int buffer;
    int buttonLength;
    int textLength;
    int currentStage, numEnemies;
    PlayerScript playerScript;
    MouseLook playerMouseLook, cameraMouseLook;
    AudioControlScript audioControl;
    GameObject player;
    GameRestClient restClient;
    ItemHandler itemHandler;

    Rect mainGroupRect, levelGroupRect, inventoryGroupRect;
    Rect pointsRect, backgroundRect, strengthRect, strengthTextRect;
    Rect agilityRect, agilityTextRect, intelligenceRect, intelligenceTextRect, quitRect, goldRect;

    Rect screenRect;
    Rect startRect;
    Rect quit2Rect;

    List<String> scenes; 
    
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        Physics.gravity = new Vector3(0, -22.0f, 0);
        restClient = new GameRestClient(this);
        itemHandler = new ItemHandler(this);
        getItems();

        GameObject tryPlayer = GameObject.FindGameObjectWithTag("Player");
        if (tryPlayer == null) {
            createPlayer();
        }
        else {
            player = tryPlayer;
        }
        playerScript = player.GetComponent<PlayerScript>();
        playerMouseLook = player.GetComponent<MouseLook>();
        cameraMouseLook = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MouseLook>();
        audioControl = GetComponentInChildren<AudioControlScript>();
        showMenu = false;
        mainMenuToggle = false;
        buffer = Screen.height / 16;
        buttonLength = Screen.height / 16;
        textLength = Screen.height / 8;
        currentStage = 1;
        scenes = new List<String>();
        scenes.Add("CharacterCreation");
        scenes.Add("NewIntro");
        scenes.Add("Stage1");
        scenes.Add("Stage2");
        scenes.Add("Stage3");
        scenes.Add("FinalStage");
        initRects(); 
    }

    void initRects() {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        startRect = new Rect(Screen.width/8, Screen.height/8, Screen.width / 4, Screen.height / 8);
        quit2Rect = new Rect(Screen.width / 8, Screen.height / 2, Screen.width / 4, Screen.height / 8);

        pointsRect = new Rect(buffer, 0, textLength, buttonLength);
        mainGroupRect = new Rect(Screen.width / 8, Screen.height / 6, 3 * Screen.width / 4, 2 * Screen.height / 3);
        levelGroupRect = new Rect(Screen.width / 6, Screen.height / 4, Screen.width / 2, 5 * Screen.height / 8);   
        inventoryGroupRect = new Rect(11 * Screen.width / 16, Screen.height / 8, Screen.width / 4, 3 * Screen.height / 4);
        backgroundRect = new Rect(0, 0, 3 * Screen.width / 4, 3 * Screen.height / 4);

        //Str
        strengthTextRect = new Rect(buffer, Screen.height / 8, Screen.height / 4, textLength);
        strengthRect = new Rect(buffer + textLength + buffer, Screen.height / 8, buttonLength, buttonLength);
        //Agil
        agilityTextRect = new Rect(buffer, Screen.height / 8 * 2, Screen.height / 4, textLength);
        agilityRect = new Rect(buffer + textLength + buffer, Screen.height / 8 * 2, buttonLength, buttonLength);
        //Intel
        intelligenceTextRect = new Rect(buffer, Screen.height / 8 * 3, Screen.height / 4, textLength);
        intelligenceRect = new Rect(buffer + textLength + buffer, Screen.height / 8 * 3, buttonLength, buttonLength);

        quitRect = new Rect(Screen.width/2-buttonLength, 0, buttonLength, buttonLength);
        goldRect = new Rect(inventoryGroupRect.width/16, inventoryGroupRect.height/16, inventoryGroupRect.width/4, inventoryGroupRect.height/16);
    }

    void createPlayer()
    {
        GameObject playerObject = (GameObject)Resources.Load("Prefabs/Mage");
        Transform player = playerObject.GetComponent<Transform>();
        this.player = Instantiate(player).gameObject;
        //Instantiate(enemy, player.transform.position + new Vector3(2, 0, 2), player.transform.rotation);
    }

    void Update()
    {
        if(numEnemies == 0 && currentStage > 1)
        {
            incrementStage();
        }
        checkInput();
        if(showMenu)
        {
            pauseGame();
        }
        else
        {
            unpauseGame();
        }
    }

    public void setMenuOpen(bool val) {
        showMenu = val;
    }

    public bool getMenuOpen() {
        return showMenu;
    }

    public void pauseGame() {
        showMenu = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        mouseLookIsEnabled(false);
    }

    public void unpauseGame() {
        showMenu = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        mouseLookIsEnabled(true);
    }

    void checkInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            showMenu = !showMenu;
            if(showMenu){
                mainMenuToggle = true;
            }
            else {
                mainMenuToggle = false;
            }
        }
    }

    void OnGUI()
    {
        int width = 100;
        int height = 30;
        if (player != null)
        {
            if (mainMenuToggle)
            {
                mainMenu();
            }
            if (playerScript.getLevelupToggle())
            {
                levelUpMenu();
            }

            if (playerScript.getInventory().isOpen()) {
                inventoryMenu();
            }
        }
        else if(currentStage > 1 && playerScript.getCurrentHealth() > 0)
        {
            if (numEnemies == 0)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height), "Next Level"))
                {
                    currentStage++;
                    switchStage(currentStage);
                }
            }
        }
    }

    public void mouseLookIsEnabled(bool val)
    {
        playerMouseLook.isEnabled(val);
        cameraMouseLook.isEnabled(val);
    }

    void mainMenu()
    {
        GUI.Box(screenRect, "");
        GUI.BeginGroup(mainGroupRect);

        GUI.Box(backgroundRect, "");
        if (GUI.Button(startRect, "Quit to Central Hub"))
        {
            saveAndQuit();
        }
        if (GUI.Button(quit2Rect, "Quit the Whole Game"))
        {
            Application.Quit();
        }
        GUI.EndGroup();
    }

    void levelUpMenu()
    {
        GUI.BeginGroup(levelGroupRect);
        GUI.Box(backgroundRect, "");
        //GUI.DrawTexture(backgroundRect, backgroundTexture);

        GUI.Label(pointsRect, "Points: " + playerScript.getSkillPoints());
        GUI.Label(strengthTextRect, "Strength: " + playerScript.getStrength());
        GUI.Label(agilityTextRect, "Agility: " + playerScript.getAgility());
        GUI.Label(intelligenceTextRect, "Intelligence: " + playerScript.getIntelligence());
        if (playerScript.getSkillPoints() > 0) {
            if (GUI.Button(strengthRect, "+"))
            {
                playerScript.setStrength(playerScript.getStrength()+1);
                playerScript.setSkillPoints(playerScript.getSkillPoints()-1);
            }
            if (GUI.Button(agilityRect, "+"))
            {
                playerScript.setAgility(playerScript.getAgility()+1);
                playerScript.setSkillPoints(playerScript.getSkillPoints()-1);
            }
            if (GUI.Button(intelligenceRect, "+"))
            {
                playerScript.setIntelligence(playerScript.getIntelligence()+1);
                playerScript.setSkillPoints(playerScript.getSkillPoints()-1);
            }
        }

        int armorButtonLength = (int)(3*levelGroupRect.height / 16);
        float armorSlotCenterX = 5 * levelGroupRect.width / 8;

        Hashtable playerEquipment = playerScript.getEquipment().getItemMap();

        //Head slot
        Armor playerHead = (Armor)playerEquipment["Head"];
        Texture2D helmTexture = (Texture2D)Resources.Load("Images/HeadSlot");
        Rect headSlot = new Rect(armorSlotCenterX, levelGroupRect.height/8, armorButtonLength, armorButtonLength);
        this.drawArmorSlot(playerHead, helmTexture, headSlot);

        //Chest slot
        Armor playerChest = (Armor)playerEquipment["Chest"];
        Texture2D chestTexture = (Texture2D)Resources.Load("Images/ChestSlot");
        Rect chestSlot = new Rect(armorSlotCenterX, levelGroupRect.height/8 + armorButtonLength + buffer, armorButtonLength, armorButtonLength);
        this.drawArmorSlot(playerChest, chestTexture, chestSlot);

        //Legs slot
        Texture2D legsTexture = (Texture2D)Resources.Load("Images/LegsSlot");
        if (null != playerEquipment["Legs"])
        {
            legsTexture = ((Item)playerEquipment["Legs"]).getIcon();
        }
        Rect legsSlot = new Rect(armorSlotCenterX, levelGroupRect.height / 8 + (2*(armorButtonLength + buffer)), armorButtonLength, armorButtonLength);
        GUI.DrawTexture(legsSlot, legsTexture);

        //Feet slot
        Texture2D bootsTexture = (Texture2D)Resources.Load("Images/BootsSlot");
        if (null != playerEquipment["Feet"])
        {
            bootsTexture = ((Item)playerEquipment["Feet"]).getIcon();
        }
        Rect bootsSlot = new Rect(armorSlotCenterX + armorButtonLength + buffer, levelGroupRect.height / 8 + (2 * (armorButtonLength + buffer)), armorButtonLength, armorButtonLength);
        GUI.DrawTexture(bootsSlot, bootsTexture);


        if (GUI.Button(quitRect, "X"))
        {
            playerScript.levelUpToggle();
        }
        GUI.EndGroup();
    }

    void drawArmorSlot(Armor armor, Texture2D helmTexture, Rect armorSlot)
    {
        //Draw armor texture
        if (null != armor)
        {
            helmTexture = armor.getIcon();
        }
        GUI.DrawTexture(armorSlot, helmTexture);
        
        //cursor tooltip
        if (null != armor && armorSlot.Contains(Event.current.mousePosition))
        {
            Rect mouseTextRect = new Rect(
                Input.mousePosition.x - levelGroupRect.x + (buffer / 2),
                Screen.height - Input.mousePosition.y - levelGroupRect.y,
                4 * buffer / 3, buffer / 2);
            GUI.Box(mouseTextRect, "Armor: " + armor.getArmorPower());
        }
    }

    void inventoryMenu()
    {
        int buttonLength = (int)(inventoryGroupRect.width / 4);
        int buffer = (int)inventoryGroupRect.width/16;
        GUI.BeginGroup(inventoryGroupRect);
        GUI.Box(backgroundRect, "");
        GUI.Box(goldRect, playerScript.getGold() + " gp");
        //GUI.DrawTexture(backgroundRect, backgroundTexture);
        for( int col = 0; col < 3; col++ ) {
            for( int row = 0; row < 3; row++ ) {
                int slotNum =  ( col * 3 ) + row;
                if(playerScript.getInventory().getSize() > slotNum) {
                    Item item = playerScript.getInventory().getItems()[slotNum];
                    Rect slot = new Rect(buffer*(row+1) + buttonLength*row, 
                        buffer*(col+1) + buttonLength*(col+1), 
                        buttonLength, buttonLength);

                    GUI.DrawTexture( slot, item.getIcon() );

                    if (GUI.Button(slot, ""+slotNum)) {
                        playerScript.useItem(item);
                    }

                }
                else {
                    Rect slot = new Rect(buffer*(row+1) + buttonLength*row, 
                        buffer*(col+1) + buttonLength*(col+1), 
                        buttonLength, buttonLength);

                    if (GUI.Button(slot, ""+slotNum)) {
                        
                    }
                }
            }
        }
        GUI.EndGroup();
    }

    public void saveAndQuit()
    {
        //TODO: Save the game somehow :P
        numEnemies = 0;
        SceneManager.LoadScene(scenes[1], LoadSceneMode.Single);
        player.transform.position = new Vector3(2.6f, 1.5f, -5.1f);
        //Destroy(player.gameObject);
        Destroy(this.gameObject);
    }

    public void incrementStage()
    {
        currentStage++;
        switchStage(currentStage);
    }

    public void switchStage(int stageNum)
    {
        if (stageNum < scenes.Count)
        {
            SceneManager.LoadScene(scenes[stageNum], LoadSceneMode.Single);
            player.transform.position = new Vector3(0.0f, 0.5f, -5.1f);
            if (stageNum > 1)
            {
                numEnemies = getNumEnemiesForStage(stageNum-1);
            }
            else if(stageNum == 1)
            {
                numEnemies = 0;
            }
            else if(stageNum == 0)
            {
                numEnemies = 0;
                Destroy(player);
                Destroy(this.gameObject);
            }
        }
    }

    int getNumEnemiesForStage(int stageNum)
    {
        return stageNum + (stageNum - 1);
    }
        
    void generateEnemies(int number)
    {
        GameObject holder = (GameObject)Resources.Load("Prefabs/Enemy");
        Rigidbody enemy = holder.GetComponent<Rigidbody>();
        holder = (GameObject)Resources.Load("Prefabs/Player");
        Rigidbody player = holder.GetComponent<Rigidbody>();
        for (int i = 0; i < number; i++)
        {
            //Rigidbody bulletClone = Instantiate(enemy, player.transform.position + new Vector3(2, 0, 2), player.transform.rotation);
            Instantiate(enemy, player.transform.position + new Vector3(2, 0, 2), player.transform.rotation);
            numEnemies++;
        }
    }

    public void enemyDied() {
        numEnemies--;
    }

    public GameRestClient getRestClient() {
        return restClient;
    }

    public void getItems() {
        IEnumerator getter = restClient.getItems();
        StartCoroutine(getter);
    }

    public void setItems(String items) {
        itemHandler.parseItems(items);
        List<Item> allItems = itemHandler.getAllItems();
        print(allItems);
    }
}
