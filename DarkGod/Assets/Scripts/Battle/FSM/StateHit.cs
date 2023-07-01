using Battle.Entity;
using Common;
using Service;
using UnityEngine;

namespace Battle.FSM
{
    public class StateHit : IState
    {
        public void Enter(EntityBase entity, params object[] args)
        {
            entity.currentAniState = AniState.Hit;
            entity.RmvSkillCB();
        }

        public void Process(EntityBase entity, params object[] args)
        {
            if (entity.entityType == EntityType.Player)
            {
                entity.canRlsSkill = false;
            }

            entity.canControl = false;
            entity.SetDir(Vector2.zero);
            entity.SetAction(Constants.ActionHit);

            if (entity.entityType == EntityType.Player)
            {
                AudioSource audio = entity.GetAudio();
                AudioSvc.Instance.PlayCharAudio(Constants.AssassinHit, audio);
            }

            TimerSvc.Instance.AddTimeTask((tid) =>
            {
                entity.canControl = true;
                entity.SetAction(Constants.ActionIdle);
                entity.Idle();
            }, (int)(getHitAniLen(entity) * 1000));
        }

        public void Exit(EntityBase entity, params object[] args)
        {
        }

        private float getHitAniLen(EntityBase entity)
        {
            AnimationClip[] clips = entity.GetAniClips();
            foreach (var clip in clips)
            {
                if (clip.name.ToLower().Contains("hit"))
                {
                    return clip.length;
                }
            }

            return 1;
        }
    }
}