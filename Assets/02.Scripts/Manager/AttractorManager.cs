using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine.Events;
using Framework.GameData.Defense;

namespace Framework.UI
{
    public class AttractorManager : MonoBehaviour
    {
        public SerializableDictionary<string, ParticleImage> dic_Attractor;
        public static AttractorManager Instance;

        public GameObject invisibleDim;

        public Transform trans_InActiveContainer;
        public Transform trans_ActiveContainer;

        public Dictionary<string, Queue<ParticleImage>> dic_queue_Attractor = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            foreach (var item in dic_Attractor)
            {
                Queue<ParticleImage> queue = new();
                queue.Enqueue(item.Value);

                dic_queue_Attractor.Add(item.Key, queue);
            }
        }

        public ParticleImage GetAttractor(string key)
        {
            if (dic_queue_Attractor[key].Count == 0)
            {
                GameObject obj = Instantiate(dic_Attractor[key].gameObject);
                ParticleImage attractor = obj.GetComponent<ParticleImage>();
                attractor.transform.SetParent(trans_ActiveContainer);
                return attractor;
            }
            else
            {
                ParticleImage attractor = dic_queue_Attractor[key].Dequeue();
                attractor.transform.SetParent(trans_ActiveContainer);
                return attractor;
            }
        }

        public void ReturnAttractor(string key, ParticleImage attractor)
        {
            attractor.transform.SetParent(trans_InActiveContainer);
            dic_queue_Attractor[key].Enqueue(attractor);
        }

        public void FirstParticleActionSequence(ParticleImage attractor, bool invisibleInActive, UnityAction onStartAttractor)
        {
            //매개변수 소개
            //1. 어트렉터
            //2. 화면 막기 비활성화 시켜줄지 말지 (first 와 last액션이 있어서 어느시점에 풀어줄지 정하는것)
            //3. 외부에서 액션 실행 null로 들어오면 미호출임

            attractor.onFirstParticleFinish.RemoveAllListeners();

            attractor.onFirstParticleFinish.AddListener(() =>
            {
                if (invisibleInActive)
                {
                    invisibleDim.SetActive(false);
                }

                onStartAttractor?.Invoke();
            });
        }

        public void LastParticleActionSequence(string key, ParticleImage attractor, bool invisibleInActive, UnityAction onLastAttractor = null)
        {
            attractor.onLastParticleFinish.RemoveAllListeners();

            attractor.onLastParticleFinish.AddListener(() =>
            {
                if (invisibleInActive)
                {
                    invisibleDim.SetActive(false);
                }

                onLastAttractor?.Invoke();

                ReturnAttractor(key, attractor);
            });
        }

        public void SetAttractor(string key, Transform setStartPosition, UnityAction onAttractorAction = null, Transform setTargetPosition = null)
        {
            //매개변수 소개
            //1. 딕셔너리에 바인딩되어있는 어트렉터를 찾는 key (로비씬에있는 어트렉터 매니저에서 확인가능)
            //2. 첫 포지션을 셋팅해주는 트렌스폼 null로 들어오면 포지션 셋팅안해주고 인스펙터에 저장된 포지션 이용 근대 일단 무조껀 트랜스폼을 받아서 어디 위치에 생성할지 강제성 부여
            //3. 외부에서 액션 실행 디폴트 인자로 들어오면 미호출임
            //4. 외부 인스펙터에 연결된 어트렉터 타겟이없으면 셋해주는 함수
            
            invisibleDim.SetActive(true);

            //컨테이너에서 꺼내옴
            ParticleImage attractor = GetAttractor(key);

            //포지션 설정
            SetAttractorPosition(attractor, setStartPosition);

            //처음 파티클이 죽을때 콜백해주는 함수 
            FirstParticleActionSequence(attractor, true, onAttractorAction);

            //지정된 타겟이없으면 셋해주는 함수
            SetAttractorTarget(attractor, setTargetPosition);
            
            //마지막으로 파티클이 죽을 때 콜백해주는 함수 (First는 안써줘도 되지만 last는 무조껀 써줘야함 풀링 반환이 있기에)
            LastParticleActionSequence(key, attractor, false);

            attractor.gameObject.SetActive(true);
        }

        public void SetAttractor_Late(string key, Transform setStartPosition, UnityAction onAttractorAction = null, Transform setTargetPosition = null)
        {
            invisibleDim.SetActive(true);

            ParticleImage attractor = GetAttractor(key);

            SetAttractorPosition(attractor, setStartPosition);
            
            SetAttractorTarget(attractor, setTargetPosition);

            LastParticleActionSequence(key, attractor, true, onAttractorAction);

            attractor.gameObject.SetActive(true);
        }

        public void SetAttractor_StartAction_LateInteractable(string key, Transform setStartPosition, UnityAction onAttractorAction = null, Transform setTargetPosition = null)
        {
            invisibleDim.SetActive(true);

            ParticleImage attractor = GetAttractor(key);

            SetAttractorPosition(attractor, setStartPosition);

            FirstParticleActionSequence(attractor, false, onAttractorAction);

            SetAttractorTarget(attractor, setTargetPosition);
            
            LastParticleActionSequence(key, attractor, true);

            attractor.gameObject.SetActive(true);
        }

        public void SetHatchingAttractor(string key, string itemType, Transform setStartPosition, UnityAction onAttractorAction = null, Transform setTargetPosition = null)
        {
            invisibleDim.SetActive(true);

            ParticleImage attractor = GetAttractor(key);

            SetAttractorPosition(attractor, setStartPosition);

            attractor.texture = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[itemType].texture_Attactor;

            FirstParticleActionSequence(attractor, true, onAttractorAction);

            SetAttractorTarget(attractor, setTargetPosition);
            
            LastParticleActionSequence(key, attractor, false);

            attractor.gameObject.SetActive(true);
        }

        public void SetHatchingAttractor_StartAction_LateInteractable(string key, string itemType, Transform setStartPosition, UnityAction onAttractorAction = null, Transform setTargetPosition = null)
        {
            invisibleDim.SetActive(true);

            ParticleImage attractor = GetAttractor(key);

            SetAttractorPosition(attractor, setStartPosition);

            attractor.texture = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[itemType].texture_Attactor;

            FirstParticleActionSequence(attractor, false, onAttractorAction);

            SetAttractorTarget(attractor, setTargetPosition);
            
            LastParticleActionSequence(key, attractor, true);

            attractor.gameObject.SetActive(true);
        }

        public void SetHatchingAttractor_Late(string key, string itemType, Transform setStartPosition, Transform setTargetPosition, UnityAction onAttractorAction = null)
        {
            invisibleDim.SetActive(true);

            ParticleImage attractor = GetAttractor(key);

            SetAttractorPosition(attractor, setStartPosition);

            attractor.texture = DataManager.Instance.uiPropertyData.dic_HatchingStoneSprite[itemType].texture_Attactor;

            SetAttractorTarget(attractor, setTargetPosition);

            LastParticleActionSequence(key, attractor, true, onAttractorAction);

            attractor.gameObject.SetActive(true);
        }

        public void SetAttractorPosition(ParticleImage attractor, Transform transform)
        {
            if (null != transform)
            {
                attractor.transform.SetParent(transform);
                attractor.transform.localPosition = Vector3.zero;
                attractor.transform.SetParent(trans_ActiveContainer);
            }
        }

        public void SetAttractorTarget(ParticleImage attractor, Transform setTargetPosition)
        {
            if (setTargetPosition != null)
            {
                attractor.attractorTarget = setTargetPosition;
                DOTweenAnimation anim = setTargetPosition.GetComponent<DOTweenAnimation>();
                if (null != anim)
                {
                    attractor.onParticleFinish.RemoveAllListeners();
                    
                    attractor.onParticleFinish.AddListener(() =>
                    {
                        anim.DORestart();
                    });
                }
            }
        }
        
    }
}