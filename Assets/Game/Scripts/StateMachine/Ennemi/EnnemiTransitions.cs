using EnnemiState;
using UnityEngine;

///// TRANSITIONS
namespace EnnemiTransition
{
    public class AttackToFollow: ITransition
    {
        public FollowTransform followTransform;
        public WeaponScript weapon;
    
        public override bool Test()
        {
            return followTransform.IsStopped == false
                   && weapon.IsCurrentlyAttacking == false;
        }
    }    

    public class IdleToFollow : ITransition
    {
        public FollowTransform followTransform;
    
        public override bool Test()
        {
            return followTransform.IsTargetReachable;
        }
    }    
    
    public class FollowToIdle : ITransition
    {
        public FollowTransform followTransform;
    
        public override bool Test()
        {
            return followTransform.IsTargetReachable == false;
        }
    }       
    public class FollowToAttack : ITransition
    {
        public FollowTransform followTransform;
    
        public override bool Test()
        {
            return followTransform.IsStopped;
        }
    }    
    public class AttackToCounter : ITransition
    {
        public WeaponScript weapon;
        public Parade parry;
        private bool _isCounter;

        public override void Init()
        {
            _isCounter = false;
            weapon.OnContact += (col) => VerifyIfCounter();
        }

        void VerifyIfCounter()
        {
            _isCounter = parry.IsParring;
        }
    
        public override bool Test()
        {
            if (_isCounter == false)
                return false;

            _isCounter = false;
            return true;
        }
    }    
    public class CounterToFollow : ITransition
    {
        public Counter counter;
    
        public override bool Test()
        {
            return counter.IsEnded;
        }
    }

}
