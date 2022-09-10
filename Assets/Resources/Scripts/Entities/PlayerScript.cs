using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    protected GameScript gameScript;
    protected Transform selectedTarget;
    protected GameObject player, myCamera, gameEngine;
    protected Rigidbody rigidbody;
    protected Texture2D selectedTexture;
    protected Ability selectedAbility;

    protected float jumpSpeed, nextTime;
    protected int damage, speed, interval, running;
    protected int currentHealth, strength, intelligence, agility;
    
    protected float regularBarLength = Screen.width / 3;
    protected float healthBarLength = Screen.width / 3;
    protected float expBarLength = Screen.width / 3;
    protected int level = 1;
    protected int exp = 0;
    protected int range = 10;
    protected int skillPoints = 5;
    protected int gold;
    protected bool levelUpMenuToggle = false;

    protected List<Ability> abilities;
    protected List<Quest> activeQuests;
    protected Inventory inventory;
    protected Equipment equipment;
    protected Animator weaponAnimator;
    protected Weapon[] weapons;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    protected void Start ()
    {
        //Cursor.visible = false;

        basicInits();
        loadAbilities();
        initStats();
        initAnimations();
    }

    protected void basicInits()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        myCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player.GetComponent<Renderer>().material.color = Color.black;
        selectedTexture = (Texture2D)Resources.Load("Images/SelectedIcon");
        rigidbody = GetComponent<Rigidbody>();
        abilities = new List<Ability>();
        activeQuests = new List<Quest>();
        inventory = new Inventory();
        equipment = new Equipment();
        inventory.addItem(new Consumable(0, "Health Potion", "Heal", (Texture2D)Resources.Load("Images/HealthPotion"), 50));
        gold = 200;
    }

    public GameScript retrieveGameScript() {
        if(gameScript == null) {
            getGameScript();
        }
        return gameScript;
    }

    //NOT THE ONE THAT RETRIEVES THE GAMESCRIPT ^^
    protected void getGameScript() {
        gameEngine = GameObject.FindGameObjectWithTag("GameEngine");
        if(gameEngine != null) {
            gameScript = gameEngine.GetComponent<GameScript>();
        }
    }

    protected void initStats()
    {
        damage = 10;
        speed = 10;
        jumpSpeed = 5f;
        currentHealth = 100;
        strength = 1;
        intelligence = 1;
        agility = 1;
        nextTime = 0;
        interval = 1;
    }

    protected void initAnimations() {
        weaponAnimator = GetComponentInChildren<Animator>();   
    }

    protected void loadAbilities()
    {
        abilities.Add( new Ability( "Bullet", "RangedProjectile", 0, 30,
                ( (GameObject)Resources.Load("Prefabs/Bullet") ),
                (Texture2D)Resources.Load("Images/BulletIcon") ) );
        selectedAbility = abilities[0];
    }

    protected void Update() {
       basicUpdates();
    }

    protected void basicUpdates() {
        if(rigidbody.velocity.y < -22) {
            rigidbody.velocity = rigidbody.velocity + new Vector3(0, 0.1f, 0);
        }
        if(gameScript == null) {
            getGameScript();
        }
        checkInput();
        if (Time.time >= nextTime)
        {
            oncePerSecondUpdate();
            nextTime += interval;
        }
        healthBarLength = (regularBarLength) * (currentHealth / (float)getMaxHealth());
        expBarLength = (regularBarLength) * (exp / (float)GetExpToNextLevel());
    }

    protected void oncePerSecondUpdate()
    {
        gainResource(getMaxResource()/25);
    }

    protected void checkInput()
    {
        basicInputs();
        extraInputs();
    }

    protected void basicInputs()
    {
        if (Input.GetKey("w"))
        {
            transform.Translate(Vector3.forward * (speed) * Time.deltaTime);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.back * (speed) * Time.deltaTime);
        }
        if (Input.GetKey("a"))
        {
            transform.Translate(Vector3.left * (speed) * Time.deltaTime);
        }
        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * (speed) * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jump();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            closeMenus();
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            switchAbilities(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            switchAbilities(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            switchAbilities(2);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            switchAbilities(3);
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            switchAbilities(4);
        }
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Vector3 forward = transform.forward + GameObject.FindGameObjectWithTag("MainCamera").transform.forward;
            if (Physics.Raycast(transform.position, forward, out hit))
            {
                DeselectTarget();
                SelectTarget(hit.transform);
            }
        }
        if(Input.GetKeyDown("i")) {
            inventoryToggle();
        }
        if(Input.GetKeyDown("c")) {
            levelUpToggle();
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            speed += 3;
        }
    }

    public void inventoryToggle() {
        if(!gameScript.getMenuOpen()) {
            gameScript.setMenuOpen(true);
            inventory.toggle();
        }
        else if(levelUpMenuToggle) {
            inventory.toggle();
        }
        else if(inventory.isOpen()) {
            gameScript.setMenuOpen(false);
            inventory.toggle();
        }
    }

    public void levelUpToggle() {
        if(!gameScript.getMenuOpen()) {
            gameScript.setMenuOpen(true);
            levelUpMenuToggle = true;
        }
        else if(inventory.isOpen()) {
            levelUpMenuToggle = !levelUpMenuToggle;
        }
        else if(levelUpMenuToggle) {
            gameScript.setMenuOpen(false);
            levelUpMenuToggle = false;
        }
    }

    protected void closeMenus() {
        levelUpMenuToggle = false;
        inventory.close();
    }

    protected void switchAbilities(int abilityNumber) {
        selectedAbility = abilities[abilityNumber];
        if(abilities[abilityNumber].getName() == "Weapon") {
            GetComponentInChildren<WeaponScript>().unSheathe();
        }
        else {
            GetComponentInChildren<WeaponScript>().sheathe();
        }
    }

    protected virtual void extraInputs() {
        
    }

    protected void jump() {
        if(rigidbody.velocity.y < 5 && transform.position.y < 100) {
            rigidbody.velocity += (Vector3.up * jumpSpeed);
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    void OnTriggerEnter(Collider collision)
    {
        ProjectileScript bulletScript = (BulletScript)collision.gameObject.GetComponent("BulletScript");
        if(bulletScript == null) {
            bulletScript = (TargetedProjectileScript)collision.gameObject.GetComponent("TargetedProjectileScript");
        }
        if (bulletScript != null && bulletScript.getShooter() != this.gameObject)
        {
            print("Losing health! " + bulletScript.getDamage());
            loseHealth(bulletScript.getDamage());
            Destroy(collision.gameObject);
        }
    }

    void OnDestroy()
    {

    }

    protected void Attack()
    {
        Rigidbody bulletClone = Instantiate(selectedAbility.getBody(), 
                transform.position + new Vector3(0.0f, .5f, 0.0f), transform.rotation);
        BulletScript bulletScript = (BulletScript)bulletClone.gameObject.GetComponent("BulletScript");
        bulletScript.setShooter(this.gameObject);
        bulletClone.velocity = myCamera.transform.forward * bulletScript.bulletSpeed;
    }

    protected void OnGUI(){
        drawBasics();        
    }

    protected void drawBasics() {
        drawCrosshair();
        drawHealthBar();
        drawExpBar();
        drawAbilities();
        if (selectedTarget != null && selectedTarget.gameObject != null)
        {
            EnemyScript enemyScript = selectedTarget.gameObject.GetComponent<Collider>().GetComponent<EnemyScript>();
            drawEnemyHealthBar(enemyScript);
        }

        if (currentHealth <= 0)
        {
            int width = 100;
            int height = 50;
            if (GUI.Button(new Rect(regularBarLength - width / 2, Screen.height / 2 - height / 2, width, height), "Back to Menu"))
            {
                gameEngine.GetComponent<GameScript>().switchStage(0);
            }
        }
    }

    protected void drawCrosshair()
    {
        GUIStyle redStyle = new GUIStyle(GUI.skin.box);
        redStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 0.75f));
        GUI.Label(new Rect(Screen.width / 2 - 10, Screen.height / 2 - 10, 20, 20), "X");
    }

    protected void drawHealthBar()
    {
        GUIStyle redStyle = new GUIStyle(GUI.skin.box);
        redStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 0.75f));
        GUIStyle blackStyle = new GUIStyle(GUI.skin.box);
        blackStyle.normal.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 1f));
        if (healthBarLength > 0)
        {
            GUI.Box(new Rect(10, 10, healthBarLength, 20), "", redStyle);
        }
        GUI.Box(new Rect(10, 10, regularBarLength, 20), currentHealth + "/" + getMaxHealth());
        GUI.Box(new Rect(regularBarLength + 20, 10, 20, 20), "" + level);
    }

    protected void drawExpBar()
    {
        GUIStyle greenStyle = new GUIStyle(GUI.skin.box);
        greenStyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 0.75f));
        if (expBarLength > 0)
        {
            GUI.Box(new Rect(10, 35, expBarLength, 20), "", greenStyle);
        }
        GUI.Box(new Rect(10, 35, regularBarLength, 20), exp + "/" + GetExpToNextLevel());
        if(skillPoints > 0)
        {
            GUI.Box(new Rect(regularBarLength + 20, 35, 20, 20), "+");
        }
    }

    protected void drawAbilities()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if(abilities[i] == selectedAbility) { 
                GUI.DrawTexture(
                    new Rect((i * Screen.width / 16)+ 15, 7 * Screen.height / 8 - 25, Screen.height / 8 + 10, Screen.height / 8 + 10),
                    selectedTexture
                );
            }
            GUI.DrawTexture(
                new Rect((i * Screen.width / 16) + 20, 7 * Screen.height / 8- 20, Screen.height / 8, Screen.height / 8),
                abilities[i].getIcon()
            );
        }
    }

    protected void drawEnemyHealthBar(EnemyScript enemyScript)
    {
        if (enemyScript != null)
        {
            GUIStyle redStyle = new GUIStyle(GUI.skin.box);
            redStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f, 0.75f));
            float healthBarLength = (regularBarLength) * (enemyScript.currentHealth / (float)enemyScript.maxHealth);
            if (healthBarLength > 0)
            {
                GUI.Box(new Rect(regularBarLength + 50, 10, healthBarLength, 20), "", redStyle);
            }
            GUI.Box(new Rect(regularBarLength + 50, 10, regularBarLength, 20), enemyScript.currentHealth + "/" + enemyScript.maxHealth);
        }
    }
    
    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    protected Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    protected void SelectTarget(Transform transform) {
        if(transform.gameObject.tag != "Untagged") {
            selectedTarget = transform;
        }
        if(selectedTarget != null) {
            float distance = Vector3.Distance(selectedTarget.position, player.transform.position);
            if (selectedTarget.transform.CompareTag("Enemy")) {
                selectedTarget.GetComponent<Renderer>().material.color = Color.red;
            }
            if (distance < range) {
                if (selectedTarget.transform.CompareTag("QuestGiver"))
                {
                    selectedTarget.GetComponent<QuestGiverScript>().showQuests();
                }
                if (selectedTarget.transform.CompareTag("ShopKeeper"))
                {
                    selectedTarget.GetComponent<ShopKeeperScript>().showInventory();
                }
            }
        }
    }

    protected void DeselectTarget()
	{ 
		if (selectedTarget && selectedTarget.GetComponent<Renderer>() != null)
		{ // if any guy selected, deselect it 
			selectedTarget.GetComponent<Renderer>().material.color = Color.white; 
			selectedTarget = null; 
		} 
	}

    protected void loseHealth(int x)
    {
        float armorBlock = 0;
        foreach(Item item in equipment.getItems())
        {
            if(null != item && item.getType() == "Armor") {
                armorBlock += ((Armor)item).getArmorPower();
            }
        }
        armorBlock = 1 - (armorBlock / 100);
        float newDamage = x * armorBlock;
        Debug.Log("Armor block: " + armorBlock);
        Debug.Log("Old damage: " + x + ", New Damage: " + newDamage);
        currentHealth -= (int)newDamage;
        if (currentHealth <= 0)
            Die();
    }

    protected bool loseResource(int x)
    {
        //To be implemented by children classes
        return false;
    }

    protected virtual void fillResource() {
        print("Ugh?");
        //To be implemented by children classes
    }

    protected int getMaxHealth()
    {
        return 100 + (level * 20) + (strength*10);
    }

    public bool gainHealth(int health) { 
        if(currentHealth >= getMaxHealth()) {
            return false;
        }
        else {
            currentHealth += health;
            if(currentHealth >= getMaxHealth()) {
                fullHeal();
            }
            return true;
        }
    }

    protected void gainExp(int x)
    {
        exp += x;
        while (exp >= GetExpToNextLevel())
        {
            int leftoverXP = exp - GetExpToNextLevel();
            LevelUp();
            exp += leftoverXP;
        }
    }

    public virtual bool gainResource(int x) { 
        return false;
    }

    public void setStrength(int newStrength) {
        this.strength = newStrength;
    }

    public void setIntelligence(int intelligence) {
        this.intelligence = intelligence;
    }

    public void setAgility(int agility) {
        this.agility = agility;
    }

    public void setSkillPoints(int skillPoints) {
        this.skillPoints = skillPoints;
    }

    protected void incrementQuestProgress(EnemyScript enemyScript) {
        foreach(Quest quest in activeQuests){ 
            if(!quest.completed) {
                if(quest.objectiveTarget == enemyScript.getName()) {
                    quest.currentNumber++;
                }
                if(quest.numberObjective == quest.currentNumber) {
                    quest.completed = true;
                }
            }
        }
    }

    public void killedEnemy(EnemyScript enemyScript) {
        gainExp(enemyScript.expWorth);
        gainGold(enemyScript.goldWorth);
        incrementQuestProgress(enemyScript);
    }

    public void completeQuest(Quest quest) {
        removeQuest(quest);
        gainExp(quest.expReward);
        gainGold(quest.goldReward);
        //TODO: Do something with gold reward
    }

    public void removeQuest(Quest quest) {
        activeQuests.Remove(quest);
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public void fullHeal() {
        this.currentHealth = this.getMaxHealth();
    }

    protected int GetMaxHealth()
    {
        return 50 + (10 * level) + (10 * strength);
    }

    protected virtual int getMaxResource() {
        return 100;
    }

    protected int GetExpToNextLevel()
    {
        return (level * 50);
    }

    public int getSkillPoints() {
        return skillPoints;
    }

    public int getStrength() {
        return strength;
    }

    public int getIntelligence() {
        return intelligence;
    }

    public int getAgility() {
        return agility;
    }

    public int getGold() {
        return gold;
    }

    public void setGold(int gold) {
        this.gold = gold;
    }

    public void gainGold(int x) {
        this.gold += x;
    }

    public void loseGold(int x) {
        if(this.gold - x <= 0) {
            this.gold = 0;
        }
        else {
            this.gold -= x;
        }
    }

    public bool getLevelupToggle() {
        return levelUpMenuToggle;
    }

    public void setLevelupToggle(bool levelUpMenuToggle) {
        this.levelUpMenuToggle = levelUpMenuToggle;
    }

    public Inventory getInventory() {
        return inventory;
    }

    protected void LevelUp()
    {
        level++;
        skillPoints += 5;
        currentHealth = getMaxHealth();
        fillResource();
        exp = 0;
    }

    public void addQuest(Quest quest) {
        activeQuests.Add(quest);
    }

    protected void Die()
    {
        //Destroy(camera);
        //Destroy(this.gameObject);
        GameObject[] objects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject o in objects)
        {
            EnemyScript eScript = o.GetComponent<EnemyScript>();
            if(eScript != null)
            {
                Destroy(o);
            }
        }
        gameScript.setMenuOpen(true);
        //Destroy(gameObject);
        //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public bool hasQuest(Quest quest) {
        return activeQuests.Contains(quest); 
    }

    public void useItem(Item item) {
        if(item.use()) {
            inventory.loseItem(item);
        }
    }

    public bool buyItem(Item item, int cost) {
        if(gold >= cost && inventory.addItem(item)) {
            gold -= cost;
            return true;
        }
        else {
            Debug.Log("Either not enough gold or not enough inventory space");
            return false;
        }
    }

    public Equipment getEquipment() {
        return equipment;
    }

    public Armor getArmorSlot(string slot) {
        return (Armor)equipment.getItemMap()[slot];
    }
}
