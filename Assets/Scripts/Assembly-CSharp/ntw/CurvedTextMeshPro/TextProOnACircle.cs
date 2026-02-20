using System;
using TMPro;
using UnityEngine;

namespace ntw.CurvedTextMeshPro
{
	[ExecuteInEditMode]
	public class TextProOnACircle : TextProOnACurve
	{
		[SerializeField]
		[Tooltip("The radius of the text circle arc")]
		private float m_radius = 10f;

		[SerializeField]
		[Tooltip("How much degrees the text arc should span")]
		private float m_arcDegrees = 90f;

		[SerializeField]
		[Tooltip("The angular offset at which the arc should be centered, in degrees")]
		private float m_angularOffset = -90f;

		[SerializeField]
		[Tooltip("The maximum angular distance between letters, in degrees")]
		private int m_maxDegreesPerLetter = 360;

		private float m_oldRadius = float.MaxValue;

		private float m_oldArcDegrees = float.MaxValue;

		private float m_oldAngularOffset = float.MaxValue;

		private float m_oldMaxDegreesPerLetter = float.MaxValue;

		protected override bool ParametersHaveChanged()
		{
			bool result = m_radius != m_oldRadius || m_arcDegrees != m_oldArcDegrees || m_angularOffset != m_oldAngularOffset || m_oldMaxDegreesPerLetter != (float)m_maxDegreesPerLetter;
			m_oldRadius = m_radius;
			m_oldArcDegrees = m_arcDegrees;
			m_oldAngularOffset = m_angularOffset;
			m_oldMaxDegreesPerLetter = m_maxDegreesPerLetter;
			return result;
		}

		protected override Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx)
		{
			float num = Mathf.Min(m_arcDegrees, textInfo.characterCount / textInfo.lineCount * m_maxDegreesPerLetter);
			float f = ((zeroToOnePos - 0.5f) * num + m_angularOffset) * (MathF.PI / 180f);
			float num2 = Mathf.Cos(f);
			float num3 = Mathf.Sin(f);
			float num4 = m_radius - textInfo.lineInfo[0].lineExtents.max.y * (float)textInfo.characterInfo[charIdx].lineNumber;
			Vector2 vector = new Vector2(num2 * num4, (0f - num3) * num4);
			return Matrix4x4.TRS(new Vector3(vector.x, vector.y, 0f), Quaternion.AngleAxis((0f - Mathf.Atan2(num3, num2)) * 57.29578f - 90f, Vector3.forward), Vector3.one);
		}
	}
}
