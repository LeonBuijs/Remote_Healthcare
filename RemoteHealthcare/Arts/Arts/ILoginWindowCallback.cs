namespace Arts;

public interface ILoginWindowCallback
{
	void OnLogin(string response);
	void ConnectionFailed();
}