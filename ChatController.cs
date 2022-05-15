using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    // ㅇㅠ지 보수를 전혀 고려하지 않은 코딩이다라는 생각이 든다 ....
    // 새로 다시 만들 것인가 ???
    public AudioClip getMessageSound;
    public AudioClip sendMessageSound;
    public AudioClip[] typingSound;




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
        if(GameObject.Find("ChatPanel"))
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




    public void ShowHeroineMessage(string line)
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
  
        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;

        if (panelHeight + 50f >= maskHeight)
        {
            chatBoxes.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);
        }

        obj.GetComponent<CanvasGroup>().alpha = 1;       
        box.gameObject.SetActive(true);

    }

    public void ShowPlayerMessage(string line)
    {

        string sentence = line.Split(':')[1];
        Transform chatBoxes = GameObject.Find("Chat_Boxes").transform;

        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Player"), chatBoxes);
        Transform box = obj.transform.GetChild(0);
        Text messageText = box.GetChild(0).GetComponent<Text>();
        messageText.text = sentence;

        
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

        if (panelHeight + 50 >= maskHeight)
        {
            chatBoxes.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);
        }


        box.gameObject.SetActive(true);
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

        for(int i=0; i< chatBoxes.transform.childCount-1; i++)
        {
            nextBoxPosY += chatBoxes.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
            nextBoxPosY += space;
        }

        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(100, -150 - nextBoxPosY);


        for (int i = 0; i < chatBoxes.childCount; i++)
        {
            panelHeight += chatBoxes.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;

            try
            {
                if (chatBoxes.transform.GetChild(i).GetChild(4).gameObject.activeSelf == true)
                {
                    nextBoxPosY += 220;
                }
            }
            catch
            {

            }

            panelHeight += space;
        }
        chatBoxes.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight+150f);

        yield return new WaitForEndOfFrame();


        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;

        if(panelHeight+50f>=maskHeight)
        {
            chatBoxes.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight-maskHeight+150, 0);
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


 
}
