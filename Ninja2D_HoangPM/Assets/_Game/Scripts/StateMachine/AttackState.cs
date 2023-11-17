using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    float timner;

    public void OnEnter(Enemy enemy)
    {
        if (enemy.Target != null)
        {
            //doi huong toi player
            enemy.ChangeDirection(enemy.Target.transform.position.x > enemy.transform.position.x);
            enemy.StopMoving();
            enemy.Attack();
        }

        timner = 0;
    }

    public void OnExecute(Enemy enemy)
    {
        timner += Time.deltaTime;
        if (timner >= 1.5f)
        {
            enemy.ChangeState(new PatrolState());
        }
    }

    public void OnExit(Enemy enemy)
    {

    }
}
