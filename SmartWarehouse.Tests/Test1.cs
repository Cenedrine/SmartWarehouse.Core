using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWarehouse.Core;
using System;

namespace SmartWarehouse.Tests
{
    [TestClass]
    public class WarehouseSensorTests
    {
        [TestMethod]
        public void Constructor_ValidInputs_SetsInitialState()
        {
            string id = "SR-101";
            string name = "Chiller";
            string zone = "Cold-Vault-01";
            double threshold = 5.5;

            var sensor = new WarehouseSensor(id, name, zone, threshold);

            Assert.AreEqual("SR-101", sensor.SensorId);
            Assert.AreEqual("Chiller", sensor.SensorName);
            Assert.AreEqual("Cold-Vault-01", sensor.WareHouseZones);
            Assert.AreEqual(0.0, sensor.CurrentTemp);
            Assert.IsFalse(sensor.IsActive);
            Assert.IsFalse(sensor.IsAlertTriggered);
            Assert.AreEqual(5.5, sensor.CriticalTempThreshHold);
        }

        [TestMethod]
        public void Constructor_NoThresholdProvided_DefaultsToFour()
        {
            string id = "SR-102";
            string name = "Standard";
            string zone = "Main-Floor";

            var sensor = new WarehouseSensor(id, name, zone);

            Assert.AreEqual(4.0, sensor.CriticalTempThreshHold);
        }

        [TestMethod]
        public void Constructor_EmptySensorId_ThrowsArgumentException()
        {
            string badId = "";
            string name = "Sensor-A";
            string zone = "Zone-A";

            Action action = () => new WarehouseSensor(badId, name, zone);
            Assert.Throws<ArgumentException>(action);
        }

        [TestMethod]
        public void Constructor_EmptyWarehouseZone_ThrowsArgumentException()
        {
            string id = "SR-103";
            string name = "Sensor-B";
            string badZone = "";

            Action action = () => new WarehouseSensor(id, name, badZone);
            Assert.Throws<ArgumentException>(action);
        }

        [TestMethod]
        public void Lifecycle_Activate_SetsIsActiveToTrue()
        {
            var sensor = new WarehouseSensor("SR-101", "Chiller", "Zone-A");

            sensor.Activate();

            Assert.IsTrue(sensor.IsActive);
        }

        [TestMethod]
        public void Lifecycle_Deactivate_ResetsFlags()
        {
            var sensor = new WarehouseSensor("SR-101", "Chiller", "Zone-A");
            sensor.Activate();
            sensor.RecordTemp(10.0);

            sensor.Deactivate();

            Assert.IsFalse(sensor.IsActive);
            Assert.IsFalse(sensor.IsAlertTriggered);
        }

        [TestMethod]
        public void RecordTemp_SensorInactive_ThrowsInvalidOperationException()
        {
            var sensor = new WarehouseSensor("SR-101", "Chiller", "Zone-A");

            Action action = () => sensor.RecordTemp(2.0);
            Assert.Throws<InvalidOperationException>(action);
        }

        [DataTestMethod]
        [DataRow(3.9, false)]
        [DataRow(4.0, true)]
        [DataRow(4.1, true)]
        public void RecordTemp_ThresholdEvaluation_SetsAlertFlagCorrectly(double inputTemp, bool expectedAlertState)
        {
            var sensor = new WarehouseSensor("SR-101", "Chiller", "Zone-A", 4.0);
            sensor.Activate();

            sensor.RecordTemp(inputTemp);

            Assert.AreEqual(expectedAlertState, sensor.IsAlertTriggered);
        }

        [DataTestMethod]
        [DataRow(-50.0)]
        [DataRow(80.0)]
        public void RecordTemp_ExactBoundaryValues_AreValid(double boundaryTemp)
        {
            var sensor = new WarehouseSensor("SR-101", "Chiller", "Zone-A");
            sensor.Activate();

            sensor.RecordTemp(boundaryTemp);

            Assert.AreEqual(boundaryTemp, sensor.CurrentTemp);
        }

        [DataTestMethod]
        [DataRow(-50.1)]
        [DataRow(80.1)]
        public void RecordTemp_BoundaryViolations_ThrowsArgumentOutOfRangeException(double invalidTemp)
        {
            var sensor = new WarehouseSensor("SR-101", "Chiller", "Zone-A");
            sensor.Activate();

            Action action = () => sensor.RecordTemp(invalidTemp);
            Assert.Throws<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void UpdateThreshold_ModifyingThreshold_ReevaluatesAlertState()
        {
            var sensor = new WarehouseSensor("SR-101", "Chiller", "Zone-A", 10.0);
            sensor.Activate();
            sensor.RecordTemp(6.0);

            sensor.UpdateThreshhold(5.0);

            Assert.IsTrue(sensor.IsAlertTriggered);
        }
    }
}