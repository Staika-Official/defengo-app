using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using UniRx;
using UniRx.Triggers;

public enum ProjectileType
{
    EXPLOSION,
    NONE_EXPLOSION
}

namespace Framework.Game.Defense
{
    public class TestProjectile : MonoBehaviour
    {
        public Monster targetMonster;
        public bool isMove;

        public float moveSpeed;
        public float explosionRange;
        public float distance;
        public ObjectParticle objectParticle;

        private void Start()
        {
            //this.UpdateAsObservable()
            //    .Where(_ => isMove)
            //    .Subscribe(_ => TrackingTarget());
        }

        public void TrackingTarget()
        {
            Vector3 targetPos = targetMonster.transform.position;
            Vector3 moveDir = targetPos - transform.position;

            moveDir.Normalize();
            float rotationZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(rotationZ - 90.0f, Vector3.forward);

            transform.Translate(moveSpeed * Time.deltaTime * Vector2.up);

            distance = Vector2.Distance(targetPos, transform.position);
            if(distance < 0.2f)
            {
                ProjectileAction();
            }
        }

        public void ProjectileAction()
        {
            objectParticle.SimplePlay();
        }
    }
}
