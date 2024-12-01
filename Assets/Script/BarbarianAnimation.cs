using UnityEngine;
using UnityEngine.Events;
public class BarbarianAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string movementSpeed = "MovementSpeed";
    public UnityEvent OnStep;
    public void SetSpeed(float speed){
        animator.SetFloat(movementSpeed, speed);
    }

    public void StepEvent()
    {
        OnStep.Invoke();
    }

}
