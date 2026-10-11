namespace SmartWarehouse.Core
{
    public class WarehouseSensor
    {
        public string SensorId { get; private set;}
        public string SensorName { get; private set;}
        public string WareHouseZones { get; private set;}
        public double CurrentTemp {  get; private set;}
        public bool IsActive { get; private set;}
        public bool IsAlertTriggered { get; private set;}
        public double CriticalTempThreshHold { get; private set;}

        public WarehouseSensor(string sensorId, string sensorName, string wareHouseZones, double criticalTempTheshHold = 4.0)
        {
            if (string.IsNullOrWhiteSpace(sensorId))
            {
                throw new ArgumentException("you Cannot leave this blank", nameof(sensorId));
            }
            if (string.IsNullOrWhiteSpace(wareHouseZones))
            {
                throw new ArgumentException("you Cannot leave this blank", nameof(sensorId));
            }
            SensorId = sensorId.Trim();
            SensorName = sensorName.Trim();
            WareHouseZones = wareHouseZones.Trim();
            CurrentTemp = 0.0;
            IsActive = false;
            IsAlertTriggered = false;
            CriticalTempThreshHold = criticalTempTheshHold;
        }
        public void Activate()
        {
            IsActive = true;
        }
        public void Deactivate() 
        { 
            IsActive = false;
            IsAlertTriggered = false;
        }
        public void RecordTemp(double newTemp)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("The Sensor isn't currently active");
            }
            if (newTemp < -50.0 || newTemp > 80.0)
            {
                throw new ArgumentOutOfRangeException(nameof(newTemp), "Temp needs to be between -50.0 and 80.0");
            }
            CurrentTemp = newTemp;
            if (newTemp >= CriticalTempThreshHold)
            {
                IsAlertTriggered = true;
            }
            else
            {
                IsAlertTriggered = false;
            }
        }
        public void UpdateThreshhold(double newThreshhold)
        {
            if (newThreshhold < -30.0 || newThreshhold > 50.0)
            {
                throw new ArgumentOutOfRangeException(nameof(newThreshhold), "Threshhold needs to be between -30.0 and 50.0");
            }
            CriticalTempThreshHold = newThreshhold;

            if (CurrentTemp >= CriticalTempThreshHold)
            {
                IsAlertTriggered = true;
            }
            else
            {
                IsAlertTriggered= false;
            }
        }
    }
    
}
