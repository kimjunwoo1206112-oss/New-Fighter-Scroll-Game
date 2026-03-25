using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
    [Header("플레이어 이미지 버튼 (GridLayoutGroup 하위 3개)")]
    [SerializeField] private Button[] playerButtons = new Button[3];
    [SerializeField] private Image[] playerButtonImages = new Image[3];
    [SerializeField] private GameObject[] highlights = new GameObject[3];

    [Header("전투기 이름 (A, B, C)")]
    [SerializeField] private Text[] fighterNameTexts = new Text[3];

    [Header("전투기 공격력 (버튼 하위)")]
    [SerializeField] private Text[] attackTexts = new Text[3];

    [Header("전투기 공격속도 (버튼 하위)")]
    [SerializeField] private Text[] attackSpeedTexts = new Text[3];

    [Header("선택시 상세 설명")]
    [SerializeField] private Text descriptionText;

    [Header("스테이터스 이미지")]
    [SerializeField] private Text statusHPText;
    [SerializeField] private Text statusSpeedText;
    [SerializeField] private Text statusAtkText;
    [SerializeField] private Text statusAtkSpeedText;

    [Header("선택된 전투기 큰 이미지")]
    [SerializeField] private Image selectedFighterImage;

    [Header("스프라이트")]
    [SerializeField] private Sprite[] fighterSprites = new Sprite[3];

    [Header("시작 버튼")]
    [SerializeField] private Button startButton;

    [Header("게임 씬")]
    [SerializeField] private string gameSceneName = "test";

    private int selectedPlayerId = 0;
    private string[] fighterNames = new string[3];
    private string[] fighterDescs = new string[3];
    private string[] fighterHP = new string[3];
    private string[] fighterSpeed = new string[3];
    private string[] fighterAtk = new string[3];
    private string[] fighterFireRate = new string[3];

    private void Start()
    {
        // 패널 배경만 투명하게 설정 (GridLayoutGroup, LayoutGroup이 있는 것)
        var layouts = FindObjectsOfType<UnityEngine.UI.LayoutGroup>();
        foreach (var layout in layouts)
        {
            var img = layout.GetComponent<UnityEngine.UI.Image>();
            if (img != null && layout.gameObject.name != "Background")
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }
        
        // 이름에 "Panel"이 포함된 것도 투명하게
        var allImages = FindObjectsOfType<UnityEngine.UI.Image>();
        foreach (var img in allImages)
        {
            if (img.gameObject.name.Contains("Panel") && img.gameObject.name != "Background")
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
        }
        
        Debug.Log("[UI] 패널 배경 투명 설정 완료");

        // DescImage > 테스트 자동 연결
        if (descriptionText == null)
        {
            GameObject descImage = GameObject.Find("DescImage");
            if (descImage != null)
            {
                Transform testTransform = descImage.transform.Find("테스트");
                if (testTransform != null)
                {
                    descriptionText = testTransform.GetComponent<UnityEngine.UI.Text>();
                    Debug.Log("[UI] DescImage > 테스트 Text 자동 연결 완료");
                }
            }
        }

        LoadFighterData();

        for (int i = 0; i < playerButtons.Length; i++)
        {
            if (playerButtons[i] != null)
            {
                int index = i;
                playerButtons[i].onClick.AddListener(() => SelectFighter(index));
            }

            if (playerButtonImages[i] != null && fighterSprites[i] != null)
            {
                playerButtonImages[i].sprite = fighterSprites[i];
            }

            if (fighterNameTexts[i] != null)
            {
                fighterNameTexts[i].text = fighterNames[i];
            }

            if (attackTexts[i] != null)
            {
                attackTexts[i].text = fighterAtk[i];
            }

            if (attackSpeedTexts[i] != null)
            {
                attackSpeedTexts[i].text = fighterFireRate[i];
            }
        }

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        SelectFighter(0);
    }

    private void LoadFighterData()
    {
        if (ExcelDataManager.Instance == null)
        {
            Debug.LogWarning("ExcelDataManager가 없습니다.");
            return;
        }

        string category = "Player";

        // Player 카테고리의 모든 키 출력
        var allData = ExcelDataManager.Instance.GetAllData();
        Debug.Log("===== Player 카테고리 전체 키 =====");
        if (allData.ContainsKey(category))
        {
            foreach (var item in allData[category])
            {
                Debug.Log($"  {category}.{item.Key} = {item.Value}");
            }
        }
        else
        {
            Debug.LogWarning("Player 카테고리 없음!");
            foreach (var cat in allData.Keys)
            {
                Debug.Log($"  존재하는 카테고리: {cat}");
            }
        }
        Debug.Log("================================");

        for (int i = 0; i < 3; i++)
        {
            int id = i + 1;

            fighterNames[i] = ExcelDataManager.Instance.GetValue(category, $"{id}_Name", $"전투기 {id}");
            fighterDescs[i] = ExcelDataManager.Instance.GetValue(category, $"{id}_Desc", "설명 없음");
            fighterHP[i] = ExcelDataManager.Instance.GetValue(category, $"{id}_HP", "100");
            fighterSpeed[i] = ExcelDataManager.Instance.GetValue(category, $"{id}_Speed", "5");
            fighterAtk[i] = ExcelDataManager.Instance.GetValue(category, $"{id}_Damage", "10");
            fighterFireRate[i] = ExcelDataManager.Instance.GetValue(category, $"{id}_AttackSpeed", "1.0");

            Debug.Log($"전투기 {id}: Name={fighterNames[i]}, Desc={fighterDescs[i]}");
        }

        Debug.Log("엑셀에서 전투기 데이터 로드 완료");
    }

    private void SelectFighter(int index)
    {
        selectedPlayerId = index + 1;
        
        // PlayerPrefs에 선택한 플레이어 ID 저장
        PlayerPrefs.SetInt("SelectedPlayerId", selectedPlayerId);
        PlayerPrefs.Save();
        Debug.Log($"PlayerPrefs에 선택한 ID 저장: {selectedPlayerId}");

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SelectPlayer(selectedPlayerId);
        }

        for (int i = 0; i < highlights.Length; i++)
        {
            if (highlights[i] != null)
                highlights[i].SetActive(i == index);
        }

        if (selectedFighterImage != null && fighterSprites[index] != null)
        {
            selectedFighterImage.sprite = fighterSprites[index];
        }

        if (descriptionText != null)
        {
            descriptionText.text = fighterDescs[index];
            Debug.Log($"[UI] descriptionText 업데이트: {fighterDescs[index]}");
        }
        else
        {
            Debug.LogWarning("[UI] descriptionText가 null입니다! Inspector에서 연결해주세요.");
        }

        if (statusHPText != null)
        {
            statusHPText.text = "HP " + fighterHP[index];
        }

        if (statusSpeedText != null)
        {
            statusSpeedText.text = "속도 " + fighterSpeed[index];
        }

        if (statusAtkText != null)
        {
            statusAtkText.text = "공격력 " + fighterAtk[index];
        }

        if (statusAtkSpeedText != null)
        {
            statusAtkSpeedText.text = "공격속도 " + fighterFireRate[index];
        }

        Debug.Log($"{fighterNames[index]} 선택됨");
    }

    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    private void SetPanelTransparent(string panelName)
    {
        GameObject panel = GameObject.Find(panelName);
        if (panel != null)
        {
            var image = panel.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                Color c = image.color;
                c.a = 0f;
                image.color = c;
                Debug.Log($"[UI] {panelName} Image 투명 설정 완료");
            }
        }
    }
}