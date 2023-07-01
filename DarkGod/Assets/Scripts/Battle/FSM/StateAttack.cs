using Battle.Entity;
using Common;
using Service;

namespace Battle.FSM
{
    /// <summary>
    /// 攻击状态
    /// </summary>
    public class StateAttack : IState
    {
        public void Enter(EntityBase entity, params object[] args)
        {
            entity.skMoveCBLst.Clear();
            entity.skActionCBLst.Clear();
            entity.curSkill = ResSvc.Instance.GetSkillCfgData((int)args[0]);
            entity.currentAniState = AniState.Attack;
        }

        public void Process(EntityBase entity, params object[] args)
        {
            if (entity.entityType == EntityType.Player)
            {
                entity.canRlsSkill = false;
            }

            entity.SkillAttack((int)args[0]);
        }

        public void Exit(EntityBase entity, params object[] args)
        {
            entity.ExitCurSkill();
        }
    }
}