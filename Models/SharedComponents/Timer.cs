﻿namespace SharedComponents
{
	using System;
	using SafetySharp.Modeling;

	public class Timer : Component
	{
		public bool Triggered = false;
		private int _i = 1;

		public int Do()
		{
			var q = 38;
			q = Choose.Value(23, 4, 23, 55);
			_i = _i + 1;
			return _i + q;
		}
	}
}