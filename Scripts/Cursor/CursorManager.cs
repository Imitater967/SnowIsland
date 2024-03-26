using System;
using System.Collections;
using System.Collections.Generic;
using SnowIsland.Scripts.Character;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D aimCursor;
    public Texture2D aimTargetCursor;
    // Start is called before the first frame update
    private CursorManager instance;
    [SerializeField] public Vector2 cursorOffset = new Vector2(-0.5f, -0.5f);
    private void Awake()
    {
        if(instance!=null)
        {    Destroy(gameObject);
            return;}
        instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (PlayerCharacter.Local)
        {
            if (PlayerCharacter.Local.PlayerGunUsage.aiming.Value)
            { 
                if (PlayerCharacter.Local.PlayerControl.hasAimTarget)
                {
                    Cursor.SetCursor(aimTargetCursor,cursorOffset, CursorMode.Auto);
                    return;
                }
                Cursor.SetCursor(aimCursor,cursorOffset,CursorMode.Auto);
                return;
            }  
        } 
        Cursor.SetCursor(defaultCursor,cursorOffset,CursorMode.Auto);
     
         
    }
}
