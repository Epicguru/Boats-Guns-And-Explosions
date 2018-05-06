using UnityEngine;
using UnityEngine.Networking;

public class Test : NetworkBehaviour
{
    public NetParent Parent;

	public void Start()
	{
        if (isServer)
        {
            GetComponent<NetParenting>().SetParent(Parent);
        }
	}
}