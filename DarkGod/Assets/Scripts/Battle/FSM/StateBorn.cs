using Battle.Entity;
using Common;

namespace Battle.FSM
{
    public class StateBorn : IState
    {
        public void Enter(EntityBase entity, params object[] args)
        {
            entity.currentAniState = AniState.Born;
        }

        public void Process(EntityBase entity, params object[] args)
        {
            entity.SetAction(Constants.ActionBorn);
        }

        public void Exit(EntityBase entity, params object[] args)
        {
            entity.SetAction(Constants.ActionIdle);
        }
    }
}