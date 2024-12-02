#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.Alarm;
using System.Collections.Generic;
using LogsHandler;
using System.Transactions;
#endregion

public class AlarmsObserverLogic : BaseNetLogic
{
    public override void Start()
    {
        // Build the observer for the output messages
        affinityId = LogicObject.Context.AssignAffinityId();
        StartObserver();
    }

    public override void Stop()
    {
        // Destroy the observer when closing the NetLogic
        alarmsCreationObserver?.Dispose();
    }

    public void StartObserver()
    {
        // Get the server object
        var retainedAlarmsObject = LogicObject.Context.GetNode(FTOptix.Alarm.Objects.RetainedAlarms);
        var localizedAlarmsObject = retainedAlarmsObject.GetVariable("LocalizedAlarms");
        var localizedAlarmsNodeId = (NodeId) localizedAlarmsObject.Value;
        IUANode localizedAlarmsContainer = null;
        if (localizedAlarmsNodeId?.IsEmpty == false)
            localizedAlarmsContainer = LogicObject.Context.GetNode(localizedAlarmsNodeId);
        if (localizedAlarmsContainer == null)
        {
            Log.Error("AlarmsObserverLogic", "LocalizedAlarms node not found");
            return;
        }
        // Create a new observer
        var logsObserver = new AlarmsCreationObserver();
        // Register the observer to the server node
        alarmsCreationObserver = localizedAlarmsContainer.RegisterEventObserver(
            logsObserver,
            EventType.ForwardReferenceAdded | EventType.ForwardReferenceRemoved,
            affinityId);
    }

    private IEventRegistration alarmsCreationObserver;
    private uint affinityId;
}

namespace LogsHandler
{
    class AlarmsCreationObserver : IReferenceObserver
    {
        public AlarmsCreationObserver()
        {
            Log.Info("LogsEventObserver", "Starting alarms observer");
        }

        public void OnReferenceAdded(IUANode sourceNode, IUANode targetNode, NodeId referenceTypeId, ulong senderId)
        {
            Log.Info("LogsEventObserver", $"{targetNode.BrowseName} alarm got enabled");
            try
            {
                if (targetNode is not UAObject alarmNode)
                {
                    Log.Error("LogsEventObserver", "Alarm node is invalid");
                    return;
                }

                alarmNode.GetVariable("ActiveState/Id").VariableChange += AlarmsCreationObserver_VariableChange;
                alarmNode.GetVariable("AckedState/Id").VariableChange += AlarmsCreationObserver_VariableChange;
                alarmNode.GetVariable("ConfirmedState/Id").VariableChange += AlarmsCreationObserver_VariableChange;
            }
            catch
            {
                Log.Error("LogsEventObserver", "Error while trying to subscribe to alarm variables");
            }
        }

        private void AlarmsCreationObserver_VariableChange(object sender, VariableChangeEventArgs e)
        {
            var variable = sender as IUAVariable;
            Log.Info("LogsEventObserver", $"{variable.Owner.BrowseName} alarm status changed to {e.NewValue}");
        }

        public void OnReferenceRemoved(IUANode sourceNode, IUANode targetNode, NodeId referenceTypeId, ulong senderId)
        {
            Log.Info("LogsEventObserver", $"{targetNode.BrowseName} alarm got removed");
        }
    }
}
