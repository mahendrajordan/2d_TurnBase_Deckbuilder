using UnityEngine;

public class BattleMaster : MonoBehaviour
{
    MainInputSystem mainInputSystem;
    DeckBuilderMaster deckBuilderMaster;

    public PlayerBody playerBody;
    public EnemyBody[] enemyBodys;
    int enemyAmount = 0;

    Vector2 mouseVector;
    MainBody mainTarget;

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


    public void RemoveEnemy()
    {
        enemyAmount--;
        if(enemyAmount<=0)
        {
            enemyAmount =0;
            //win
        }
    }

    
#region Mouse
    void MouseCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            MainBody mainBody = hit.transform.GetComponent<MainBody>();
            if(mainBody == null)
            {
                mainTarget = null;
                return;
            }

            if(mainBody.isCanSelect && mainTarget != mainBody)
            {
                mainTarget = mainBody;
            }
        }
    }

    void SelectTarget()
    {
        if(mainTarget == null) return;

        deckBuilderMaster.ActiveCard(mainTarget);
        mainTarget = null;
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
}
