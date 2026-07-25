using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayHistory : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textInfomationPrefab;
    [SerializeField] Transform textParent;

    [SerializeField] Transform mainPanel;
    [SerializeField] Transform showPoint;
    [SerializeField] Transform hidePoint;
    [SerializeField] Button showBtn;
    int clickIndex = 0;

    void Start()
    {
        mainPanel.position = hidePoint.position;
        showBtn.onClick.AddListener(ShowHidePanelAction);
    }

    public void CreateHitInformation(MainBody user, MainBody target, int attackRoll, int armorClassTarget)
    {
        bool isMising = attackRoll < armorClassTarget;
        string attackText = isMising ? "<color=red>Miss</color>" : "<color=red>Hit</color>";
        string info = $"{user.characterName} attack {target.characterName} is {attackText}, \nAttack Roll <color=red>{attackRoll}</color> Armor Class Target <color=red>{armorClassTarget}</color> ";
        SpawnText(info);
    }

    public void CreateDamageInformation(MainBody user, MainBody target, int damageRoll)
    {
        string info = $"{user.characterName} give damage to {target.characterName}, Damage Roll <color=red>{damageRoll}</color>";
        SpawnText(info);
    }

    public void CreateCardInformation(MainBody user, CardData cardData)
    {
        string info = $"{user.characterName} use <color=red>{cardData.name}</color>";
        SpawnText(info);
    }

    public void CreateTakeEffectInformation(MainBody target, BuffDebuffData buffDebuffData)
    {
        string info = $"{target.characterName} take <color=red>{buffDebuffData.name}</color>";
        SpawnText(info);
    }

    void SpawnText(string n)
    {
        var txt= Instantiate(textInfomationPrefab, textParent);
        txt.text = n;
    }

#region Show Hide Panel
    void ShowHidePanelAction()
    {
        if(clickIndex==0) ShowPanel();
        else HidePanel();

        clickIndex = clickIndex==1 ? 0 : 1;
    }

    void ShowPanel() => StartCoroutine(MovePanel(hidePoint.position, showPoint.position));
    void HidePanel() => StartCoroutine(MovePanel(showPoint.position, hidePoint.position));

    IEnumerator MovePanel(Vector2 start, Vector3 end)
    {
        float duration = .25f;
        float timer = 0;
        float lerpPoint = 0;

        do
        {
            timer += Time.deltaTime;
            lerpPoint = timer/duration;
            mainPanel.position = Vector2.Lerp(start, end, lerpPoint);

            yield return null;
        }while(lerpPoint<1);
        mainPanel.position = end;
    }
#endregion
}
