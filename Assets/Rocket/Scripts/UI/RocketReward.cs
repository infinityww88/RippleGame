using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;
using UnityEngine.Assertions;

public class RocketReward : MonoBehaviour
{
	public RectTransform card;
	public RectTransform item;
	public float appearDuration = 1f;
	public float shootDuration = 1f;
	public float rotateCircleNum = 3;
	public float gap = 1f;
	public ParticleSystem particleSystem;
	public TMPro.TextMeshProUGUI rocketNumLabel;
	private int rocketNum = 0;

	public int RocketNum => rocketNum;
	
    // Start is called before the first frame update
    void Start()
    {
	    rocketNum = ES3.Load<int>("rocket_num", 20);
	    SetRocketNum(rocketNum);
    }
    
	public void Use1Rocket() {
		Assert.IsTrue(rocketNum > 0);
		SetRocketNum(rocketNum - 1);
		SaveRocketNum();
	}
    
	void SetRocketNum(int num) {
		rocketNum = num;
		rocketNumLabel.text = $"x {num}";
	}
	
	void SaveRocketNum() {
		ES3.Save("rocket_num", rocketNum);
	}
	
	[Button]
	void WriteRocketNum(int num) {
		SetRocketNum(num);
		SaveRocketNum();
	}
    
	[Button]
	public void Play() {
		card.DOKill();
		card.gameObject.SetActive(true);
		card.localScale = Vector3.zero;
		card.localRotation = Quaternion.identity;
		card.localPosition = Vector3.zero;
		var scaleTween = card.DOScale(1, appearDuration).SetEase(Ease.OutBack);
		var rotateTween = card.DOLocalRotate(new Vector3(0, 360 * rotateCircleNum, 0), appearDuration, RotateMode.FastBeyond360);
		var scaleTween1 = card.DOScale(0, shootDuration);
		var translateTween = card.DOMove(item.transform.position, shootDuration);
		var seq = DOTween.Sequence().Append(scaleTween)
			.Insert(0, rotateTween)
			.AppendInterval(gap)
			.Append(translateTween)
			.Insert(appearDuration + gap, scaleTween1)
			.SetTarget(gameObject)
			.AppendCallback(() => {
				card.gameObject.SetActive(false);
				particleSystem.Emit(12);
				SetRocketNum(rocketNum + 10);
				SaveRocketNum();
			});
	}
}
