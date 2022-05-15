using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    
    public AudioClip getMessageSound;
    public AudioClip sendMessageSound;
    public AudioClip[] typingSound;

    const float maskHeight = 993f;



    #region Show,Hide Chat Panel

    public void ShowChatPanel()
    {
        if (GameObject.Find("ChatPanel") == null)
        {
            GameObject panel = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Panel"), GameObject.Find("Cam").transform);
            panel.name = "ChatPanel";
            panel.GetComponent<CanvasGroup>().alpha = 1;
        }
    }


    public void HideChatPanel()
    {
        if (GameObject.Find("ChatPanel"))
        {
            Destroy(GameObject.Find("ChatPanel"));
        }

        Resources.UnloadUnusedAssets();
    }


    public IEnumerator RunHideChatPanelImmidately()
    {
        if (GameObject.Find("ChatPanel"))
        {
            GameObject chatPanel = GameObject.Find("ChatPanel");
            chatPanel.SetActive(false);
            Destroy(chatPanel);
        }

        yield return new WaitForSeconds(0.25f);
        Resources.UnloadUnusedAssets();
    }


    public IEnumerator RunShowChatPanel()
    {
        if (GameObject.Find("ChatPanel") == null)
        {
            GameObject panel = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Panel"), GameObject.Find("Cam").transform);
            panel.name = "ChatPanel";
            yield return StartCoroutine(SpriteController.FadeIn(panel, 1.5f));
            yield return new WaitForSeconds(0.75f);
        }
    }


    public IEnumerator RunHideChatPanel()
    {
        GameObject panel = GameObject.Find("ChatPanel");
        yield return StartCoroutine(SpriteController.FadeOut(panel, 1.75f));
        Destroy(panel);
        yield return new WaitForEndOfFrame();
        Resources.UnloadUnusedAssets();
        yield return new WaitForSeconds(0.5f);
    }

    #endregion





    public void ShowHeroineMessage(string line)
    {
        string charName         = line.Split(':')[0];
        string sentence         = line.Split(':')[1];
        
        Transform chatBoxTransform     = GameObject.Find("Chat_Boxes").transform;


        GameObject chatBox = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Heroine"), chatBoxTransform);
        

        InputName(chatBox, charName);
        SetSentenceBox(chatBox, sentence);
        LocateSetHeroineBox(chatBox, chatBoxTransform);
        SetPanelHeight(chatBoxTransform);
        SetEmoji(chatBox, sentence);


        if(chatBox.transform.childCount==5)
        {
            chatBox.transform.GetChild(4).GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            chatBox.transform.GetChild(0).gameObject.SetActive(true);
        }
            
        chatBox.GetComponent<CanvasGroup>().alpha = 1;
    }


    public void ShowPlayerMessage(string line)
    {

        string sentence = line.Split(':')[1];
        Transform chatBoxTransform = GameObject.Find("Chat_Boxes").transform;

        GameObject chatBox = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Player"), chatBoxTransform);
        
        SetSentenceBox(chatBox, sentence);
        LocateSetPlayerBox(chatBox, chatBoxTransform);
        SetPanelHeight(chatBoxTransform);

        chatBox.gameObject.SetActive(true);
    }


    public IEnumerator RunShowHeroineMessage(string line)
    {
        string charName = line.Split(':')[0];
        string sentence = line.Split(':')[1];
        Transform chatBoxes = GameObject.Find("Chat_Boxes").transform;

        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Heroine"), chatBoxes);

        switch (charName)
        {
            case "이슬비":
                {
                    obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_sb");
                }
                break;
            case "강나린":
                {
                    obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_nr");
                }
                break;
            case "안시은":
                {
                    obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_se");
                }
                break;
            default:
                {
                    obj.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_empty");
                }
                break;
        }

        obj.transform.GetChild(2).GetComponent<Text>().text = charName;

        Transform box = obj.transform.GetChild(0);
        Text messageText = box.GetChild(0).GetComponent<Text>();
        messageText.text = sentence;

        yield return new WaitForEndOfFrame();

        float boxHeight = messageText.GetComponent<Text>().preferredHeight + 22.5f > 73f ?
            messageText.GetComponent<Text>().preferredHeight + 22.5f : 73f;

        box.GetComponent<RectTransform>().sizeDelta
            = new Vector2(messageText.GetComponent<Text>().preferredWidth + 42.5f, boxHeight);


        float panelHeight = 0;
        float nextBoxPosY = 0;
        float space = 50f;

        for (int i = 0; i < chatBoxes.transform.childCount - 1; i++)
        {
            nextBoxPosY += chatBoxes.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
            nextBoxPosY += space;
        }

        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(100, -150 - nextBoxPosY);


        for (int i = 0; i < chatBoxes.childCount; i++)
        {
            panelHeight += chatBoxes.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
            panelHeight += space;
        }

        chatBoxes.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight + 150f);

        yield return new WaitForEndOfFrame();


        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;

        if (panelHeight + 50f >= maskHeight)
        {
            chatBoxes.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);
        }




        obj.GetComponent<CanvasGroup>().alpha = 1;

        if (ConfigData.fastMode == false)
        {
            yield return StartCoroutine(RunLoadingHeroineMessage(obj, sentence));
        }
        else
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.volume = ConfigData.effectVolume;
            audioSource.PlayOneShot(getMessageSound);
        }

        box.gameObject.SetActive(true);
    }


    public IEnumerator RunShowPlayerMessage(string line)
    {
        string sentence = line.Split(':')[1];
        Transform chatBoxes = GameObject.Find("Chat_Boxes").transform;

        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Player"), chatBoxes);
        Transform box = obj.transform.GetChild(0);
        Text messageText = box.GetChild(0).GetComponent<Text>();
        messageText.text = sentence;

        yield return new WaitForEndOfFrame();

        float boxHeight = messageText.GetComponent<Text>().preferredHeight + 22.5f > 73f ?
            messageText.GetComponent<Text>().preferredHeight + 22.5f : 73f;

        box.GetComponent<RectTransform>().sizeDelta
            = new Vector2(messageText.GetComponent<Text>().preferredWidth + 42.5f, boxHeight);



        float panelHeight = 0;
        float nextBoxPosY = 0;
        float space = 50f;

        for (int i = 0; i < chatBoxes.transform.childCount - 1; i++)
        {
            nextBoxPosY += chatBoxes.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
            nextBoxPosY += space;
        }

        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(75, -150 - nextBoxPosY);


        for (int i = 0; i < chatBoxes.childCount; i++)
        {
            panelHeight += chatBoxes.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
            panelHeight += space;
        }
        chatBoxes.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight + 150f);


        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;



        if (ConfigData.fastMode == false)
        {
            yield return StartCoroutine(RunLoadingPlayerMessage(sentence));
        }
        else
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.volume = ConfigData.effectVolume;
            audioSource.PlayOneShot(sendMessageSound);
        }


        box.gameObject.SetActive(true);

        if (panelHeight + 50 >= maskHeight)
        {
            chatBoxes.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);
        }
    }






    #region Loading Animation...

    IEnumerator RunLoadingHeroineMessage(GameObject obj, string sentence)
    {
        GameObject loadingAnim = obj.transform.GetChild(3).gameObject;
        AudioSource audioSource = GetComponent<AudioSource>();
        float typingTime = Mathf.Lerp(0.5f, 2f, sentence.Length / 20f);
        float time = 0;

        loadingAnim.SetActive(true);


        while (time <= typingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        loadingAnim.SetActive(false);
        audioSource.volume = ConfigData.effectVolume;
        audioSource.PlayOneShot(getMessageSound);
    }


    IEnumerator RunLoadingPlayerMessage(string sentence)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        float typingTime = Mathf.Lerp(0.5f, 2f, sentence.Length / 20f);
        float time = 0;

        audioSource.volume = ConfigData.effectVolume;
        audioSource.PlayOneShot(typingSound[Random.Range(0, typingSound.Length)]);

        while (time <= typingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.PlayOneShot(sendMessageSound);
    }

    #endregion



    void InputName(GameObject chatBox, string charName)
    {
        switch (charName)
        {
            case "이슬비":
                {
                    chatBox.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_sb");
                }
                break;
            case "강나린":
                {
                    chatBox.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_nr");
                }
                break;
            case "안시은":
                {
                    chatBox.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_se");
                }
                break;
            default:
                {
                    chatBox.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Chats/chat_empty");
                }
                break;
        }

        chatBox.transform.GetChild(2).GetComponent<Text>().text = charName;
    }

    void SetSentenceBox(GameObject chatBox, string sentence)
    {
        Transform sentenceBox = chatBox.transform.GetChild(0);
        Text messageText = sentenceBox.GetChild(0).GetComponent<Text>();
        messageText.text = sentence;

        float boxHeight = messageText.GetComponent<Text>().preferredHeight + 22.5f > 73f ?
            messageText.GetComponent<Text>().preferredHeight + 22.5f : 73f;

        sentenceBox.GetComponent<RectTransform>().sizeDelta
            = new Vector2(messageText.GetComponent<Text>().preferredWidth + 42.5f, boxHeight);
    }

    void LocateSetHeroineBox(GameObject chatBox, Transform chatBoxTransform)
    {
        float nextBoxPosY = 0;
        float space = 50;

        for (int i = 1; i < chatBoxTransform.transform.childCount; i++)
        {
            if (chatBoxTransform.transform.GetChild(i-1).childCount == 5)
            {
                nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 200f;
                nextBoxPosY += space;
            }
            else
            {
                nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                nextBoxPosY += space;
            }
        }
        chatBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(100, -150 - nextBoxPosY);
    }

    void LocateSetPlayerBox(GameObject chatBox, Transform chatBoxTransform)
    {
        float nextBoxPosY = 0;
        float space = 50;

        for (int i = 1; i < chatBoxTransform.transform.childCount; i++)
        {
            if (chatBoxTransform.transform.childCount == 5)
            {
                nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 200f;
                nextBoxPosY += space;
            }
            else
            {
                nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                nextBoxPosY += space;
            }
        }
        chatBox.GetComponent<RectTransform>().anchoredPosition = new Vector3(75, -150 - nextBoxPosY);
    }

    void SetPanelHeight(Transform chatBoxTransform)
    {
        float panelHeight = 0;

        float space = 50;

        for (int i = 0; i < chatBoxTransform.childCount; i++)
        {
            if (chatBoxTransform.transform.childCount == 5)
            {
                panelHeight += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 200f;
                panelHeight += space;
            }
            else
            {
                panelHeight += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                panelHeight += space;
            }
        }

        chatBoxTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight + 150f);


        if (panelHeight+ 50f >= maskHeight)
        {
            chatBoxTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);
        }
    }

    void SetEmoji(GameObject chatBox, string sentence)
    {
        if(sentence.Contains("<")&&sentence.Contains(">"))
        {
           GameObject emoji = Instantiate(Resources.Load<GameObject>("Prefabs/Emoji"),chatBox.transform);
            emoji.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Emoji/" + sentence.Split('<', '>')[1]);
        }
    }

}