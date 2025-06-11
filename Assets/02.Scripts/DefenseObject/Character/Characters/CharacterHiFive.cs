using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Util;
using Framework.Sound;
using UniRx.Triggers;
using UniRx;
using Spine.Unity;
using Spine;

namespace Framework.Game.Defense
{
    public class CharacterHiFive : Character
    {
        public enum HifiveDirection
        {
            LEFT = 0,
            RIGHT = 1,
            UP = 2,
            DOWN = 3
        }

        public bool isChange = false;

        public float addtiveAttack;
        public float addtiveDetectRange;

        public Dictionary<HifiveDirection, CharacterHiFive> findCharacters = new();
        public Stack<CharacterHiFive> stack = new();
        public List<CharacterHiFive> visited = new();

        public int connectionCount = 0;

        //하이파이브가 연결되어 상태가 변하기 시작하는 숫자
        public readonly int connectionActiveCount = 5;

        //-3 : 좌, 3 : 우, -60 : 위 , 60 : 아래
        public readonly int[] glacierDirIndex = { -3, 3, -60, 60 };

        public IEnumerator attackActionCoroutine;
        public IEnumerator nonAttackCoroutine;
        
        void Start()
        {
            this.UpdateAsObservable()
               .Where(_ => isSummoned && isAttackType)
               .Subscribe(_ => GetPossibleAttackMonster());
        }

        public override IEnumerator ActionSequence()
        {
            characterState = CharacterState.ATTACK;
            FlipCharacter(targetedMonster[0].transform.localPosition.x);

            string attackKey = isChange ? "Attack_FrontSide_Change" : "Attack_FrontSide";

            TrackEntry entry = anim.AnimationState.SetAnimation(0, attackKey, false);
            
            List<Monster> targetCache = targetedMonster;
            float duration = entry.Animation.Duration;
            float tempSpeed = speedBuffValue == 0 ? attackSpeed : attackSpeed * speedBuffValue;
            entry.TimeScale = Calculator.GetAnimationSpeed(tempSpeed, duration);
            
            yield return new WaitForSpineEvent(anim.AnimationState, "Attack");

            SoundManager.Instance.PlaySound(SoundKey.SF_HIFIVE_ATTACK);
            
            bool isCritical;
            if (criticalRange <= 0)
            {
                isCritical = false;
            }
            else
            {
                int tempValue = (int)(criticalRange * 100);
                int criticalValue = Random.Range(0, 100);
                isCritical = criticalValue <= tempValue;
            }
            
            
            for (int i = 0; i < targetCache.Count; i++)
            {
                string changeProjectileName = isChange ? "BeeStingProjectile_Change" : "BeeStingProjectile";
            
                BeeStingProjectile projectile = GameManager.Instance.objectPoolManager.GetObject<BeeStingProjectile>(changeProjectileName);
                projectile.transform.localPosition = transform.localPosition;
            
                float damage = totalDamage * addtiveAttack;
                projectile.attackValue = damage;
                projectile.criticalDamageRate = criticalDamageRate;
                projectile.isMove = true;
                projectile.provokedIndex = glacierIdx;
            
                projectile.Initialize(this, targetCache[i], isCritical, projectileSpeed, strikeRange, particleName);
            }

            //방어코드
            nonAttackCoroutine = NonAttackDefensiveHiFive(entry);
            StartCoroutine(nonAttackCoroutine);
            
            if (!entry.IsComplete)
            {
                yield return new WaitForSpineAnimationComplete(entry);
            }
            
            if (null != nonAttackCoroutine)
            {
                StopCoroutine(nonAttackCoroutine);
                nonAttackCoroutine = null;
            }
            
            entry.TimeScale = 1;
            string idleKey = isChange ? "Idle_Change" : "Idle";
            anim.AnimationState.SetAnimation(0, idleKey, true);
            characterState = CharacterState.DETECT;
            actionCoroutine = null;
        }

        public IEnumerator NonAttackDefensiveHiFive(TrackEntry entry)
        {
            yield return new WaitForSeconds(3f);
            
            entry.TimeScale = 1;
            string idleKey = isChange ? "Idle_Change" : "Idle";
            anim.AnimationState.SetAnimation(0, idleKey, true);
            characterState = CharacterState.DETECT;
            
            if (null != attackActionCoroutine)
            {
                StopCoroutine(attackActionCoroutine);
                attackActionCoroutine = null;
            }
        }
        
        
        public override void SetCharacterInfo()
        {
            characterState = CharacterState.APPEAR;
            
            if (targetedMonster.Count != 0)
            {
                targetedMonster.Clear();
            }

            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
                actionCoroutine = null;
            }

            endOfUse = () =>
            {
                DisconnectHiFive();
                findCharacters.Clear();
                connectionCount = 1;
            };

            findCharacters.Clear();

            isChange = false;

            upgradeIndex = GameManager.Instance.characterSpawner.GetCharacterLevel(characterIndex);
            CharacterAction = null;

            isAttackType = true;
            targetCount = (int)characterData.targetCount;

            float attackDamage = characterData.attackDamage + (characterData.characterClassLevel - 1) * characterData.classUpFactor[0];

            basicAttackDamage = attackDamage;
            attackSpeed = characterData.attackSpeed;
            criticalRange = characterData.criticalRange + GameManager.Instance.buffManager.increaseCriticalRate;
            criticalDamageRate = characterData.criticalDamageRate;
            upgradeFactor = characterData.powerFactor[0];
            projectileSpeed = characterData.projectileSpeed;
            attackRange = Mathf.Pow(characterData.detectRange, 2);
            strikeRange = Mathf.Pow(characterData.strikeRange, 2);
            isThrow = characterData.isThrow;
            buffDuration = characterData.buffDuration;
            buffValue = characterData.buffValue;
            SetAttackDamage();
            projectileName = characterData.projectileName;

            CharacterAction = () =>
            {
                if (isSummoned && targetedMonster.Count > 0)
                {
                    if (characterState == CharacterState.DETECT)
                    {
                        if (null != attackActionCoroutine)
                        {
                            StopCoroutine(attackActionCoroutine);
                            attackActionCoroutine = null;
                        }
                        
                        attackActionCoroutine = ActionSequence();
                        StartCoroutine(attackActionCoroutine);
                    }
                }
            };

            particleName = "YellowHit";
            maxStarGradeValue = characterData.maxStarGradeValue;
            isSummoned = true;
            connectionCount = 1;
            
            if (characterData.characterType != CharacterType.SUB)
            {
                GameManager.Instance.missionManager.AddOrRemoveCharacter(this, true);
            }

            addtiveDetectRange = characterData.characterUniqueValue[1];

            addtiveAttack = 1 + (connectionCount - 1)
                    * (characterData.characterUniqueValue[0]
                    + (characterData.characterClassLevel - 1)
                    * characterData.classUpFactor[1]);

            bool isAppear = starGradeIndex == 0;

            AppearInit(isAppear);
        }

        public override void SynthesisCharacter()
        {
        }

        public override void UpgradeLevel(int upgradeIndex)
        {
            this.upgradeIndex = upgradeIndex;
            SetAttackDamage();
        }

        public override void Reposition(Glacier glacier)
        {
            DisconnectHiFive();
            findCharacters.Clear();
            connectionCount = 0;
            ChangeActionSequence();

            base.Reposition(glacier);

            StartCoroutine(HiFiveReposition());
        }

        public void AppearInit(bool isAppear)
        {
            if (isAppear)
            {
                ConnectionHiFive();
                AppearAction(ChangeIdle, isAppear);
            }
            else
            {
                StartCoroutine(HiFiveMarge());
            }
        }

        public void SetHiFiveCharacter()
        {
            if (!isSummoned) return;

            Vector2 pos = Calculator.TransformationVector(constructibleIdx);

            //현재 소환된 캐릭터 순회
            for (int i = 0; i < GameManager.Instance.characterSpawner.summonedCharacters.Count; i++)
            {
                Character character = GameManager.Instance.characterSpawner.summonedCharacters[i];
                CharacterHiFive hifive = character as CharacterHiFive;

                if (hifive != null && hifive != this)
                {
                    float distance = Calculator.DistanceCheck(pos - character.currentGridPos);

                    if (distance <= 16)
                    {
                        ConnectionHiFiveCharacter(hifive);
                    }
                }
            }
        }

        public void ConnectionHiFive()
        {
            SetHiFiveCharacter();
            FindConnectionHiFive();
            GetConnectionCharacter();
        }

        public IEnumerator HiFiveReposition()
        {
            yield return new WaitForEndOfFrame();
            ConnectionHiFive();
        }
        
        public IEnumerator HiFiveMarge()
        {
            yield return new WaitForEndOfFrame();
            ConnectionHiFive();
            AppearAction(ChangeIdle, false);
        }

        public void ChangeIdle()
        {
            string idleKey = isChange ? "Idle_Change" : "Idle";
            anim.AnimationState.SetAnimation(0, idleKey, true);
            
            actionCoroutine = WaitActionSequence();
            StartCoroutine(actionCoroutine);
        }
        
        public bool GlacierDirectionCheck(int selectHifiveIdx, int myHifiveIdx)
        {
            //현재 선택된 캐릭터의 빙하인덱스와 내가바라보는 빙하 방향인덱스와 동일하다면 true 아니라면 false
            bool isValid = false;

            if(selectHifiveIdx == myHifiveIdx)
            {
                isValid = true;
            }

            return isValid;
        }

        public void AddConnection(int index, CharacterHiFive character)
        {
            if (findCharacters.ContainsKey((HifiveDirection)index)) return;

            findCharacters.Add((HifiveDirection)index, character);
        }

        public void RemoveConnection(int index)
        {
            findCharacters.Remove((HifiveDirection)index);
        }

        public void SetDetectRange()
        {
            //사거리 변화 함수
            float detectRange = 0f;

            if (isChange)
            {
                detectRange = characterData.detectRange + addtiveDetectRange;
            }
            else
            {
                detectRange = characterData.detectRange;
            }

            attackRange = Mathf.Pow(detectRange, 2);
        }

        public void ChangeActionSequence()
        {
            //함수 의도 : 현재 5개이상의 캐릭터가 연결되면 상태변화가 있어야하기에
            //여기함수에서 처리를해줌 (애니메이션 변화, 공격력 처리, 사거리 변화)

            if(connectionCount >= connectionActiveCount)
            {
                isChange = true;
            }
            else
            {
                isChange = false;
            }

            if (characterState == CharacterState.DETECT)
            {
                string idleKey = isChange ? "Idle_Change" : "Idle";
                anim.AnimationState.SetAnimation(0, idleKey, true);
            }

            addtiveAttack = 1 + (connectionCount - 1)
                    * (characterData.characterUniqueValue[0]
                    + (characterData.characterClassLevel - 1)
                    * characterData.classUpFactor[1]);

            SetDetectRange();
        }

        public void FindConnectionHiFive()
        {
            //현재 주변에 캐릭터에게 나를 연결시켜달라고하는 함수
            foreach (var character in findCharacters)
            {
                findCharacters[character.Key].ConnectionHiFiveCharacter(this);
            }
        }

        
        public void ConnectionHiFiveCharacter(CharacterHiFive character)
        {
            //주변의 캐릭터를 순회하며 파라미터로 들어온 캐릭터를 연결시킴
            for (int j = 0; j < glacierDirIndex.Length; ++j)
            {
                if (GlacierDirectionCheck(character.constructibleIdx, constructibleIdx + glacierDirIndex[j]))
                {
                    AddConnection(j, character);
                }
            }
        }

        public void DisconnectionHiFiveCharacter(CharacterHiFive character)
        {
            //주변의 캐릭터를 순회하며 파라미터로 들어온 캐릭터를 연결해제
            for (int j = 0; j < glacierDirIndex.Length; ++j)
            {
                if (GlacierDirectionCheck(character.constructibleIdx, constructibleIdx + glacierDirIndex[j]))
                {
                    RemoveConnection(j);
                }
            }
        }


        public void DisconnectHiFive()
        {
            //함수의도 : 주변에 캐릭터에게 나와의 연결을 지워주고
            //새로운 연결을 만들어내기 위함

            //현재 캐릭터 주변캐릭터에게 나의 연결을 해제해줌 
            foreach (var character in findCharacters)
            {
                findCharacters[character.Key].DisconnectionHiFiveCharacter(this);
            }

            //주변 캐릭터의 주변에캐릭터를 순회하면서 새로운 연결을 만들어줌
            foreach (var character in findCharacters)
            {
                CharacterHiFive findhifive = findCharacters[character.Key];

                stack.Push(findhifive); // 현재 캐릭터부터 시작

                while (stack.Count > 0)
                {
                    CharacterHiFive hifive = stack.Pop();

                    // 이미 방문한 캐릭터는 건너뛰기
                    // 방문한 캐릭터 수를 세면 안되기에
                    if (visited.Contains(hifive))
                    {
                        continue;
                    }
                    visited.Add(hifive);

                    //주변에 캐릭터 순회
                    foreach (var connectCharacter in hifive.findCharacters)
                    {
                        stack.Push(connectCharacter.Value);
                        // 연결된 캐릭터를 스택에 추가
                        // 이유 : 스택에 추가해서 팝하면서 주변 하이파이브를 탐색 할 수 있기에
                    }
                }

                //방문한 캐릭터 갯수를 알면 연결된 캐릭터 수도 알 수 있다
                foreach (var visitCharacter in visited)
                {
                    visitCharacter.connectionCount = Mathf.Max(1, visited.Count);
                    visitCharacter.ChangeActionSequence();
                }

                ConnectionActivateSound();
                visited.Clear();
            }

            visited.Clear();
        }

        
        public void GetConnectionCharacter()
        {
            stack.Push(this); // 현재 캐릭터부터 시작

            while (stack.Count > 0)
            {
                CharacterHiFive hifive = stack.Pop();

                // 이미 방문한 캐릭터는 건너뛰기
                // 방문한 캐릭터 수를 세면 안되기에
                if (visited.Contains(hifive))
                {
                    continue;
                }
                visited.Add(hifive);

                //주변에 캐릭터 순회
                foreach (var connectCharacter in hifive.findCharacters)
                {
                    stack.Push(connectCharacter.Value);
                    // 연결된 캐릭터를 스택에 추가
                    // 이유 : 스택에 추가해서 팝하면서 주변 하이파이브를 탐색 할 수 있기에
                }
            }

            //방문한 캐릭터 갯수를 알면 연결된 캐릭터 수도 알 수 있음
            //방문한 캐릭터를 순회하며 연결된 캐릭터 수와 애니메이션 변경
            foreach (var visitCharacter in visited)
            {
                visitCharacter.connectionCount = Mathf.Max(1, visited.Count);
                visitCharacter.ChangeActionSequence();
            }

            ConnectionActivateSound();

            visited.Clear();
        }

        public void ConnectionActivateSound()
        {
            if (isChange)
            {
                SoundManager.Instance.PlaySound(SoundKey.SF_HIFIVE_ACTIVATED);
            }
        }

    }
}
