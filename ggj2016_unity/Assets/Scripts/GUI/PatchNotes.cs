using UnityEngine;
using System.Collections;

public class PatchNotes : MonoBehaviour
{

    public static PatchNotes Instance;

    public UILabel titleLabel;
    public UILabel notesLabel;

    public TweenScale scale;

    // grade
    private bool _showingGrade;
    public UILabel gradeLabel;
    public TweenAlpha gradeAlpha;
    public TweenScale gradeScale;

    public AudioSource audio;
    public AudioClip textBlip;
    public AudioClip gradeBlip;

    protected void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SetText(string title, string notes)
    {
        gameObject.SetActive(true);
        
        titleLabel.text = title;
        notesLabel.text = notes; 

        scale.ResetToBeginning();
        scale.PlayForward();
    }

    public void ShowFinalGrade()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowFinalGradeCoroutine());
    }

    private IEnumerator ShowFinalGradeCoroutine()
    {
        _showingGrade = true;
        gradeAlpha.ResetToBeginning();
        gradeScale.ResetToBeginning();
        SetText("RAID OVER", "");

        yield return new WaitForSeconds(1f);

        notesLabel.text = string.Format("Final fun p/s: {0} \n", FunSystem.FunPerSecond);
        audio.PlayOneShot(textBlip);

        yield return new WaitForSeconds(1f);

        notesLabel.text += string.Format("Total fun: {0}", FunSystem.TotalFun);
        audio.PlayOneShot(textBlip);


        yield return new WaitForSeconds(1f);

        notesLabel.text += string.Format("\n\nFinal grade:");
        audio.PlayOneShot(textBlip);


        yield return new WaitForSeconds(1f);

        // todo: grading system
        gradeLabel.text = FunSystem.GetFinalGrade();

        gradeAlpha.PlayForward();
        gradeScale.PlayForward();

        yield return new WaitForSeconds(0.2f);
        audio.PlayOneShot(gradeBlip);

        yield return new WaitForSeconds(1f);

        _showingGrade = false;

    }

    public void Update()
    {
        if (Input.anyKeyDown && !_showingGrade)
        {
            CloseNotes();
        }
    }

    public void CloseNotes()
    {
        gameObject.SetActive(false);
    }
}
