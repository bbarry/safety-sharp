﻿namespace Elbtunnel.Environment
{
	using System;
	using SafetySharp.Modeling;

	/// <summary>
	///   Represents a collection of vehicles.
	/// </summary>
	/// <remarks>
	///   TODO: Currently, the collection is hardcoded to 3 vehicles. Replaces this by an array once S# supports arrays.
	/// </remarks>
	public class VehicleCollection : Component
	{
		private readonly IVehicle _vehicle1;
		private readonly IVehicle _vehicle2;
		private readonly IVehicle _vehicle3;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public VehicleCollection(IVehicle vehicle1, IVehicle vehicle2, IVehicle vehicle3)
		{
			_vehicle1 = vehicle1;
			_vehicle2 = vehicle2;
			_vehicle3 = vehicle3;

			Bind(_vehicle1.RequiredPorts.IsTunnelClosed = ProvidedPorts.CheckIsTunnelClosed);
			Bind(_vehicle2.RequiredPorts.IsTunnelClosed = ProvidedPorts.CheckIsTunnelClosed);
			Bind(_vehicle3.RequiredPorts.IsTunnelClosed = ProvidedPorts.CheckIsTunnelClosed);
		}

		// TODO: Remove once S# supportes port forwardings
		private bool CheckIsTunnelClosed()
		{
			return IsTunnelClosed();
		}

		/// <summary>
		///   Informs the vehicle whether the tunnel is closed.
		/// </summary>
		// TODO: Use a property once supported by the S# compiler.
		public extern bool IsTunnelClosed();

		/// <summary>
		///   Gets the position of the vehicle with the given <paramref name="vehicleIndex" />.
		/// </summary>
		/// <param name="vehicleIndex">The index of the vehicle that should be checked.</param>
		public int GetVehiclePosition(int vehicleIndex)
		{
			switch (vehicleIndex)
			{
				case 0:
					return _vehicle1.GetPosition();
				case 1:
					return _vehicle2.GetPosition();
				default:
					return _vehicle3.GetPosition();
			}
		}

		/// <summary>
		///   Gets the speed of the vehicle with the given <paramref name="vehicleIndex" />.
		/// </summary>
		/// <param name="vehicleIndex">The index of the vehicle that should be checked.</param>
		public int GetVehicleSpeed(int vehicleIndex)
		{
			switch (vehicleIndex)
			{
				case 0:
					return _vehicle1.GetSpeed();
				case 1:
					return _vehicle2.GetSpeed();
				default:
					return _vehicle3.GetSpeed();
			}
		}

		/// <summary>
		///   Gets the lane of the vehicle with the given <paramref name="vehicleIndex" />.
		/// </summary>
		/// <param name="vehicleIndex">The index of the vehicle that should be checked.</param>
		public Lane GetVehicleLane(int vehicleIndex)
		{
			switch (vehicleIndex)
			{
				case 0:
					return _vehicle1.GetLane();
				case 1:
					return _vehicle2.GetLane();
				default:
					return _vehicle3.GetLane();
			}
		}

		/// <summary>
		///   Gets the speed of the vehicle with the given <paramref name="vehicleIndex" />.
		/// </summary>
		/// <param name="vehicleIndex">The index of the vehicle that should be checked.</param>
		public VehicleKind GetVehicleKind(int vehicleIndex)
		{
			switch (vehicleIndex)
			{
				case 0:
					return _vehicle1.GetKind();
				case 1:
					return _vehicle2.GetKind();
				default:
					return _vehicle3.GetKind();
			}
		}
	}
}