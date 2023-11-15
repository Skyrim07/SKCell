using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

namespace SKCell
{
    [AddComponentMenu("SKCell/UI/SKScrollController")]
    public class SKScrollController : MonoBehaviour
    {
        public Scrollbar scrollBar;
        public LayoutGroup layoutGroup;

        public bool isGridLayoutGroup=true;
        public bool reverse01 = false;

        [Header("Common")]
        [Range(0,1)]
        [SerializeField] float initialValue = 0;
        [SerializeField] bool calibrateOnEnable = true;
        [SerializeField] bool smoothCalibration = true;

        [Header("ScrollIndicators")]
        [SerializeField] SKUIAnimation indicator0;
        [SerializeField] SKUIAnimation indicator1;

        [Header("HideWhenStatic")]
        [SerializeField] bool hideWhenStatic = true;
        [Range(0.1f,3f)]
        [SerializeField] float staticTime = 0.5f;
        [SerializeField] SKUIAnimation scrollBarUIAnim;

        private float timer = 0;
        private void Start()
        {
            Invoke("CalibrateToInitialValue", 0.1f);
            scrollBar.onValueChanged.AddListener(OnScrollValueChanged);
        }
        private void Update()
        {
            if (hideWhenStatic)
            {
                timer += Time.deltaTime;

                if (timer >= staticTime)
                {
                    HideScrollBar();
                }
            }
        }
        private void OnEnable()
        {
            if (calibrateOnEnable)
            {
                Calibrate(initialValue);
            }
        }

        public void OnScrollEnable()
        {
            if(SKCommonTimer.instance!=null)
                 SKCommonTimer.instance.CreateTimer("ScrollEnable");
        }
        private void OnScrollValueChanged(float value)
        {
            if (indicator0 != null && indicator1 != null)
            {
                if (!scrollBar.IsActive())
                {
                    indicator0.SetState(false);
                    indicator1.SetState(false);
                    return;
                }
                if (SKCommonTimer.instance.GetTimerValue("ScrollEnable") < 0.2f)
                {
                    indicator0.SetState(false);
                    indicator1.SetState(false);
                    return;
                }
                if (value <= 0.05f)
                {
                    indicator0.SetState(false);
                }
                else
                {
                    indicator0.SetState(true);
                }
                if (value >= 0.95f)
                {
                    indicator1.SetState(false);
                }
                else
                {
                    indicator1.SetState(true);
                }
            }
            if (hideWhenStatic)
            {
                ShowScrollBar();
                timer = 0;
            }
        }
        public void Calibrate(float target)
        {
            target = Mathf.Clamp01(target);
            if (reverse01)
                target = 1 - target;
            if (smoothCalibration)
            {
                StopAllCoroutines();
                StartCoroutine(CalibrateCR(target));
            }
            else
            {
                scrollBar.value = target;
            }
        }
        public void CalibrateToInitialValue()
        {
            if (smoothCalibration)
            {
                StopAllCoroutines();
                StartCoroutine(CalibrateCR(initialValue));
            }
            else
            {
                scrollBar.value = initialValue;
            }
        }
        public void CalibrateToItem(int id)
        {
            float unitLength = 1f / (CalculateConstraintItemCount() - 1);
            Calibrate(id * unitLength);
        }
        private IEnumerator CalibrateCR(float target)
        {
            for (int i = 0; i < 20; i++)
            {
                scrollBar.value = Mathf.Lerp(scrollBar.value, target, 0.2f);
                yield return new WaitForFixedUpdate();
            }
            scrollBar.value = target;
        }
        /// <summary>
        /// Scroll towards 1 in one unit
        /// </summary>
        public void OneUnit1()
        {
            float unitLength = 1f / (CalculateConstraintItemCount()-1);
            Calibrate(CalculateNearestUnit() + unitLength);
        }
        /// <summary>
        /// Scroll towards 0 in one unit
        /// </summary>
        public void OneUnit0()
        {
            float unitLength = 1f / (CalculateConstraintItemCount() - 1);
            Calibrate(CalculateNearestUnit() - unitLength);
        }

        private void ShowScrollBar()
        {
            if(scrollBarUIAnim)
            scrollBarUIAnim.SetState(true);
        }
        private void HideScrollBar()
        {
            scrollBarUIAnim.SetState(false);
        }

        public int CalculateConstraintItemCount()
        {
            if (isGridLayoutGroup)
                return layoutGroup.transform.childCount / (layoutGroup as GridLayoutGroup).constraintCount;
            else
                return layoutGroup.transform.childCount;
        }
        private float CalculateNearestUnit()
        {
            int unitCount = CalculateConstraintItemCount();
            float unitLength = 1f / (CalculateConstraintItemCount() - 1);

            float resValue=0, minDiff=1;
            for (int i = 0; i < unitCount; i++)
            {
                float diff = Mathf.Abs(scrollBar.value - i * unitLength);
                if (diff <= minDiff)
                {
                    resValue = i * unitLength;
                    minDiff = diff;
                }
            }
            return resValue;
        }

    }
}
