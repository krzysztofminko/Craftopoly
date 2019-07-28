using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoney
{
	float Money {get;set;}
	void Pay(IMoney payTo, float money);
    
}
