using TMPro;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject endSessionButton;
    [SerializeField] private GameObject dropDown;
    [SerializeField] private TMP_Dropdown tmpDropDown;
    [SerializeField] private GameObject aiChat;
    [SerializeField] private TextMeshProUGUI aiChatText;
    [SerializeField] private TMP_InputField playerInput;

    private List<string> pidList = new List<string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void BeenClicked()
    {
        loginButton.SetActive(false);

        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<FSLoginSystem>().Enabled = true;
    }

    public void GetAuthResultsWeb(string result)
    {
        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<FSLoginSystem>().GetAuthResultsWeb(result);
    }

    public void DropDown()
    {
        tmpDropDown.options.Clear();
        tmpDropDown.captionText.text = "";

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(AncestorComponent));

        NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.TempJob);

        foreach (Entity entity in entities)
        {
            AncestorComponent ancestorComponent = entityManager.GetComponentData<AncestorComponent>(entity);

            tmpDropDown.options.Add(new TMP_Dropdown.OptionData() { text = ancestorComponent.name.ToString() + " : " + ancestorComponent.pid.ToString() });

            pidList.Add(ancestorComponent.pid.ToString());
        }

        entities.Dispose();

        dropDown.SetActive(true);
    }

    public void DropDownSelected()
    {
        dropDown.SetActive(false);

        endSessionButton.SetActive(true);

        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<FSAncestorInfoSystem>().StartSystem(pidList[tmpDropDown.value]);
    }

    public void EndSessionClicked()
    {
        aiChat.SetActive(false);

        endSessionButton.SetActive(false);

        dropDown.SetActive(true);

        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<AIEndSessionSystem>().Enabled = true;
    }

    public void ActivateAiChat()
    {
        aiChat.SetActive(true);
    }

    public void SetAIText(string aiResponse)
    {
        aiChatText.text = aiResponse;
    }

    public void PlayerSubmit()
    {
        string playerResponse = playerInput.text;

        playerInput.text = "";

        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<AISendTextSystem>().SendText(playerResponse);
    }

    public void PlaySound(AudioClip audioFromEleven)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioFromEleven;
        audioSource.Play();
    }
}
