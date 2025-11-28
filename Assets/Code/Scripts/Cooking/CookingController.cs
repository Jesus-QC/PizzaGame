using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Cooking
{
    public class CookingController : MonoBehaviour
    {
        private static int AnimatorNextHash = Animator.StringToHash("Next");

        public AudioSource SoundSource;
        public Animator CanvasAnimator;
        public Animator DoughAnimator;
        public Animator KnifeAnimator;
        public KeyPopUp KeyPopUp;
        public BadPopUp BadPopUp;
        public GoodPopUp GoodPopUp;
        public AudioClip SuccessClip;
        public AudioClip VictoryClip;

        private CookingStep _currentStep = CookingStep.NotStarted;
        private bool _locked = true;

        private void AdvanceToNextStep()
        {
            KeyPopUp.HideAllKeys();
            _locked = true;
            _currentStep++;
            DoughAnimator.SetTrigger(AnimatorNextHash);
            KnifeAnimator.SetTrigger(AnimatorNextHash);
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
                case CookingStep.SetupKnife when input.y < -0.5f:
                case CookingStep.RollDoughX1 when input.y < -0.5f:
                case CookingStep.RotateDough2 when input.y < -0.5f:
                case CookingStep.Cut1 when input.y > 0.5f:
                    {
                        StartCoroutine(ShowNextInstructions("W"));
                        AdvanceToNextStep();
                        break;
                    }

                case CookingStep.SetupDough when input.y > 0.5f:
                case CookingStep.Cut1 when input.y > 0.5f:
                case CookingStep.RotateDough3 when input.y > 0.5f:
                    {
                        StartCoroutine(ShowNextInstructions("S"));
                        AdvanceToNextStep();
                        break;
                    }

                case CookingStep.RotateDough1 when input.x > 0.5f:
                    {
                        StartCoroutine(ShowGoodPopUp("S"));
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
                        StartCoroutine(FinishedDough());
                        break;
                    }
                case CookingStep.Cut2 when input.y > 0.5f:
                    {
                        AdvanceToNextStep();
                        StartCoroutine(Error());
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
            SoundSource.PlayOneShot(SuccessClip);
            yield return new WaitForSeconds(2f);
            _locked = false;
            KeyPopUp.ShowKey(nextKey);
        }

        private IEnumerator ShowGoodPopUp(string nextKey)
        {
            GoodPopUp.Enable();
            // We speed up the music
            SoundSource.pitch = 1.1f;
            yield return new WaitForSeconds(2f);
            StartCoroutine(ShowNextInstructions(nextKey));
        }

        private IEnumerator FinishedDough()
        {
            AdvanceToNextStep();
            SoundSource.pitch = 1f;
            yield return null;
            SoundSource.PlayOneShot(VictoryClip, 0.25f);
            yield return new WaitForSeconds(2f);
            CanvasAnimator.SetTrigger(AnimatorNextHash);
            yield return new WaitForSeconds(3f);
            DoughAnimator.gameObject.SetActive(false);
            KnifeAnimator.gameObject.SetActive(true);
            yield return new WaitForSeconds(6f);
            SoundSource.pitch = 1.2f;
            yield return new WaitForSeconds(1f);
            KeyPopUp.ShowKey("S");
            _locked = false;
        }

        private IEnumerator Error()
        {
            yield return new WaitForSeconds(0.4f);
            SoundSource.pitch = 0.5f;
            yield return new WaitForSeconds(0.1f);
            SoundSource.enabled = false;
            yield return new WaitForSeconds(4f);
            SceneManager.LoadScene(2);
        }
    }
}