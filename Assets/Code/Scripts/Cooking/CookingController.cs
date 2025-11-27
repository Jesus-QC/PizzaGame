using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Cooking
{
    public class CookingController : MonoBehaviour
    {
        private static int AnimatorNextHash = Animator.StringToHash("Next");

        public Animator DoughAnimator;
        public Animator KnifeAnimator;
        public KeyPopUp KeyPopUp;
        public BadPopUp BadPopUp;

        private CookingStep _currentStep = CookingStep.NotStarted;
        private bool _locked = true;

        private void AdvanceToNextStep()
        {
            KeyPopUp.HideAllKeys();
            _locked = true;
            _currentStep++;
            DoughAnimator.SetTrigger(AnimatorNextHash);
            //KnifeAnimator.SetTrigger(AnimatorNextHash);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(9f);
            // Play countdown
            yield return new WaitForSeconds(1f);
            // Play countdown
            yield return new WaitForSeconds(1f);
            // Play countdown
            yield return new WaitForSeconds(4f);
            _locked = false;
            KeyPopUp.ShowKey("W");
        }

        private void OnMove(InputValue value)
        {
            if (_locked) return;

            Vector2 input = value.Get<Vector2>();
            switch (_currentStep)
            {

                case CookingStep.NotStarted when input.y > 0.5f:
                case CookingStep.RollDoughX1 when input.y < -0.5f:
                case CookingStep.RotateDough2 when input.y < -0.5f: 
                case CookingStep.RollDoughY1 when input.y < -0.5f:
                    {
                        StartCoroutine(ShowNextInstructions("W"));
                        AdvanceToNextStep();
                        break;
                    }   
                
                case CookingStep.SetupDough when input.y > 0.5f:
                case CookingStep.RotateDough1 when input.x > 0.5f:
                case CookingStep.RotateDough3 when input.y > 0.5f:
                    {
                        StartCoroutine(ShowNextInstructions("S"));
                        AdvanceToNextStep();
                        break;
                    }

                case CookingStep.RollDoughX2 when input.y > 0.5f:

                    {
                        StartCoroutine(ShowNextInstructions("D"));
                        AdvanceToNextStep();
                        break;
                    }
                case CookingStep.RollDoughY1 when input.y < -0.5f:
                    {
                        AdvanceToNextStep();
                        break;
                    }

                default:
                    {
                        BadPopUp.Enable();
                        break;
                    }
            }
        }

        private IEnumerator ShowNextInstructions(string nextKey)
        {
            yield return new WaitForSeconds(2f);
            _locked = false;
            KeyPopUp.ShowKey(nextKey);
        }
    }
}