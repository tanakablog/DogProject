using UnityEngine;
using UnityEditor;
using System;

/*
[CustomEditor( typeof( SpriteRenderer))]
public class CustomSpriteRenderer : Editor
{
	public void OnEnable()
	{
		m_SpriteRenderer = (SpriteRenderer)target;

		selectsprite = SpriteManager.Instance.SpriteNameToSpriteEnum(m_SpriteRenderer.sprite.name);
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Image");
		selectsprite = (SpriteTable.eSprite)EditorGUILayout.EnumPopup(selectsprite);
		m_SpriteRenderer.sprite = SpriteManager.Instance.GetSprite(selectsprite);
		EditorGUILayout.EndHorizontal();	

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Color");
		m_SpriteRenderer.color = EditorGUILayout.ColorField(m_SpriteRenderer.color);
		EditorGUILayout.EndHorizontal();

		m_SpriteRenderer.material = (Material)EditorGUILayout.ObjectField("Material", m_SpriteRenderer.material, typeof(Material), false);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Sorting Layer");
		m_SpriteRenderer.sortingLayerID = EditorGUILayout.LayerField(m_SpriteRenderer.sortingLayerID);
		EditorGUILayout.EndHorizontal();

		m_SpriteRenderer.sortingOrder = EditorGUILayout.IntField("Order in Layer", m_SpriteRenderer.sortingOrder);
		

	}
	SpriteTable.eSprite selectsprite = (SpriteTable.eSprite)0;

	SpriteRenderer m_SpriteRenderer;
}
*/