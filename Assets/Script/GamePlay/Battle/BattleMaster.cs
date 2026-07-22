using UnityEngine;

public class BattleMaster : MonoBehaviour
{
    public GamePlayCondition gamePlayCondition;
    MainInputSystem mainInputSystem;
    DeckBuilderMaster deckBuilderMaster;
    [SerializeField] TurnBaseSystem turnBaseSystem;

    public PlayerBody playerBody;
    public EnemyBody[] enemyBodys;
    int enemyAmount = 0;

    Vector2 mouseVector;
    MainBody mainTarget;

    BuffDebuffIcon CurrentBuffDebuffIconSelect;

    void Awake()
    {
        Setup();
    }

    void Update()
    {
        MouseCheck();
    }

    #region Setup
    void Setup()
    {
        mainInputSystem = new MainInputSystem();
        deckBuilderMaster = FindAnyObjectByType<DeckBuilderMaster>();
        enemyAmount = enemyBodys.Length;

        turnBaseSystem.SetupTurnBaseSystem(playerBody, enemyBodys);

        SetupInput();
    }

    void SetupInput()
    {
        mainInputSystem.Main.MousePosition.performed += Context => mouseVector = Context.ReadValue<Vector2>();
        mainInputSystem.Main.MousePosition.canceled += Context => mouseVector = Vector2.zero;

        mainInputSystem.Main.MouseClick.performed += Context => SelectTarget();
    }
#endregion

#region Select
    public void ActiveSelectAllEnemy(bool isSelect)
    {
        foreach(EnemyBody enemyBody in enemyBodys)
        {
            enemyBody.ActiveSelect(isSelect);
        }
    }

    public void ActiveSelectPlayer(bool isSelect)
    {
        playerBody.ActiveSelect(isSelect);
    }
#endregion
   
#region Mouse
    void MouseCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            MainBodyCheck(hit);
            BuffDebuffIconCheck(hit);
        }
        else
        {
            HideBuffdebuffIcon();
        }
    }

    void MainBodyCheck(RaycastHit2D hit)
    {
        MainBody mainBody = hit.transform.GetComponent<MainBody>();
        if(mainBody == null)
        {
            mainTarget = null;
        }
        else if(mainBody.isCanSelect && mainTarget != mainBody)
        {
            mainTarget = mainBody;
        }
    }

    void BuffDebuffIconCheck(RaycastHit2D hit)
    {
        BuffDebuffIcon buffDebuffIcon = hit.transform.GetComponent<BuffDebuffIcon>();
        if(buffDebuffIcon)
        {
            CurrentBuffDebuffIconSelect = buffDebuffIcon;
            CurrentBuffDebuffIconSelect.ShowDescriptionPanel();
        }
    }

    void HideBuffdebuffIcon()
    {
        if(CurrentBuffDebuffIconSelect == null) return;
        CurrentBuffDebuffIconSelect.HideDescriptionPanel();
        CurrentBuffDebuffIconSelect = null;
    }

    void SelectTarget()
    {
        if(mainTarget == null) return;

        deckBuilderMaster.ActiveCard(mainTarget);
        mainTarget = null;
    }
#endregion

#region Player & enemy    
    public void RemoveEnemy(EnemyBody enemyBody)
    {
        enemyAmount--;    
        turnBaseSystem.RemoveEnemy(enemyBody);    

        if(enemyAmount<=0)
        {
            enemyAmount =0;
            gamePlayCondition = GamePlayCondition.win;
        }
    }

    public void PlayerIsDead()
    {
        gamePlayCondition = GamePlayCondition.Lose;
    }
#endregion

#region Enable Disable
    void OnEnable()
    {
        mainInputSystem.Enable();
    }

    void OnDisable()
    {
        mainInputSystem.Disable();
    }
#endregion

    public TurnBaseSystem GetTurnBaseSystem() =>turnBaseSystem;
}
