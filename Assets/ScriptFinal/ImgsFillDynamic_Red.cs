using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgsFillDynamic_Red : MonoBehaviour
{
    float BeforeValue = 0F;
    float TargetValue = 0F;

    float speed = 1F;
    float nowTime = 0F;

    public Image[] ImgFacterTarget;
    public FillRound FillRound;
    float facter = 0F;
    public float Factor { get { return this.facter; } }

    public Color[] PercentByColor = {
        new Color(1f, 0.2f, 0.2f), // 밝은 빨간색
        new Color(0.8f, 0.1f, 0.1f),
        new Color(0.6f, 0.05f, 0.05f),
        new Color(0.4f, 0.02f, 0.02f), // 어두운 빨간색
    };

    public bool IsUsingMaxColor = false;
    public Color MaxColor = Color.red; // 이 설정은 필요 없을 수도 있습니다.

    public Text TxtValue;
    public float MultifyText = 100F;
    public string TailText = "%";

    /// <summary>
    /// USAGE: Set SetFillAmount
    /// </summary>
    /// <param name="_value">Target Value (0F ~ 1F)</param>
    /// <param name="_isDirectly">Is Directly Fill</param>
    /// <param name="_duringSpeed">Fill Speed 0.5(2sec), 2(0.5sec)</param>
    public void SetValue(float _value, bool _isDirectly = false, float _duringSpeed = 1F)
    {
        Mathf.Clamp01(_value);
        this.TargetValue = _value;

        if (_isDirectly)
        {
            this.nowTime = 1F;
            this.SetImageFillAmount(_value);
            this.SetImageColor();
            return;
        }
        else
        {
            this.nowTime = 0F;
            this.speed = _duringSpeed;
        }
    }

    void Update()
    {
        if (this.ImgFacterTarget != null && nowTime > -1F)
        {
            nowTime += Time.deltaTime * this.speed;
            this.facter = Mathf.Lerp(this.BeforeValue, this.TargetValue, nowTime);
            this.SetImageFillAmount(this.facter);
            this.BeforeValue = this.facter;
            if (this.FillRound != null)
                this.FillRound.SetFill(this.facter);

            if (nowTime < 0F)
            {
                nowTime = -1F;
                this.facter = this.TargetValue;
                this.SetImageFillAmount(this.facter);
                if (this.FillRound != null)
                    this.FillRound.SetFill(this.facter);
            }

            this.SetTextFactor();
            this.SetImageColor();
        }
    }

    void SetTextFactor()
    {
        float textFactor = Mathf.Clamp01(this.facter) * this.MultifyText;
        if (this.TxtValue != null)
            this.TxtValue.text = string.Format("{0}{1}", textFactor.ToString("0"), this.TailText);
    }

    void SetImageFillAmount(float facter)
    {
        for (int i = 0; i < this.ImgFacterTarget.Length; i++)
            this.ImgFacterTarget[i].fillAmount = facter;
    }

    void SetImageColor()
    {
        if (this.PercentByColor.Length < 1) return;

        int arr = (int)Mathf.Lerp(0, this.PercentByColor.Length, this.facter);
        if (arr >= this.PercentByColor.Length)
            arr = this.PercentByColor.Length - 1;
        else if (arr < 0) arr = 0;
        for (int i = 0; i < this.ImgFacterTarget.Length; i++)
            this.ImgFacterTarget[i].color = this.PercentByColor[arr];
    }
}
