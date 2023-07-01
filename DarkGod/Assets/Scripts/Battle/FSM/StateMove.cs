using Battle.Entity;
using Common;
using PEProtocol;

namespace Battle.FSM
{
    public class StateMove : IState
    {
        public void Enter(EntityBase entity,params object[] args)
        {
            entity.currentAniState = AniState.Move;
        }

        public void Process(EntityBase entity,params object[] args)
        {
            entity.SetBlend(Constants.BlendMove);
        }

        public void Exit(EntityBase entity,params object[] args)
        {
        }
    }
}