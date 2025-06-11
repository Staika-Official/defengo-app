using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.GameData.Defense;
using Framework.Game.Defense;
using UniRx.Triggers;
using UniRx;
using DG.Tweening;
using Spine;
using Spine.Unity;

public class AnimationTest : MonoBehaviour
{
    public GameObject projectile;
    public float projectileSpeed;

    public ObjectParticle hitParticle;
    public ObjectParticle characterParticle;
    public ObjectParticle enemyParticle;

    public TestProjectile testProjectile;

    public SkeletonAnimation characterAnimation;
    public SkeletonAnimation enemyAnimation;

    void Start()
    {
        characterAnimation.AnimationState.SetAnimation(0, "Idle", true);
        enemyAnimation.AnimationState.SetAnimation(0, "Walk", true);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ => StartCoroutine(CharacterAnimation()));

        this.FixedUpdateAsObservable()
            .Where(_ => projectile.activeSelf)
            .Subscribe(_ => ProjectileSequence());

        hitParticle.transform.position = enemyAnimation.transform.position;
    }

    public IEnumerator CharacterAnimation()
    {
        TrackEntry entry = characterAnimation.AnimationState.SetAnimation(0, "Attack_FrontSide", false);
       
        yield return new WaitForSpineEvent(characterAnimation.AnimationState, "Attack");
        testProjectile.gameObject.SetActive(true);
        testProjectile.transform.position = characterAnimation.transform.position;
        testProjectile.isMove = true;
        //projectile.transform.position = characterAnimation.transform.position;
        //projectile.SetActive(true);

        yield return new WaitForSpineAnimationComplete(entry);

        characterAnimation.AnimationState.SetAnimation(0, "Idle", true);
    }

    public IEnumerator EnemyAnimation()
    {
        TrackEntry entry = enemyAnimation.AnimationState.SetAnimation(0, "Hit", false);

        yield return new WaitForSpineAnimationComplete(entry);

        enemyAnimation.AnimationState.SetAnimation(0, "Walk", true);
    }

    public void ProjectileSequence()
    {
        Vector3 target = enemyAnimation.transform.position;
        Vector3 moveDir = target - projectile.transform.position;

        float rotationZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(rotationZ - 90.0f, Vector3.forward);

        projectile.transform.Translate(projectileSpeed * Time.fixedDeltaTime * Vector2.up);

        if(Vector2.Distance(projectile.transform.position, enemyAnimation.transform.position) < 0.2f)
        {
            if (hitParticle != null)
            {
                hitParticle.SimplePlay();
            }
            StartCoroutine(EnemyAnimation());
            projectile.SetActive(false);
        }
    }
}
