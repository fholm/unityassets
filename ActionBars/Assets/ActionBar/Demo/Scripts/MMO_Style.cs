using UnityEngine;
using System.Collections;
using System.Linq;

public class MMO_Style : MonoBehaviour
{
    public ActionBarRow BagRow;
    public ActionBarRow BottomBar;
    public GameObject Spellbook_Background;
    public ActionBarRow Spellbook_Buttons;
    public GameObject Bag_Background;
    public ActionBarRow Bag_Buttons;

    ActionBarDescriptor[] spellDescriptors = new ActionBarDescriptor[0];

    void Start()
    {
        spellDescriptors = new ActionBarDescriptor[16];

        for (int i = 0; i < spellDescriptors.Length; ++i)
        {
            spellDescriptors[i] = new ActionBarDescriptor
                {
                    Atlas = 2,
                    Icon = i,
                    Callback = (d) =>
                    {
                        d.Cooldown = 5f;
                    },
                };
        }

        BottomBar.AddInitCallback((row) =>
        {
            row.SetButton(0, spellDescriptors[0]);
            row.SetButton(1, spellDescriptors[1]);
            row.SetButton(2, spellDescriptors[3]);
            row.SetButton(3, spellDescriptors[11]);
            row.SetButton(4, spellDescriptors[15]);
        });

        BagRow.AddInitCallback((row) =>
        {
            row.SetButton(0, new ActionBarDescriptor
            {
                Atlas = 3,
                Icon = 3,
                Callback = BagClick,
                PressAudioClip = Resources.Load("43598__freqman__garbage-bag-3", typeof(AudioClip)) as AudioClip
            });

            row.SetButton(1, new ActionBarDescriptor
            {
                Atlas = 3,
                Icon = 2,
                Callback = SpellBookClick,
                PressAudioClip = Resources.Load("FlippingPages", typeof(AudioClip)) as AudioClip
            });
        });

        Spellbook_Buttons.AddInitCallback((row) =>
        {
            for (int n = 0; n < 16; ++n)
            {
                row.SetButton(n, spellDescriptors[n]);
            }
        });

        Bag_Buttons.AddInitCallback((row) =>
        {
            InitPotion(row, 0, 8);
            InitPotion(row, 1, 8);
            InitPotion(row, 2, 9);
            InitPotion(row, 3, 9);
            InitPotion(row, 4, 10);
            InitPotion(row, 5, 10);
        });


        Bag_Background.transform.localScale = new Vector3(256, 256, 1);
    }

    void Update()
    {
        Bag_Background.transform.position = new Vector3(
            (Screen.width/2) - 138,
            0,
            4
        );
    }

    void InitPotion(ActionBarRow row, int b, int n)
    {
        row.SetButton(b, new ActionBarDescriptor
        {
            Atlas = 3,
            Icon = n,
            ItemGroup = 1,
            ItemType = n,
            Stackable = true,
            Stack = 1,
            Callback = PotionClick
        });
    }

    void PotionClick(ActionBarDescriptor descriptor)
    {
        if (descriptor.Stack > 0)
        {
            descriptor.Stack -= 1;
            descriptor.Cooldown = 10;

            if (descriptor.Stack == 0)
            {
                foreach (ActionBarButton b in descriptor.Buttons.ToArray())
                {
                    if (b.ItemGroup == descriptor.ItemGroup)
                    {
                        b.RemoveDescriptor();
                    }
                }
            }
            else
            {

            }
        }
    }

    void SpellBookClick(ActionBarDescriptor descriptor)
    {
        Spellbook_Background.active = true;
        Spellbook_Buttons.gameObject.active = true;
    }

    void BagClick(ActionBarDescriptor descriptor)
    {

    }
}