using Battle.Entity;
using Common;
using Service;

namespace Battle.FSM
{
    public class StateDie : IState
    {
        public void Enter(EntityBase entity, params object[] args)
        {
            entity.currentAniState = AniState.Die;
            entity.RmvSkillCB();
        }

        public void Process(EntityBase entity, params object[] args)
        {
            entity.SetAction(Constants.ActionDie);
            if (entity.entityType == EntityType.Monster)
            {
                entity.SetCharacterCotoller(false);
                TimerSvc.Instance.AddTimeTask((tid) => { entity.SetActive(false); }, Constants.DieAniLength);
            }
        }

        public void Exit(EntityBase entity, params object[] args)
        {
        }
    }
}