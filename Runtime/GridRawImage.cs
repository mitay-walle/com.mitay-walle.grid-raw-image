using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace mitaywalle.UI.Packages.GridImage.Runtime
{
	public class GridRawImage : MaskableGraphic, ILayoutSelfController, ICanvasRaycastFilter
	{
		static Vector2 UV0 = new Vector2(0, 0);
		static Vector2 UV1 = new Vector2(0, 1);
		static Vector2 UV2 = new Vector2(1, 1);
		static Vector2 UV3 = new Vector2(1, 0);

		public Texture texture;
		[SerializeField] bool _resize = true;
		[SerializeField] Vector2 _cellSize = Vector2.one * 100;
		[SerializeField] float _extrude;
		[SerializeField] GridShape _shape = GridShape.Default;

		DrivenRectTransformTracker _tracker;

		static List<Vector2Int> _buffer = new List<Vector2Int>();
		Color32 color32;

		/// <summary>
		/// Image's texture comes from the UnityEngine.Image.
		/// </summary>
		public override Texture mainTexture
		{
			get
			{
				if (texture == null)
				{
					if (material != null && material.mainTexture != null)
					{
						return material.mainTexture;
					}
					return s_WhiteTexture;
				}

				return texture;
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			_shape.GetValidPositions(_buffer);
			vh.Clear();
			color32 = color;

			for (int i = 0; i < _buffer.Count; i++)
			{
				Vector2 position = _buffer[i] * _cellSize;
				DrawPosition(vh, position, i);
			}
		}

		private void DrawPosition(VertexHelper vh, Vector2 p, int i)
		{
			var corners = new Vector4(p.x - _extrude, p.y - _extrude, p.x + _cellSize.x + _extrude, p.y + _cellSize.y + _extrude);

			vh.AddVert(new Vector3(corners.x, corners.y), color32, UV0);
			vh.AddVert(new Vector3(corners.x, corners.w), color32, UV1);
			vh.AddVert(new Vector3(corners.z, corners.w), color32, UV2);
			vh.AddVert(new Vector3(corners.z, corners.y), color32, UV3);

			i *= 4;

			vh.AddTriangle(i + 0, i + 1, i + 2);
			vh.AddTriangle(i + 2, i + 3, i + 0);
		}

		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			Vector2 local;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local))
				return false;
			
			_shape.GetValidPositions(_buffer);
			
			if (_buffer.Count == 0) return false;
			for (int i = 0; i < _buffer.Count; i++)
			{
				Vector2 p = _buffer[i] * _cellSize;
				Vector4 corners = new Vector4(p.x - _extrude, p.y - _extrude, p.x + _cellSize.x + _extrude, p.y + _cellSize.y + _extrude);
				Rect rect = new Rect(corners.x, corners.y, corners.z - corners.x, corners.w - corners.y);

				if (rect.Contains(local))
				{
					return true;
				}
			}
			return false;
		}

		public void SetLayoutHorizontal()
		{
			if (!_resize)
			{
				_tracker.Clear();
				return;
			}

			_tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _shape.Size.x * _cellSize.x);
		}

		public void SetLayoutVertical()
		{
			if (!_resize)
			{
				_tracker.Clear();
				return;
			}

			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _shape.Size.y * _cellSize.y);
		}

		private void OnDrawGizmosSelected()
		{
			_shape.OnDrawGizmos(transform, Color.green, true, _cellSize, true);
		}
	}
}