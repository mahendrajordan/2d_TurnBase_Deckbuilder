using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMaster : MonoBehaviour
{
    public GamePlayCondition gamePlayCondition;
    MainInputSystem mainInputSystem;
    DeckBuilderMaster deckBuilderMaster;
    WinLoseSystem winLoseSystem;
    [SerializeField] TurnBaseSystem turnBaseSystem;

    public PlayerBody playerBody;
    public EnemyBody[] enemyBodys;
    int enemyAmount = 0;

    Vector2 mouseVector;
    MainBody mainTarget;
    MainBody mainCharacterDetail;

    BuffDebuffIcon CurrentBuffDebuffIconSelect;

    [Header("UI")]
    [SerializeField] CanvasGroup turnPanel;
    [SerializeField] TextMeshProUGUI turnText;

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
        winLoseSystem = FindAnyObjectByType<WinLoseSystem>();
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
            CharacterDetailCheck(hit);
        }
        else
        {
            mainTarget = null;
            HideBuffdebuffIcon();
            HideCharacterDetail();
        }
    }

    //Card Target Select
    void MainBodyCheck(RaycastHit2D hit)
    {
        MainBody mainBody = hit.transform.GetComponent<MainBody>();
        if(mainBody == null)
        {            
            mainTarget = null;
        }
        else if(mainBody.isCanSelect && mainCharacterDetail != mainBody)
        {
            mainTarget = mainBody;
        }
    }
    //

    //Character Detail
    void CharacterDetailCheck(RaycastHit2D hit)
    {
        MainBody mainBody = hit.transform.GetComponent<MainBody>();
        if(mainBody == null)
        {            
            HideCharacterDetail();
        }
        else if(mainBody && mainCharacterDetail != mainBody)
        {
            mainCharacterDetail = mainBody;
            mainCharacterDetail.ShowCharacterDetail(true);
        }
    }

    void HideCharacterDetail()
    {
        mainCharacterDetail?.ShowCharacterDetail(false);
        mainCharacterDetail = null;
    }
    //

    //buff debuff detail
    void BuffDebuffIconCheck(RaycastHit2D hit)
    {
        BuffDebuffIcon buffDebuffIcon = hit.transform.GetComponent<BuffDebuffIcon>();
        if(buffDebuffIcon == null)
        {
            HideBuffdebuffIcon();
        }
        else if(buffDebuffIcon && CurrentBuffDebuffIconSelect != buffDebuffIcon)
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
    //

    void SelectTarget()
    {
        if(mainTarget == null) return;

        deckBuilderMaster.ActiveCard(mainTarget);
        mainTarget = null;
    }
#endregion

#region Player & enemy Dead
    public void RemoveEnemy(EnemyBody enemyBody)
    {
        enemyAmount--;    
        turnBaseSystem.RemoveEnemy(enemyBody);    

        if(enemyAmount<=0)
        {
            enemyAmount =0;
            gamePlayCondition = GamePlayCondition.win;

            int currenSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int maxSceneIndex = SceneManager.sceneCountInBuildSettings;
            if(currenSceneIndex == maxSceneIndex-1)
                winLoseSystem.ShowFinishPanel();
            else
                winLoseSystem.ShowWinPanel();
        }
    }

    public void PlayerIsDead()
    {
        gamePlayCondition = GamePlayCondition.Lose;        
        winLoseSystem.ShowLosePanel();
    }
#endregion

#region UI
    public IEnumerator ShowTurnPanel(string n)
    {
        turnText.text = n;
        turnPanel.alpha = 1;

        yield return new WaitForSeconds(1f);

        float duration = .5f;
        float timer = 0;
        float lerpPoint = 0;

        do
        {
            timer += Time.deltaTime;
            lerpPoint = timer/duration;
            float alpha = Mathf.Lerp(1, 0, lerpPoint);
            turnPanel.alpha = alpha;
            yield return null;
        }while(lerpPoint<1);
        turnPanel.alpha = 0;
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
