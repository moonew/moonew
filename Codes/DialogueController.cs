using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Video;

public class DialogueController : MonoBehaviour
{
    
    #region Controllers

    public BackgroundController backgroundController;
    public CharacterController characterController;
    public LogPanelController logController;
    public SoundController soundController;
    public CameraController cameraController;
    public ObjectController objectController;
    public KeywordController keywordController;

    #endregion


    #region UI

    public Text charNameText;
    public Text sentenceText;
    public GameObject dialogueBox;
    public GameObject autoModeObj;
    public GameObject voiceButton;
    public GameObject dialogueTail;

    #endregion


    #region Coroutines

    public Coroutine clickRoutine = null;
    Coroutine typingRoutine = null;
    Coroutine tailAnimRoutine = null;
    Coroutine autoModeRoutine = null;
    
    #endregion


    #region SceneVariable

    public string   scenarioFileName = "";
    public string[] lines;
    public int      index = 0;
    
    #endregion


    #region LogVariable

    string          currentTalker = "";
    string          currentSentence = "";
    string          currentVoiceFileName = "";

    #endregion



    public bool isPanelOpened = false;








    #region Public Function


    public void GetScenario(string fileName , int index = 0)
    {
        clickRoutine = null;

        TextAsset asset = Resources.Load<TextAsset>("Scenario/" + fileName);
        string data = asset.ToString();
        lines = data.Split('\n');
        this.index = index;

        clickRoutine = StartCoroutine(RunClick());
    }


    public void Click()
    {
        if (isPanelOpened == false)
        {
            if (autoModeRoutine != null)
            {
                autoModeObj.SetActive(false);
                StopCoroutine(autoModeRoutine);
                autoModeRoutine = null;
            }
            // if it is typing, skipTyping.
            else if (typingRoutine != null)
            {
                SkipTyping();
            }
            else if (clickRoutine == null)
            {
                //check index
                if (index == lines.Length) return;
                else
                    clickRoutine = StartCoroutine(RunClick());
            }
        }

    }


    public void OnClickAutoButton()
    {
        if (autoModeRoutine != null)
        {
            autoModeObj.SetActive(false);
            StopCoroutine(autoModeRoutine);
            autoModeRoutine = null;
        }
        else
        {
            autoModeRoutine = StartCoroutine(RunAutoMode());
        }
    }


    public void LoadClick()
    {
        clickRoutine = StartCoroutine(RunClick());
    }


    public void InitDialogueBox()
    {
        if(typingRoutine!=null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        if(tailAnimRoutine!=null)
        {
            StopCoroutine(tailAnimRoutine);
            tailAnimRoutine = null;
            dialogueTail.SetActive(false);
        }


        charNameText.text = "";
        sentenceText.text = "";
    }

    #endregion






    #region SubFunction



    IEnumerator RunClick()
    {
        float delayForTyping = 0.05f;
        string currentLine = lines[index];


        //turnoff tailAnim
        if (tailAnimRoutine != null) StopCoroutine(tailAnimRoutine);
        dialogueTail.SetActive(false);



        //Do Command until talking
        while (index < lines.Length)
        {
            if (currentLine.Contains("(") && currentLine.Contains(")") && !currentLine.Contains(":"))
            {
                yield return StartCoroutine(RunCommand(currentLine));

                if (++index == lines.Length)
                {
                    Debug.Log("This is the last sentence");
                    clickRoutine = null;
                    yield break;
                }

                currentLine = lines[index];
            }
            else
            {
                yield return new WaitForSeconds(delayForTyping);
                break;
            }

            yield return null;
        }


        if (dialogueBox.GetComponent<CanvasGroup>().alpha == 0)
        {
            yield return StartCoroutine(SpriteController.FadeIn(dialogueBox, 0.5f));
            yield return new WaitForSeconds(0.75f);
            dialogueBox.GetComponent<CanvasGroup>().interactable = true;
        }

        Talk(currentLine);

        logController.Add(currentTalker, currentSentence, currentVoiceFileName);


        index++;
        clickRoutine = null;
    }


    IEnumerator RunCommand(string commandLine)
    {
        string command = commandLine.Split('(', ')')[0];
        string[] p = commandLine.Split('(', ')')[1].Split(',');

        switch (command)
        {


            #region Dialogue
            case "delay":
                {
                    yield return new WaitForSeconds(float.Parse(p[0]));
                }break;

            case "db_fadeIO":
                {
                    yield return StartCoroutine(SpriteController.FadeOut(dialogueBox, 1.5f));
                    yield return new WaitForSeconds(0.75f);
                    charNameText.text = "";
                    sentenceText.text = "";
                    yield return StartCoroutine(SpriteController.FadeIn(dialogueBox, 1.5f));
                }
                break;

            #endregion



            #region Cam

            case "cam_move":
                {
                    yield return StartCoroutine(cameraController.Move(p[0], p[1], p[2], p[3]));
                }
                break;


            case "cam_set_pos":
                {
                    cameraController.SetCamPos(p[0], p[1], p[2]);
                }
                break;

            case "cam_shake":
                {
                    yield return StartCoroutine(cameraController.Shake(p[0]));
                }break;


            #endregion



            #region Background

            case "bg_set_posX":
                {
                    backgroundController.SetPosX(p[0]);
                }
                break;

            case "bg_set_scale":
                {
                    backgroundController.SetScale(p[0]);
                }
                break;

            case "fadein":
                {
                    yield return StartCoroutine(backgroundController.RunFadeIn(p[0], p[1]));
                }
                break;

            case "swipe":
                {
                    yield return StartCoroutine(backgroundController.RunSwipe(p[0], p[1]));
                }
                break;
            case "crossfade":
                {
                    yield return StartCoroutine(backgroundController.RunCrossFade(p[0], p[1]));
                }
                break;
            #endregion



            #region Video

            case "video_play":
                {
                    yield return StartCoroutine(GetComponent<VideoController>().PlayVideo(p[0]));
                }break;

            case "video_rewind":
                {
                    yield return StartCoroutine(SpriteController.FadeOut(dialogueBox, 0.75f));
                    charNameText.text = "";
                    sentenceText.text = "";

                    yield return StartCoroutine(soundController.FadeOutBgm("1"));

                    VideoPlayer vp = GetComponent<VideoPlayer>();
                    StartCoroutine(GetComponent<VideoController>().PlayVideo("prologue_rewind"));

                    while(!vp.isPlaying)
                    {
                        yield return new WaitForSeconds(0.25f);
                    }

                    backgroundController.ShowBackground("street1_t2");
                    characterController.Hide("sb");

                    

                    while(vp.isPlaying)
                    {
                        yield return new WaitForSeconds(0.25f);
                    }

                    yield return StartCoroutine(SpriteController.FadeIn(dialogueBox, 1));

                }
                break;


            #endregion



            #region Character Basic Function

            case "create":
                {
                   characterController.CreateCharacter(p[0], p[1], p[2], p[3]);
                }break;

            case "distance":
                {
                    characterController.SetDistance(p[0], p[1]);
                }break;

            case "posX":
                {
                    characterController.SetPosX(p[0], p[1]);
                }break;

            case "show":
                {
                    yield return StartCoroutine(characterController.RunShow(p[0]));
                }break;

            case "show_all":
                {
                    Transform characters = GameObject.Find("Characters").transform;

                    for(int i=0; i<characters.childCount; i++)
                    {
                        if(i== characters.childCount -1)
                        {
                            yield return StartCoroutine(characterController.RunShow(characters.GetChild(i).name));
                        }
                        else
                        {
                            StartCoroutine(characterController.RunShow(characters.GetChild(i).name));
                        }
                    }

                }break;

            case "hide":
                {
                    yield return StartCoroutine(characterController.RunHide(p[0]));

                }break;

            case "hide_all":
                {
                    Transform characters = GameObject.Find("Characters").transform;

                    for (int i = 0; i < characters.childCount; i++)
                    {
                        if (i == characters.childCount - 1)
                        {
                            yield return StartCoroutine(characterController.RunHide(characters.GetChild(i).name));
                        }
                        else
                        {
                            StartCoroutine(characterController.RunHide(characters.GetChild(i).name));
                        }
                    }
                }
                break;

            case "e":
                {
                    yield return StartCoroutine(characterController.RunChangeExpression(p[0], p[1], p[2]));
                }break;

            case "move":
                {
                    if(p.Length==3)
                    {
                        yield return StartCoroutine(characterController.RunMove(p[0], p[1], p[2]));
                    }
                    else if(p[4]=="show")
                    {
                        StartCoroutine(characterController.RunShow(p[0]));
                        yield return StartCoroutine(characterController.RunMove(p[0], p[1], p[2]));
                    }
                    else if(p[4]=="hide")
                    {
                        StartCoroutine(characterController.RunMove(p[0], p[1], p[2]));
                        yield return StartCoroutine(characterController.RunHide(p[0]));
                    }
                    
                }break;



            #endregion


            //not yet
            #region Character Extra Function

            case "shake":
                {
                    yield return StartCoroutine (characterController.RunShake(p[0], p[1], p[2], p[3]));
                }break;


            case "rabbitear_show":
                {
                    yield return StartCoroutine(characterController.RunRabbitEar());
                    
                }break;

            case "rabbitear_set":
                {
                    characterController.SetRabbitEar();
                }break;


            case "glasses":
                {

                }break;

            case "hat":
                {

                }break;

            #endregion



            #region Sound

            case "bgm_play":
                {
                    soundController.PlayBgm(p[0]);
                }break;

            case "bgm_stop":
                {
                    soundController.StopBgm();
                }break;

            case "bgm_fadeout":
                {
                    StartCoroutine(soundController.FadeOutBgm(p[0]));
                }
                break;

            case "effect_play":
                {
                    soundController.PlayEffect(p[0]);
                }break;

            case "effect_stop":
                {
                    soundController.StopEffect();
                }break;

            #endregion


            #region Keyword
            case "keyword":
                {
                    keywordController.ShowKeyword(p[0]);
                }break;
                
                #endregion

        }

    }


    




    IEnumerator RunAutoMode()
    {
        dialogueTail.SetActive(false);
        autoModeObj.SetActive(true);

        if (clickRoutine == null && typingRoutine == null)
        {
            yield return new WaitForSeconds(1.5f);
            clickRoutine = StartCoroutine(RunClick());
        }
        else
        {
            Debug.Log("coroutine is running");
        }
    }


    IEnumerator RunTyping(string sentence)
    {

        

        float typingSpeed = ConfigData.textSpeed;

        sentenceText.text = "";
        currentSentence = sentence;

        foreach (char c in sentence)
        {
            sentenceText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }



        tailAnimRoutine = StartCoroutine(RunTail());
        typingRoutine = null;
    }


    IEnumerator RunTail()
    {
        float delayTime = 0.3f;
        yield return new WaitForSeconds(delayTime);

        if (autoModeRoutine != null)
        {
            float time = 0;
            float targetTime = ConfigData.autoSpeed;

            if (soundController.voiceSpeaker.isPlaying)
            {
                while (soundController.voiceSpeaker.isPlaying)
                {
                    if (autoModeRoutine == null)
                    {
                        break;
                    }
                    yield return new WaitForSeconds(0.2f);
                }
            }

            while (time <= targetTime)
            {
                if (autoModeRoutine == null)
                {
                    dialogueTail.SetActive(true);
                    break;
                }
                time += 0.2f;
                yield return new WaitForSeconds(0.2f);
            }

            clickRoutine = StartCoroutine(RunClick());
        }
        else
        {
            dialogueTail.SetActive(true);
        }

    }




    void Talk(string currentLine)
    {
        //Index for Debug.
        //GameObject.Find("IndexText").GetComponent<DebugIndex>().ShowIndex();

        int letterCount = 43;


        if (ConfigData.voiceCutMode == true)
        {
            soundController.voiceSpeaker.Stop();
        }

        currentTalker = currentLine.Split(":")[0].Trim();

        if(currentTalker=="주")
        {
            currentTalker = PlayerPrefs.GetString("TempCharName","오성훈");
        }

        charNameText.text = currentTalker;


        string adjustSentence = "";

        if (currentTalker != "이슬비" && currentTalker != "강나린" && currentTalker != "안시은")
        {
            string temp;

            currentVoiceFileName = "";
            voiceButton.SetActive(false);
            temp = currentLine.Split(":")[1];

            foreach (char c in temp)
            {
                if (adjustSentence.Length == letterCount)
                {
                    if (c == ' ' || c == ',' || c == '.' || c=='?')
                    {
                        adjustSentence += c;
                        adjustSentence += "\n";
                    }
                    else
                    {
                        adjustSentence += "\n";
                        adjustSentence += c;
                    }
                }
                else
                {
                    adjustSentence += c;
                }

            }
        }
        else
        {
            string temp;

            temp = currentLine.Split(':', '[')[1];
            foreach (char c in temp)
            {
                if (adjustSentence.Length == letterCount)
                {
                    if (c == ' ' || c == ',' || c == '.')
                    {
                        adjustSentence += c;
                        adjustSentence += "\n";
                    }
                    else
                    {
                        adjustSentence += "\n";
                        adjustSentence += c;
                    }
                }
                else
                {
                    adjustSentence += c;
                }

            }

            currentVoiceFileName = currentLine.Split('[', ']')[1];

            soundController.PlayVoice(currentTalker, currentVoiceFileName);
            voiceButton.GetComponent<VoiceButton>().voiceFileName = currentVoiceFileName;
            voiceButton.SetActive(true);
        }

        currentSentence = adjustSentence;


        if (ConfigData.fastMode == false)
        {
            
            typingRoutine = StartCoroutine(RunTyping(adjustSentence));
        }
        else
        {
            sentenceText.text = adjustSentence;
            tailAnimRoutine = StartCoroutine(RunTail());
            typingRoutine = null;
        }
    }
    

    void SkipTyping()
    {
        StopCoroutine(typingRoutine);
        typingRoutine = null;
        sentenceText.text = currentSentence;
        tailAnimRoutine = StartCoroutine(RunTail());
    }



    #endregion


    




    #region Legacy

    //IEnumerator RunCommand(string commandLine)
    //{
    //    string command = commandLine.Split('(', ')')[0];
    //    string[] parameters = commandLine.Split('(', ')')[1].Split(',');


    //    switch (command)
    //    {

    //        #region DialogueControl

    //        //Dialogue...
    //        case "scene_end":
    //            {

    //                //TurnOff DialogueBox...
    //                GameObject dialogueBox = GameObject.Find("DialogueBox");
    //                float dialogueBoxFadeOutTime = 0.35f;
    //                yield return StartCoroutine(SpriteController.FadeOut(dialogueBox, dialogueBoxFadeOutTime));
    //                voiceButton.SetActive(false);



    //                //FadeOut Cam
    //                float targetTime = float.Parse(parameters[0]);

    //                GameObject fadeRect = new GameObject("FadeRect");
    //                fadeRect.transform.SetParent(GameObject.Find("Cam").transform);
    //                fadeRect.AddComponent<CanvasGroup>().alpha = 0;
    //                fadeRect.AddComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
    //                fadeRect.AddComponent<Image>().color = Color.black;

    //                StartCoroutine(soundController.FadeOutBgm(targetTime));
    //                yield return StartCoroutine(SpriteController.FadeIn(fadeRect, targetTime));


    //                GameObject.Find("Background").GetComponent<Image>().sprite = null;
    //                GameObject.Find("Background").GetComponent<CanvasGroup>().alpha = 0;
    //                backgroundController.SetBackgroundScale("1");
    //                backgroundController.background.transform.localPosition = Vector3.zero;
    //                GameObject.Find("Cam").transform.localPosition = Vector3.zero;

    //                Transform characters = GameObject.Find("Characters").transform;

    //                foreach (Transform obj in characters)
    //                {
    //                    Destroy(obj.gameObject);
    //                }
    //                yield return new WaitForEndOfFrame();
    //                Destroy(fadeRect);

    //                Resources.UnloadUnusedAssets();
    //            }
    //            break;


    //        case "delay":
    //            {
    //                yield return new WaitForSeconds(float.Parse(parameters[0]));
    //            }
    //            break;


    //        #endregion


    //        #region BackgroundControl
    //        case "set_bg_scale":
    //            {
    //                backgroundController.SetBackgroundScale(parameters[0]);
    //            }
    //            break;
    //        case "set_bg_posX":
    //            {
    //                backgroundController.SetPos(parameters[0]);
    //            }
    //            break;

    //        case "reset_bg_scale":
    //            {
    //                backgroundController.SetBackgroundScale("1");
    //            }
    //            break;


    //        case "fadeIn":
    //            {

    //                GameObject dialogueBox = GameObject.Find("DialogueBox");


    //                if (dialogueBox.GetComponent<CanvasGroup>().alpha == 1)
    //                {
    //                    yield return new WaitForEndOfFrame();
    //                    float dialogueBoxFadeInTime = 0.35f;

    //                    dialogueBox.GetComponent<CanvasGroup>().interactable = false;
    //                    yield return StartCoroutine(SpriteController.FadeOut(dialogueBox, dialogueBoxFadeInTime));
    //                    voiceButton.SetActive(false);
    //                }


    //                yield return StartCoroutine(backgroundController.FadeInOut(parameters[0], float.Parse(parameters[1])));

    //                if (dialogueBox.GetComponent<CanvasGroup>().alpha == 0)
    //                {
    //                    charNameText.text = "";
    //                    sentenceText.text = "";
    //                    yield return new WaitForEndOfFrame();
    //                    float dialogueBoxFadeInTime = 0.35f;
    //                    yield return StartCoroutine(SpriteController.FadeIn(dialogueBox, dialogueBoxFadeInTime));
    //                    dialogueBox.GetComponent<CanvasGroup>().interactable = true;
    //                }
    //                break;

    //            }
    //        case "swipe":
    //            {

    //                GameObject dialogueBox = GameObject.Find("DialogueBox");

    //                if (dialogueBox.GetComponent<CanvasGroup>().alpha == 1)
    //                {
    //                    yield return new WaitForEndOfFrame();
    //                    float dialogueBoxFadeInTime = 0.35f;
    //                    yield return StartCoroutine(SpriteController.FadeOut(dialogueBox, dialogueBoxFadeInTime));
    //                    voiceButton.SetActive(false);
    //                }


    //                yield return StartCoroutine(backgroundController.Swipe(parameters[0], float.Parse(parameters[1])));

    //                if (dialogueBox.GetComponent<CanvasGroup>().alpha == 0)
    //                {
    //                    charNameText.text = "";
    //                    sentenceText.text = "";
    //                    yield return new WaitForEndOfFrame();
    //                    float dialogueBoxFadeInTime = 0.35f;
    //                    yield return StartCoroutine(SpriteController.FadeIn(dialogueBox, dialogueBoxFadeInTime));
    //                }
    //                break;
    //            }

    //        case "swipe_ecg":
    //            {
    //                GameObject dialogueBox = GameObject.Find("DialogueBox");

    //                if (dialogueBox.GetComponent<CanvasGroup>().alpha == 1)
    //                {
    //                    yield return new WaitForEndOfFrame();
    //                    float dialogueBoxFadeInTime = 0.35f;
    //                    yield return StartCoroutine(SpriteController.FadeOut(dialogueBox, dialogueBoxFadeInTime));
    //                    voiceButton.SetActive(false);
    //                }


    //                yield return StartCoroutine(backgroundController.SwipeEcg(parameters[0], float.Parse(parameters[1])));

    //                if (dialogueBox.GetComponent<CanvasGroup>().alpha == 0)
    //                {
    //                    charNameText.text = "";
    //                    sentenceText.text = "";
    //                    yield return new WaitForEndOfFrame();
    //                    float dialogueBoxFadeInTime = 0.35f;
    //                    yield return StartCoroutine(SpriteController.FadeIn(dialogueBox, dialogueBoxFadeInTime));
    //                }
    //                break;
    //            }


    //        case "crossFade":
    //            {
    //                yield return StartCoroutine(backgroundController.CrossFade(parameters[0], float.Parse(parameters[1])));
    //                break;
    //            }

    //        case "show_bg": { backgroundController.ShowBackground(parameters[0]); } break;
    //        case "hide_bg": { backgroundController.HideBackground(); } break;

    //        #endregion


    //        #region SoundControl
    //        //Sound....
    //        case "playBgm":
    //            {
    //                soundController.PlayBgm(parameters[0]);
    //                break;
    //            }
    //        case "playEffect":
    //            {
    //                soundController.PlayEffect(parameters[0]);
    //                break;
    //            }
    //        case "stopBgm":
    //            {
    //                soundController.StopBgm();
    //                break;
    //            }
    //        case "stopEffect":
    //            {
    //                soundController.StopEffect();
    //                break;
    //            }
    //        case "fadeOutBgm":
    //            {
    //                soundController.fadeOutRoutine = StartCoroutine(soundController.FadeOutBgm(float.Parse(parameters[0])));
    //            }
    //            break;

    //        #endregion


    //        #region CharacterControl
    //        //Character....
    //        case "create":
    //            {
    //                if (parameters.Length == 4)
    //                {
    //                    characterController.CreateCharacter(parameters[0], parameters[1], parameters[2], parameters[3]);
    //                }
    //                else if (parameters.Length == 5)
    //                {
    //                    characterController.CreateCharacter(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
    //                }
    //                else
    //                {
    //                    characterController.CreateCharacter(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);
    //                }
    //                break;
    //            }

    //        case "create_cc": { characterController.CreateChongChong(parameters[0], parameters[1], parameters[2], parameters[3]); break; }

    //        case "create_extra": { characterController.CreateExtra(parameters[0]); break; }

    //        case "adjust_pos": { characterController.AdjustPosition(parameters[0], parameters[1], parameters[2], parameters[3]); } break;
    //        case "adjust_scale": { characterController.AdjustScale(parameters[0], parameters[1]); } break;

    //        case "set_pos": { characterController.SetPosition(parameters[0], parameters[1], parameters[2], parameters[3]); break; }

    //        case "set_posX": { characterController.SetPositionX(parameters[0], parameters[1]); break; }

    //        case "set_scale": { characterController.SetScale(parameters[0], parameters[1]); break; }




    //        case "show":
    //            {
    //                yield return StartCoroutine(characterController.ShowCharacter(parameters[0]));
    //                break;
    //            }
    //        case "showAll":
    //            {
    //                Transform charTransform = GameObject.Find("Characters").transform;


    //                for (int i = 0; i < charTransform.childCount; i++)
    //                {

    //                    if (i == charTransform.childCount - 1)
    //                    {
    //                        yield return StartCoroutine(characterController.ShowCharacter(charTransform.GetChild(i).name));
    //                    }
    //                    else
    //                    {
    //                        StartCoroutine(characterController.ShowCharacter(charTransform.GetChild(i).name));
    //                    }
    //                }
    //                break;
    //            }
    //        case "e":
    //            {
    //                if (parameters.Length == 3)
    //                {
    //                    yield return StartCoroutine(characterController.ChangeExpression(parameters[0], parameters[1], parameters[2]));
    //                }
    //                else
    //                {
    //                    string charName = GameObject.Find("Characters").transform.GetChild(0).name;
    //                    yield return StartCoroutine(characterController.ChangeExpression(charName, parameters[0], parameters[1]));
    //                }

    //                break;
    //            }
    //        case "move":
    //            {
    //                yield return StartCoroutine(characterController.Move(parameters[0], float.Parse(parameters[1]), float.Parse(parameters[2])));
    //                break;
    //            }
    //        case "move_show":
    //            {
    //                yield return StartCoroutine(characterController.MoveAndShow(parameters[0], float.Parse(parameters[1]), float.Parse(parameters[2])));
    //                break;
    //            }

    //        case "move_hide":
    //            {
    //                yield return StartCoroutine(characterController.MoveAndHide(parameters[0], float.Parse(parameters[1]), float.Parse(parameters[2])));
    //                break;
    //            }
    //        case "hide":
    //            {
    //                yield return StartCoroutine(characterController.HideCharacter(parameters[0]));
    //                break;
    //            }
    //        case "hideAll":
    //            {
    //                Transform charTransform = GameObject.Find("Characters").transform;

    //                foreach (Transform heroine in charTransform)
    //                {
    //                    if (charTransform.childCount != 1)
    //                    {
    //                        StartCoroutine(characterController.HideCharacter(heroine.name));
    //                    }
    //                    else
    //                    {
    //                        yield return StartCoroutine(characterController.HideCharacter(heroine.name));
    //                    }

    //                }
    //                break;
    //            }
    //        case "show_ear": { yield return StartCoroutine(characterController.RabbitEar()); } break;

    //        case "show_hood": { yield return StartCoroutine(characterController.Hood()); } break;

    //        case "shake":
    //            {
    //                yield return StartCoroutine(characterController.Shake(parameters[0], parameters[1], parameters[2], parameters[3]));
    //            }
    //            break;


    //        case "delete":
    //            {
    //                characterController.DeleteCharacter(parameters[0]);
    //            }
    //            break;



    //        //chongchong....
    //        case "lift_cc":
    //            {
    //                if (GameObject.Find("cc") != null && GameObject.Find("sb") != null)
    //                {
    //                    GameObject.Find("cc").transform.SetParent(GameObject.Find("sb").transform);
    //                }


    //            }
    //            break;
    //        case "talk_cc": { yield return StartCoroutine(characterController.ChongChongTalk()); } break;
    //        #endregion


    //        #region CameraControl
    //        //Cam
    //        case "cam_move":
    //            {
    //                yield return StartCoroutine(cameraController.Move(new Vector3(float.Parse(parameters[0]), float.Parse(parameters[1]),
    //                    float.Parse(parameters[2])), float.Parse(parameters[3])));
    //                break;
    //            }
    //        case "cam_move_reset":
    //            {
    //                yield return StartCoroutine(cameraController.MoveReset(float.Parse(parameters[0])));
    //                break;
    //            }
    //        case "cam_shake":
    //            {
    //                yield return StartCoroutine(cameraController.Shake(int.Parse(parameters[0])));
    //                break;
    //            }
    //        #endregion


    //        #region ObjectControl
    //        //Object
    //        case "show_object":
    //            {
    //                if (parameters.Length == 1)
    //                {
    //                    yield return StartCoroutine(objectController.Show(parameters[0]));
    //                }
    //                else
    //                {
    //                    yield return StartCoroutine(objectController.
    //                        Show(parameters[0], parameters[1], parameters[2], parameters[3]));
    //                }

    //            }
    //            break;

    //        case "hide_object":
    //            {

    //                yield return StartCoroutine(objectController.Hide());

    //            }
    //            break;

    //        #endregion


    //        case "keyword":
    //            {
    //                keywordController.ShowKeyword(parameters[0]);
    //            }
    //            break;



    //    }
    //    yield return null;
    //}


    #endregion
}


