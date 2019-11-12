using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Thumbnail
{
	public static Sprite Generate(Transform transform)
	{
		//RuntimePreviewGenerator.BackgroundColor = Color.white;
		RuntimePreviewGenerator.Padding = 0.25f;
		RuntimePreviewGenerator.OrthographicMode = true;
		Texture2D thumbnail = RuntimePreviewGenerator.GenerateModelPreview(transform, 512, 512, false);
		return Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), new Vector2(0.5f, 0.5f));
	}
}
