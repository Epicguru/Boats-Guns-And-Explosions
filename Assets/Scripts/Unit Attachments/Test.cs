using UnityEngine;
using UnityEngine.Networking;

public class Test : NetworkBehaviour
{
    public NetParent Parent;
    public Animator Anim;

    public Transform[] ProjectileSpawns;

	public void Start()
	{
        if (isServer)
        {
            GetComponent<NetParenting>().SetParent(Parent);
        }
	}

    public void Update()
    {
        Anim.SetBool("Fire", Input.GetKey(KeyCode.Space));        
    }

    public void Fire(AnimationEvent e)
    {
        if (!isServer)
            return;

        if(e.stringParameter == "Fire")
        {
            int index = e.intParameter;

            Transform spawn = ProjectileSpawns[index];

            // Spawn projectile...
            Projectile.Spawn(ProjectileType.STANDARD, spawn.position, spawn.right.ToAngle(), Faction.RED);
        }
    }
}