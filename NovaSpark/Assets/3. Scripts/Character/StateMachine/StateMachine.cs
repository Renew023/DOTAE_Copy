using System;
public interface IState
{
	public void Enter();
    public void Exit();
	public void Update();
	//public void PhysicsUpdate();
}

[Serializable]
public abstract class StateMachine
{
	//이거 자꾸 헷갈리는데 Enum이 아니라서 currentState는 현재 상태에 들어간 값을 가져오는 구조
	//헷갈리는 관계로 currentState 대신 Value로 바꿈
	protected IState stateValue;

	public virtual void ChangeState(IState state)
	{
        stateValue?.Exit();
		stateValue = state;
		stateValue?.Enter();
	}

	public IState GetState()
	{
		return stateValue;
	}

	public void Update()
	{
		//currentState의 업데이트 확인
		stateValue?.Update();
	}
}
