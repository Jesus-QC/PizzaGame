using System.Collections;
using Code.Scripts.Level.Interactables;
using TMPro;
using UnityEngine;

namespace Assets.Code.Scripts.Player
{
    public class TaskController : MonoBehaviour
    {
        private static readonly int OpenAnimation = Animator.StringToHash("Open");

        public Animator TestAnimator;
        public TextMeshProUGUI ObjectiveTitle;
        public TextMeshProUGUI ObjectiveDescription;
        public AudioClip NewTask;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(3f);

            if (InteractableHomework.HasStartedHomework)
                yield break;

            ObjectiveTitle.text = "Haz los deberes";
            ObjectiveDescription.text = "Abre moodle en el ordenador del escritorio";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                if (InteractableHomework.HasStartedHomework)
                {
                    Close();
                    yield break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            Close();
        }


        public void Open()
        {
            TestAnimator.SetBool(OpenAnimation, true);
            PlayerController.Instance.GlobalAudioSource.PlayOneShot(NewTask);
        }

        public void Close()
        {
            TestAnimator.SetBool(OpenAnimation, false);
        }

        public void OnFinishedHomework()
        {
            StartCoroutine(FinishHomeworkCoroutine());
        }

        private IEnumerator FinishHomeworkCoroutine()
        {
            yield return new WaitForSeconds(3f);

            ObjectiveTitle.text = "Saca la basura";
            ObjectiveDescription.text = "Lleva la bolsa de basura de la cocina al contenedor fuera de casa";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Close();
        }

        public void OnFinishedTakingOutTrash()
        {
            StartCoroutine(FinishTakingOutTrashCoroutine());
        }

        private IEnumerator FinishTakingOutTrashCoroutine()
        {
            yield return new WaitForSeconds(3f);

            ObjectiveTitle.text = "Mira la television";
            ObjectiveDescription.text = "Sientate en el sillon y entretente un rato viendo la television";
            Open();

            float elapsedTime = 0f;
            while (elapsedTime < 5f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Close();

            float counter = 0f;

            while (counter < 5f)
            {
                if (InteractableSit.Sitting && InteractableTV.TvOn)
                {
                    counter += Time.deltaTime;
                }

                yield return null;
            }

            ObjectiveTitle.text = "NEXT_TASK_TITLE";
            ObjectiveDescription.text = "NEXT_TASK_DESCRIPTION";
            Open();
        }
    }
}