using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusTarget : MonoBehaviour
{
	public delegate void OnFocusChange(bool focused);
	public event OnFocusChange onFocusChange;
	
	[ShowInInspector] [ReadOnly]
	private bool _focused;
	public bool Focused {
		get => _focused;
		set
		{
			if (_focused && _focused != value)
			{
				onFocusChange?.Invoke(value);

				if (renderers.Length == 0)
					renderers = GetComponentsInChildren<Renderer>();
				for (int r = 0; r < renderers.Length; r++)
					for (int m = 0; m < renderers[r].materials.Length; m++)
					{
						Material material = renderers[r].materials[m];
						Color.RGBToHSV(material.color, out float H, out float S, out float V);
						material.color = Color.HSVToRGB(H, S, V + 0.25f * (value ? 1 : -1));
					}

				_focused = value;
			}
		}
	}

	[TextArea]
	public string info;

	private Renderer[] renderers = new Renderer[0];

	private void OnDestroy()
	{
		//Move this somewhere else
		if(Focused)
			Player.instance.Unfocus();
	}
}
