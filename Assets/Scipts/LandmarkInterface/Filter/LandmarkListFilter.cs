﻿using System.Collections.Generic;
using UnityEngine;

namespace LandmarkInterface.Filter
{
	public class LandmarkListFilter
	{
		private double timeInterval;
		private double noise;
		private double displacementLimit;
		private int count;
		private List<LandmarkFilter> landmarkFilters;
		private int warmup;

		public LandmarkListFilter(double timeInterval, double noise, double displacementLimit)
		{
			this.timeInterval = timeInterval;
			this.noise = noise;
			this.displacementLimit = displacementLimit;
		}

		private void setupFilters(List<Vector3> normalizedLandmarkList)
		{
			landmarkFilters = new List<LandmarkFilter>();
			this.count = normalizedLandmarkList.Count;
			for (int i = 0; i < count; i++)
			{
				landmarkFilters.Add(new LandmarkFilter(timeInterval, noise));
			}
		}

		public void CorrectAndPredict(List<Vector3> normalizedLandmarkList)
		{
			if(landmarkFilters == null)
			{
				setupFilters(normalizedLandmarkList);
				correctAndPredict(normalizedLandmarkList);
			}
			else
			{
				var oldPosition = landmarkFilters[0].GetPosition();
				var newLandmark = normalizedLandmarkList[0];
				var newPosition = new Vector3(newLandmark.x, newLandmark.y, newLandmark.z);
				var displacement = Vector3.Distance(oldPosition, newPosition);
				//Debug.Log(displacement);
				if (displacement > displacementLimit)
				{
					if(warmup < 20)
					{
						warmup++;
						correctAndPredict(normalizedLandmarkList);
					}
					else
					{
						Debug.Log("Filtered error landmark data(d): " + displacement);
					}
				}
				else
				{ 
					correctAndPredict(normalizedLandmarkList);
				}
			}

		}

		private void correctAndPredict(List<Vector3> normalizedLandmarkList)
		{
			for (int i = 0; i < count; i++)
			{
				landmarkFilters[i].Correct(normalizedLandmarkList[i]);
				landmarkFilters[i].Predict();
			}
		}

		public List<Vector3> GetPositions()
		{
			if(landmarkFilters == null || count == 0 || landmarkFilters.Count != count)
				return null;
			var result = new List<Vector3>(count);
			for (int i = 0; i < count; i++)
			{
				result.Add(landmarkFilters[i].GetPosition());
			}
			return result;
		}

		public void UpdateFilterParameter(double timeInterval, double noise, double maxDisplacement)
		{
			this.displacementLimit = maxDisplacement;
			if (timeInterval == this.timeInterval && noise == this.noise)
				return;
			for (int i = 0; i < count; i++)
			{
				landmarkFilters[i].UpdateFilterParameter(timeInterval, noise);
			}
		}
	}
}
