using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerBox : MonoBehaviour
{
    #region UI properties
    [SerializeField] Image merchantImage;
    [SerializeField] Text playerName;
    [SerializeField] Button ultimateButton;
    [SerializeField] Text manaText;
    #endregion
    public Player player;

    public void Init(Player initPlayer)
    {
        player = initPlayer;
        SetInfo();
    }

    public void ActivateSkill()
    {
        player.GetMerchant().Skill.Activate();
    }

    public void SetInfo()
    {
        merchantImage.sprite = player.GetMerchant().gameObject.GetComponent<Image>().sprite;
        playerName.text = player.Name;

        ultimateButton.onClick.AddListener(ActivateSkill);

        //transform.Find("PanelPrice/Price").GetComponent<Text>().text = value.Price.ToString();
        //transform.Find("Button").GetComponent<Button>().onClick.AddListener(Buy);

        if(transform.Find("PlayerBox/ItemBox") != null)
        {
            SetItems();
        }


        SetMana();
    }
    
    public void SetMana()
    {
        manaText.text = player.GetMana().ToString() + "/" + player.GetMerchant().MaxMana.ToString();

        if(ultimateButton != null)
            ultimateButton.interactable = player.GetMerchant().Skill.CanActivate();
    }

    public void SetItems()
    {
        Queue<string> itemComponent = new Queue<string>();
        for (int i = 1; i <= player.itemLimit; i++)
        {
            itemComponent.Enqueue("Slot" + i);
        }

        for (int i = 1; i <= player.itemLimit; i++)
        {
            Transform b = transform.Find("PlayerBox/ItemBox/" + itemComponent.Dequeue());
            b.GetComponent<UIItemInPlayer>().SetNull();
            itemComponent.Enqueue("Slot" + i);
        }

        foreach (BaseItem item in player.playerItem)
        {
            Transform b = transform.Find("PlayerBox/ItemBox/" + itemComponent.Dequeue());
            b.GetComponent<UIItemInPlayer>().Init(item);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player.MerchantLock += SetInfo;
        player.ItemsChange += SetItems;
        player.ManaChange += SetMana;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
