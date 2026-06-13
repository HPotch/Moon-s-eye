using System;
using System.Collections;
using UnityEngine;

public class SquashStretch : MonoBehaviour
{
    [Header("Squash and stretch core")]
    [SerializeField, Tooltip("Defaults to current GO if not set.")] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField, Range(0f, 1f)] private float animationDuration;
    [SerializeField] private bool canBeOverwritten;
    [SerializeField] private bool playOnStart;

    [Flags]
    public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }

    [Header("Animation Settings")] 
    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maximumScale = 1.3f;
    [SerializeField] private bool resetToInitialScaleAfterAnimation = true;

    [SerializeField] private AnimationCurve squashAndStretchCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
        );

    [Header("Looping Settings")]
    [SerializeField] private bool looping;
    [SerializeField] private float loopingDelay = 0.5f;

    private Coroutine _squashAndStretchRoutine;
    private WaitForSeconds _loopingDelayWaitForSeconds;
    private Vector3 _initialScaleVector;
    
    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake()
    {
        if (transformToAffect == null)
            transformToAffect = transform;

        _initialScaleVector = transformToAffect.localScale;
        _loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
    }

    private void Start()
    {
        if (playOnStart) CheckForAndStartRoutine();
    }

    [ContextMenu("Play squash snd stretch")]
    public void PlaySquashAndStretch(bool canOverwrite = false)
    {
        if (looping && !canBeOverwritten) return;
        if (!canOverwrite && _squashAndStretchRoutine != null) return;
        CheckForAndStartRoutine();
    }

    private void CheckForAndStartRoutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("Axis to affect is set to None", gameObject);
            return;
        }

        if (_squashAndStretchRoutine != null)
        {
            StopCoroutine(_squashAndStretchRoutine);
            if (resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = _initialScaleVector;
        }
        
        _squashAndStretchRoutine = StartCoroutine(SquashAndStretchEffect());
    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            float elapsedTime = 0;
            Vector3 originalScale = _initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float curvePosition = elapsedTime / animationDuration;
                float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));

                float minimumThreshold = 0.0001f;
                if (Mathf.Abs(remappedValue) < minimumThreshold)
                    remappedValue = minimumThreshold;

                if (affectX)
                    modifiedScale.x = originalScale.x * remappedValue;
                else
                    modifiedScale.x = originalScale.x / remappedValue;

                if (affectY)
                    modifiedScale.y = originalScale.y * remappedValue;
                else
                    modifiedScale.y = originalScale.y / remappedValue;

                if (affectZ)
                    modifiedScale.z = originalScale.z * remappedValue;
                else
                    modifiedScale.z = originalScale.z / remappedValue;

                transformToAffect.localScale = modifiedScale;

                yield return null;
            }

            if (resetToInitialScaleAfterAnimation) transformToAffect.localScale = _initialScaleVector;

            if (looping) yield return _loopingDelayWaitForSeconds;
        } while (looping);
        _squashAndStretchRoutine = null;
    }
    
    public void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }
}
