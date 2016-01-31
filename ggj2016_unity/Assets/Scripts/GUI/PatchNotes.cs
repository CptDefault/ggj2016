using UnityEngine;
using System.Collections;

public class PatchNotes : MonoBehaviour
{

    public static PatchNotes Instance;

    public UILabel titleLabel;
    public UILabel notesLabel;

    public TweenScale scale;

    protected void Awake()
    {
        Instance = this;
    }

    public void SetText(string title, string notes)
    {
        gameObject.SetActive(true);
        
        titleLabel.text = title;
        notesLabel.text = notes; 

        scale.ResetToBeginning();
        scale.PlayForward();

    }

    public void Update()
    {
        if (Input.anyKeyDown)
        {
            CloseNotes();
        }
    }

    public void CloseNotes()
    {
        gameObject.SetActive(false);
        
    }
}
