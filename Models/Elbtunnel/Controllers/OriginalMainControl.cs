﻿namespace Elbtunnel.Controllers
{
	using System;
	using SafetySharp.Modeling;
	using Sensors;
	using SharedComponents;

	public class OriginalMainControl : Component, IMainControl
	{
		/// <summary>
		///   The sensor that detects high vehicles on the left lane.
		/// </summary>
		private readonly IVehicleDetector _leftDetector;

		/// <summary>
		///   The sensor that detects overheight vehicles on any lane.
		/// </summary>
		private readonly IVehicleDetector _positionDetector;

		/// <summary>
		///   The sensor that detects high vehicles on the right lane.
		/// </summary>
		private readonly IVehicleDetector _rightDetector;

		/// <summary>
		///   The timer that is used to deactivate the main-control automatically.
		/// </summary>
		private readonly Timer _timer;

		/// <summary>
		///   The number of high vehicles currently in the main-control area.
		/// </summary>
		// TODO: OverflowBehavior.Error
		private int _count;

		/// <summary>
		///   Indicates whether a vehicle has been detected on the left lane.
		/// </summary>
		private bool _vehicleOnLeftLane;

		/// <summary>
		///   Indicates whether a vehicle has been detected on the right lane.
		/// </summary>
		private bool _vehicleOnRightLane;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="positionDetector">The sensor that detects overheight vehicles on any lane.</param>
		/// <param name="leftDetector">The sensor that detects high vehicles on the left lane.</param>
		/// <param name="rightDetector">The sensor that detects high vehicles on the right lane.</param>
		/// <param name="timeout">The amount of time after which the main-control is deactivated.</param>
		public OriginalMainControl(IVehicleDetector positionDetector, IVehicleDetector leftDetector, IVehicleDetector rightDetector, int timeout)
		{
			_timer = new Timer(timeout);
			_positionDetector = positionDetector;
			_leftDetector = leftDetector;
			_rightDetector = rightDetector;
		}

		/// <summary>
		///   Indicates whether an vehicle leaving the main-control area on the right lane has been detected.
		/// </summary>
		public bool IsVehicleLeavingOnRightLane()
		{
			return _vehicleOnRightLane;
		}

		/// <summary>
		///   Indicates whether an vehicle leaving the main-control area on the left lane has been detected.
		/// </summary>
		public bool IsVehicleLeavingOnLeftLane()
		{
			return _vehicleOnLeftLane;
		}

		/// <summary>
		///   Gets the number of vehicles that entered the height control area during the current system step.
		/// </summary>
		public extern int GetNumberOfEnteringVehicles();

		/// <summary>
		///   Updates the internal state of the component.
		/// </summary>
		public override void Update()
		{
			var numberOfHVs = GetNumberOfEnteringVehicles();
			if (numberOfHVs > 0)
			{
				_count += numberOfHVs;
				_timer.Start();
			}

			var active = _count != 0;
			var onlyRightTriggered = !_leftDetector.IsVehicleDetected() && _rightDetector.IsVehicleDetected();

			_vehicleOnLeftLane = _positionDetector.IsVehicleDetected() && !onlyRightTriggered && active;
			_vehicleOnRightLane = _positionDetector.IsVehicleDetected() && onlyRightTriggered && active;

			if (_vehicleOnLeftLane)
				--_count;

			if (_vehicleOnRightLane)
				--_count;

			if (_timer.HasElapsed())
				_count = 0;

			if (_count == 0)
				_timer.Stop();
		}
	}
}