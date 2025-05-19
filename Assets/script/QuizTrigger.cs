using UnityEngine;
using Vuforia;

public class QuizTrigger : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;

    [Tooltip("World-space canvas with quiz UI")]
    public GameObject quizCanvas;

    [Tooltip("Drag in your QuizManager GameObject here")]
    public QuizManager quizManager;

    [Tooltip("Exact target name, e.g. \"TajMahal\"")]
    public string wonderName;

    [Tooltip("Local position offset for the canvas (X, Y, Z) in meters")]
    public Vector3 canvasOffset = new Vector3(0f, 0.05f, 0.1f);

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour != null)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        if (quizCanvas != null)
            quizCanvas.SetActive(false);
    }

    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        bool visible = (newStatus == TrackableBehaviour.Status.DETECTED ||
                        newStatus == TrackableBehaviour.Status.TRACKED ||
                        newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED);

        if (visible)
        {
            // Auto-center the canvas relative to the ImageTarget
            if (quizCanvas != null)
            {
                // Reset rotation so it faces forward
                quizCanvas.transform.localRotation = Quaternion.identity;
                // Snap local position to zero plus your offset
                quizCanvas.transform.localPosition = canvasOffset;
                // Activate the canvas
                quizCanvas.SetActive(true);
            }

            if (quizManager != null){
                quizManager.Awake();
                quizManager.StartQuiz(wonderName);
            }
        }
        else
        {
            if (quizCanvas != null)
                quizCanvas.SetActive(false);
        }
    }
}
