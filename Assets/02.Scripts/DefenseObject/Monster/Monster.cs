using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Framework.GameData.Defense;
using Spine.Unity;
using Spine;
using DG.Tweening;
using Framework.Util;
using UnityEngine.Events;
using Framework.Network;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;
using System.Linq;

namespace Framework.Game.Defense
{
    public abstract class Monster : MonoBehaviour
    {
        [Header("Character Properties")]
        public ObscuredFloat maxHealth;
        public ObscuredFloat health;
        public ObscuredFloat speed;
        public ObscuredFloat currentSpeed;
        public ObscuredFloat defense;
        public ObscuredFloat getSlowDownValue;
        public bool isBoss;
        public MonsterType monsterType
        {
            get => _monsterType;
            set
            {
                _monsterType = value;
                if (_monsterType == MonsterType.BOSS_MONSTER || _monsterType == MonsterType.FIELD_BOSS_MONSTER)
                    Debug.Log($"{gameObject.name} set monsterType {_monsterType}");
            }
        }
        MonsterType _monsterType;

        public bool isFrozen;

        public SkeletonAnimation anim;
        public ObscuredInt waveIndex;
        public ObscuredInt indexInWave;
        public ObscuredInt CurrentArrayIdx { get; set; }
        public ObscuredInt prevPositionIdx;
        public ObscuredInt NextPositionIdx { get; set; }
        public bool IsMove;
        public bool IsStop;
        public bool isSlow = false;
        public bool IsHorizontal { get; set; }
        public ObscuredFloat DestinationValue { get; set; }
        public Vector2 nextCellPos;
        public Vector2 currentGridPos;
        public bool IsAlive;
        public MonsterInterface monsterInterface;
        public OrbStackInterface orbStackInterface;
        public List<Projectile> projectiles;

        public Color toxicColor;
        public Color monsterDefalutColor;

        public int electricCount;
        public Dictionary<string, ObjectParticle> dic_StateParticle = new();

        //todo::dongmin
        public IEnumerator deadTimerCoroutine;
        public readonly float deadTime = 5f;
        public bool IsBossAttack { get; set; }

        public abstract void Abillity();

        public abstract void EndOfUse();

        public abstract void DeathSequence();

        private void Awake()
        {
            ObscuredCheatingDetector.StartDetection(OnCheterDetected);
        }

        private void OnCheterDetected()
        {
            _ = NetworkManager.Instance.AbusingRecord("Cheat On Monster");
        }

        public virtual void FieldBossInitialize(BossData bossData) { }

        public void SetMonsterMove(SkeletonDataAsset animData, int currentArrayIdx, MonsterType Type = MonsterType.WAVE_MONSTER)
        {
            monsterType = Type;
            anim.SkeletonDataAsset.Clear();
            anim.AnimationState.ClearTracks();
            anim.skeletonDataAsset = animData;
            anim.initialSkinName = "default";
            anim.Initialize(true);

            ColorUtility.TryParseHtmlString("#7EFFAC", out toxicColor);
            monsterDefalutColor = anim.skeleton.GetColor();
            deadTimerCoroutine = null;

            CurrentArrayIdx = currentArrayIdx;
            CellData data = GridManager.Instance.GetCellData(CurrentArrayIdx);
            transform.localPosition = data.cellPosition;
            prevPositionIdx = data.cellIndex;
            CurrentArrayIdx++;
            DoMoveNextCell(CurrentArrayIdx);
            currentSpeed = speed * 0.52f;
            IsAlive = true;

            monsterInterface.transform.SetParent(UIManager.GetDynamicCanvasTransform());
            monsterInterface.transform.localScale = Vector3.one;
            monsterInterface.targetMonster = this;
            monsterInterface.transform.position = transform.localPosition;
            monsterInterface.Initialize(health);
            maxHealth = health;
            IsMove = true;
            IsStop = false;
            SetWalkSequence();
        }

        public void SetBossMove()
        {
            // monsterType = MonsterType.BOSS_MONSTER;
            IsBossAttack = false;
            CurrentArrayIdx = 0;
            CellData data = GridManager.Instance.GetCellData(CurrentArrayIdx);
            transform.localPosition = data.cellPosition;
            prevPositionIdx = data.cellIndex;
            CurrentArrayIdx++;
            DoMoveNextCell(CurrentArrayIdx);
            currentSpeed = speed * 0.52f;
            IsAlive = true;

            ColorUtility.TryParseHtmlString("#7EFFAC", out toxicColor);
            monsterDefalutColor = anim.skeleton.GetColor();
            deadTimerCoroutine = null;

            monsterInterface.transform.SetParent(UIManager.GetDynamicCanvasTransform());
            monsterInterface.transform.localScale = Vector3.one;
            monsterInterface.targetMonster = this;
            monsterInterface.transform.position = transform.localPosition;
            monsterInterface.Initialize(health);
            maxHealth = health;
            IsMove = true;
            IsStop = false;
            SetWalkSequence();
            Abillity();
        }

        public void SetMonsterMove(SkeletonDataAsset animData, MonsterType Type = MonsterType.WAVE_MONSTER)
        {
            monsterType = Type;
            anim.SkeletonDataAsset.Clear();
            anim.AnimationState.ClearTracks();
            anim.skeletonDataAsset = animData;
            anim.initialSkinName = "default";
            anim.Initialize(true);

            ColorUtility.TryParseHtmlString("#7EFFAC", out toxicColor);
            monsterDefalutColor = anim.skeleton.GetColor();
            deadTimerCoroutine = null;

            CurrentArrayIdx = 0;
            CellData data = GridManager.Instance.GetCellData(CurrentArrayIdx);
            transform.localPosition = data.cellPosition;
            prevPositionIdx = data.cellIndex;
            CurrentArrayIdx++;
            DoMoveNextCell(CurrentArrayIdx);
            //speed *= 0.52f;
            currentSpeed = speed * 0.52f;
            IsAlive = true;

            monsterInterface.transform.SetParent(UIManager.GetDynamicCanvasTransform());
            monsterInterface.transform.localScale = Vector3.one;
            monsterInterface.targetMonster = this;
            monsterInterface.transform.position = transform.localPosition;
            monsterInterface.Initialize(health);
            maxHealth = health;
            IsMove = true;
            IsStop = false;
            SetWalkSequence();
        }

        private void Start()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => IsMove && !IsStop)
                .Subscribe(_ => MoveSequence());
        }

        public void SetWalkSequence()
        {
            string animKey = isFrozen ? "Walk_Frozen" : "Walk";
            anim.AnimationState.SetAnimation(0, animKey, true);
        }

        public IEnumerator HitSequenceAsync(bool isDead, string animName)
        {
            TrackEntry entry = anim.AnimationState.SetAnimation(0, animName, false);

            if (isDead && monsterType != MonsterType.CLONE_MONSTER && monsterType != MonsterType.FIELD_BOSS_MONSTER)
            {
                ObjectParticle goParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("GoIconEffect");
                UIManager.Instance.goParticles.Add(goParticle);
                goParticle.isBossGo = isBoss;
                goParticle.SimplePlay(transform);
            }

            yield return new WaitForSpineAnimationComplete(entry);
            if (isDead)
            {
                DieMethod();
            }
            else
            {
                SetWalkSequence();
            }
        }

        public void HitDamage(float damage, DamageType damageType, float criticalDamageRate)
        {
            if (!IsAlive) return;
            // if (isCritical) damage *= ConfigData.CRITICAL_DAMAGE;
            if (damageType == DamageType.CRITICAL) damage *= criticalDamageRate;

            DamageText damageText = GameManager.Instance.objectPoolManager.GetObject<DamageText>("DamageText");
            damageText.transform.SetParent(UIManager.GetDynamicCanvasTransform());
            damageText.transform.localScale = Vector3.one;

            float x = Random.Range(-0.2f, 0.2f);

            Vector3 vec = new(transform.localPosition.x + x, transform.localPosition.y, 0);
            damageText.SetDamageText(damage, damageType, vec);

            health -= damage;
            health = Mathf.Clamp(health, 0f, health);
            monsterInterface.UpdateValue(health);

            if (health <= 0f)
            {
                IsAlive = false;
                IsMove = false;

                if (monsterType == MonsterType.WAVE_MONSTER)
                {
                    GameManager.Instance.monsterSpawner.killedMonsterCount++;
                    GameManager.Instance.monsterSpawner.totalKilledMonsterCount++;
                }
                else if (monsterType == MonsterType.BOSS_MONSTER)
                {
                    GameManager.Instance.monsterSpawner.killedBossMonsterCount++;
                    GameManager.Instance.monsterSpawner.totalKilledBossMonsterCount++;
                }

                //몬스터가 안죽는 이슈가있는경우의 방어코드 5초뒤 강제로 죽임
                deadTimerCoroutine = DeadTimeSequenceAsync();
                StartCoroutine(deadTimerCoroutine);

                string animKey = isFrozen ? "Die_Frozen" : "Die";
                StartCoroutine(HitSequenceAsync(true, animKey));
            }
            else
            {
                if (IsBossAttack) return;
                string animKey = isFrozen ? "Hit_Frozen" : "Hit";
                StartCoroutine(HitSequenceAsync(false, animKey));
            }
        }

        //todo:: Dongmin
        public IEnumerator DeadTimeSequenceAsync()
        {
            yield return new WaitForSeconds(deadTime);

            string animKey = isFrozen ? "Die_Frozen" : "Die";
            StartCoroutine(HitSequenceAsync(true, animKey));
        }

        public void DieMethod()
        {
            if (isBoss)
            {
                GameManager.Instance.monsterSpawner.killedBossLevel = waveIndex;
                // Debug.Log("Boss Level : " + waveIndex);
                GameManager.Instance.missionManager.EventMission(waveIndex);
                ++GameManager.Instance.bossKillCount;
            }


            switch (monsterType)
            {
                case MonsterType.WAVE_MONSTER:
                    GameManager.Instance.MonsterGemReward(waveIndex - 1, false, isBoss);
                    break;
                case MonsterType.CLONE_MONSTER:
                    break;
                case MonsterType.BOSS_MONSTER:
                    GameManager.Instance.MonsterGemReward(waveIndex - 1, false, isBoss);
                    break;
                case MonsterType.FIELD_BOSS_MONSTER:
                    Debug.Log("Field Monster Die~~~");
                    // 필드 보스 보상 팝업 생성 이벤트 
                    UIManager.Instance.fieldBossRewardPopup.ActivePopup();
                    break;
                default:
                    break;
            }

            EndOfUseMonster(false);
        }

        public void MoveSequence()
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, nextCellPos, currentSpeed * Time.fixedDeltaTime);

            float posValue = IsHorizontal ? transform.localPosition.x : transform.localPosition.y;

            if (Mathf.Approximately(posValue, DestinationValue))
            {
                IsMove = false;
                OnCompleteNextCell();
            }
        }

        public void DoMoveNextCell(int nextIdx)
        {
            CellData data = GridManager.Instance.GetCellData(nextIdx);
            NextPositionIdx = data.cellIndex;
            int value = prevPositionIdx - NextPositionIdx;
            IsHorizontal = Mathf.Abs(value) == 1;

            nextCellPos = data.cellPosition;

            DestinationValue = IsHorizontal ? nextCellPos.x : nextCellPos.y;
            IsMove = true;
            prevPositionIdx = NextPositionIdx;
        }

        public void OnCompleteNextCell()
        {
            int finalIdx = GameManager.Instance.monsterSpawner.pathLength;

            if (CurrentArrayIdx + 1 == finalIdx)
            {
                EndOfUseMonster(true);
            }
            else
            {
                CurrentArrayIdx++;
                DoMoveNextCell(CurrentArrayIdx);
            }
        }

        public IEnumerator EnterBossSequenceAsync()
        {
            IsAlive = false;
            IsMove = false;
            if (orbStackInterface != null)
            {
                orbStackInterface.ReturnObject();
                blackOrbCount = 0;
                orbStackInterface = null;
            }

            if (projectiles.Count > 0)
            {
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].ReturnObjectPool();
                }
            }

            if (dic_StateParticle.Count > 0)
            {
                foreach (var particle in dic_StateParticle.Keys.ToList())
                {
                    if (particle.Contains("ToxicMon"))
                    {
                        GameManager.Instance.objectPoolManager.ReturnObject(dic_StateParticle[particle], particle);
                        dic_StateParticle.Remove(particle);
                    }
                }
                anim.skeleton.SetColor(monsterDefalutColor);
            }

            projectiles.Clear();

            isFrozen = false;
            ObjectParticle objectParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("MonsterDisappear");
            objectParticle.transform.position = transform.localPosition;
            objectParticle.SimplePlay();

            TrackEntry entry = anim.AnimationState.SetAnimation(0, "Die", false);

            yield return new WaitForSpineAnimationComplete(entry);
            monsterInterface.transform.SetParent(transform);
            monsterInterface.ReturnObjectPool();
            GameManager.Instance.objectPoolManager.ReturnObject(objectParticle, "MonsterDisappear");
            //GameManager.Instance.monsterSpawner.RemoveMonster(this);
            //Debug.Log(GameManager.Instance.monsterSpawner.monsterCount);
            GameManager.Instance.objectPoolManager.ReturnObject(this, "Monster");
        }

        public void EnterBossWave()
        {
            Debug.Log("EnterBossWave");
            IsAlive = false;
            IsMove = false;
            if (orbStackInterface != null)
            {
                orbStackInterface.ReturnObject();
                blackOrbCount = 0;
                orbStackInterface = null;
            }

            if (projectiles.Count > 0)
            {
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].ReturnObjectPool();
                }
            }

            if (dic_StateParticle.Count > 0)
            {
                foreach (var particle in dic_StateParticle.Keys.ToList())
                {
                    if (particle.Contains("ToxicMon"))
                    {
                        GameManager.Instance.objectPoolManager.ReturnObject(dic_StateParticle[particle], particle);
                        dic_StateParticle.Remove(particle);
                    }
                }
                anim.skeleton.SetColor(monsterDefalutColor);
            }

            projectiles.Clear();

            isFrozen = false;
            ObjectParticle objectParticle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("MonsterDisappear");
            objectParticle.SimplePlay();
            //EndOfUse();
        }

        public void EndOfUseMonster(bool isFinish)
        {
            Debug.Log($"{gameObject.name} End Of Use");
            if (orbStackInterface != null)
            {
                orbStackInterface.ReturnObject();
                blackOrbCount = 0;
                orbStackInterface = null;
            }


            if (projectiles.Count > 0)
            {
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].ReturnObjectPool();
                }
            }

            if (dic_StateParticle.Count > 0)
            {
                foreach (var particle in dic_StateParticle.Keys.ToList())
                {
                    if (particle.Contains("ToxicMon"))
                    {
                        GameManager.Instance.objectPoolManager.ReturnObject(dic_StateParticle[particle], particle);
                        dic_StateParticle.Remove(particle);
                    }
                }
                anim.skeleton.SetColor(monsterDefalutColor);
            }

            projectiles.Clear();

            if (isFinish)
            {
                switch (monsterType)
                {
                    case MonsterType.WAVE_MONSTER:
                        GameManager.Instance.Damaged();
                        GameManager.Instance.MonsterGemReward(waveIndex - 1, false, isBoss);
                        break;
                    case MonsterType.CLONE_MONSTER:
                        GameManager.Instance.Damaged();
                        break;
                    case MonsterType.BOSS_MONSTER:
                        ++GameManager.Instance.bossKillCount;

                        GameManager.Instance.Damaged();
                        GameManager.Instance.MonsterGemReward(waveIndex - 1, false, isBoss);
                        break;
                    case MonsterType.FIELD_BOSS_MONSTER:
                        Debug.Log("Field Boss Game Over");
                        GameManager.Instance.FieldBossDamage();
                        break;
                    default:
                        break;
                }
            }
            isFrozen = false;
            EndOfUse();
        }

        public int blackOrbCount;

        public void BlackOrbStackSequence(float damage, DamageType damageType, float criticalDamageRate, float additiveDamage)
        {
            blackOrbCount++;

            if (orbStackInterface == null)
            {
                orbStackInterface = GameManager.Instance.objectPoolManager.GetObject<OrbStackInterface>("OrbStackInterface");
                orbStackInterface.Initialize(this);
            }

            orbStackInterface.HitMonster(blackOrbCount);
            if (blackOrbCount >= 4)
            {
                ExplosionBlackOrb(damage, additiveDamage);
                blackOrbCount = 0;
            }
            else
            {
                HitDamage(damage, damageType, criticalDamageRate);
            }
        }

        public void ExplosionBlackOrb(float damage, float additiveDamage)
        {
            ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("BlackHit2");
            particle.PlayParticle(transform.localPosition, 0);
            float totalDamage = damage + additiveDamage;
            HitDamage(totalDamage, DamageType.BLACKORB, 1);
        }

        public void ElectricSequence(bool isStart)
        {
            if (isStart)
            {
                electricCount++;
                if (electricCount == 1)
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("ElectroHit");
                    dic_StateParticle.Add("ElectroHit", particle);
                    particle.transform.SetParent(transform);
                    particle.transform.localPosition = Vector2.zero;
                }
            }
            else
            {
                electricCount--;

                if (electricCount <= 0)
                {
                    GameManager.Instance.objectPoolManager.ReturnObject(dic_StateParticle["ElectroHit"], "ElectroHit");
                    dic_StateParticle.Remove("ElectroHit");
                }
            }
        }

        public void ElectricDamage(float damage)
        {
            HitDamage(damage, DamageType.NORMAL, 0);
        }

        public IEnumerator SlowDownSeq;

        public void SlowDownSequence(float value, float durationOrRange, Projectile projectile = null, IceGround iceGround = null)
        {
            if (value >= getSlowDownValue)
            {
                if (projectile != null)
                {
                    SlowDownSeq = ExcuteSlowDown(value, durationOrRange, projectile);
                    StartCoroutine(SlowDownSeq);
                }
                else if (iceGround != null)
                {
                    SlowDownSeq = ExcuteSlowDown(value, durationOrRange, iceGround);
                    StartCoroutine(SlowDownSeq);
                }
                else if (iceGround == null && projectile == null)
                {
                    SlowDownSeq = ExcuteSlowDown(value, durationOrRange);
                    StartCoroutine(SlowDownSeq);
                }
            }
            else
            {
                if (projectile != null)
                {
                    projectile.ReturnObjectPool();
                }
                else
                {
                    return;
                }

            }
        }

        public IEnumerator ExcuteSlowDown(float value, float range, IceGround iceGround)
        {
            currentSpeed = (speed - (speed * value)) * 0.52f;
            isFrozen = true;

            if (anim.AnimationName == "Walk")
            {
                SetWalkSequence();
            }

            while (true)
            {
                Vector2 pos = Calculator.TransformationVector(prevPositionIdx);

                float distance = Calculator.DistanceCheck(pos - iceGround.positionVector);

                if (distance > range)
                {
                    isFrozen = false;
                    if (anim.AnimationName == "Walk_Frozen")
                    {
                        SetWalkSequence();
                    }
                    break;
                }

                yield return null;
            }

            currentSpeed = speed * 0.52f;
        }

        public IEnumerator ExcuteSlowDown(float value, float duration, Projectile projectile)
        {
            projectiles.Add(projectile);
            getSlowDownValue = value;
            currentSpeed = (speed - (speed * value)) * 0.52f;
            yield return new WaitForSeconds(duration);

            projectiles.Remove(projectile);
            projectile.ReturnObjectPool();
            currentSpeed = speed * 0.52f;
        }

        public IEnumerator ExcuteSlowDown(float value, float duration)
        {
            getSlowDownValue = value;
            currentSpeed = (speed - (speed * value)) * 0.52f;
            yield return new WaitForSeconds(duration);
            currentSpeed = speed * 0.52f;
        }


        public void ToxicSequence(bool isToxcosis)
        {
            if (isToxcosis)
            {
                if (!dic_StateParticle.ContainsKey("ToxicMon"))
                {
                    ObjectParticle particle = GameManager.Instance.objectPoolManager.GetObject<ObjectParticle>("ToxicMon");
                    particle.transform.SetParent(transform);
                    particle.transform.localPosition = Vector2.zero;
                    dic_StateParticle.Add("ToxicMon", particle);
                }

                anim.Skeleton.SetColor(toxicColor);
            }
            else
            {
                if (dic_StateParticle.ContainsKey("ToxicMon"))
                {
                    GameManager.Instance.objectPoolManager.ReturnObject(dic_StateParticle["ToxicMon"], "ToxicMon");
                    dic_StateParticle.Remove("ToxicMon");
                }

                anim.skeleton.SetColor(monsterDefalutColor);
            }
        }

        public IEnumerator arrowRainSequence(float damage, float addtiveDamage, float criticalRange, float criticalDamageRate)
        {
            while (IsAlive)
            {
                float totalDamage = 0f;
                totalDamage = damage + (maxHealth - health) * addtiveDamage;

                bool isCritical = false;
                if (criticalRange > 0)
                {
                    int criticalRangeValue = (int)(criticalRange * 100);
                    int RandomcriticalValue = Random.Range(0, 100);
                    isCritical = RandomcriticalValue <= criticalRangeValue;
                }

                DamageType damageType = isCritical ? DamageType.CRITICAL : DamageType.NORMAL;

                HitDamage(totalDamage, damageType, criticalDamageRate);

                yield return new WaitForSeconds(ConfigData.ATTACK_HIT_TIK_RATE);
            }
        }


        public IEnumerator OwlrusStormSequence(float damage, float addtiveDamage, DamageType damageType, float criticalDamageRate)
        {
            while (IsAlive)
            {
                float totalDamage = 0f;

                totalDamage = damage + maxHealth * addtiveDamage;

                HitDamage(totalDamage, damageType, criticalDamageRate);

                float hitTik = ConfigData.ATTACK_HIT_TIK_RATE * 0.5f;

                yield return new WaitForSeconds(hitTik);
            }
        }

        public IEnumerator BoomerangSequence(float damage, DamageType damageType, float criticalDamageRate, UnityAction<Transform> action)
        {
            while (IsAlive)
            {
                action?.Invoke(transform);

                HitDamage(damage, damageType, criticalDamageRate);

                yield return new WaitForSeconds(ConfigData.ATTACK_HIT_TIK_RATE);
            }
        }


        public void HammerSequence(float damage, float additiveDamage, DamageType damageType, float criticalDamageRate)
        {
            float totalDamage = 0f;

            totalDamage = Mathf.Floor(damage + health * additiveDamage);
            //Debug.Log($"Monster HP{health}");
            //Debug.Log($"hammering Per(최력비례댐) Damage{totalDamage}");
            HitDamage(totalDamage, damageType, criticalDamageRate);
        }

        //public void SlowDownSequence(bool isStart, float value, Projectile projectile)
        //{
        //    if(isStart)
        //    {
        //        projectiles.Add(projectile);

        //        getSlowDownValue += value;
        //        currentSpeed = (speed - (speed * getSlowDownValue)) * 0.52f;
        //        currentSpeed = Mathf.Clamp(currentSpeed, ConfigData.MONSTER_SPEED_LIMIT, 100f);
        //    }
        //    else
        //    {
        //        getSlowDownValue -= value;
        //        if(getSlowDownValue > 0)
        //        {
        //            currentSpeed = (speed - (speed * value)) * 0.52f;
        //        }
        //        else
        //        {
        //            currentSpeed = speed * 0.52f;
        //        }

        //        currentSpeed = Mathf.Clamp(currentSpeed, ConfigData.MONSTER_SPEED_LIMIT, 100f);
        //        projectiles.Remove(projectile);

        //        if (Mathf.Approximately(0, getSlowDownValue))
        //        {

        //        }
        //    }
        //}
    }
}
