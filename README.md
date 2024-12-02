# Alarms Events Observer

## Overview

This script defines a **NetLogic** for monitoring OPC UA alarms within an FTOptix-based HMI project. It leverages an observer pattern to track changes in alarm states and log corresponding events. The main logic is encapsulated in the `AlarmsObserverLogic` class, which initializes and manages an observer (`AlarmsCreationObserver`) for localized alarms.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Key Features](#key-features)
3. [Script Details](#script-details)
    - [AlarmsObserverLogic](#alarmsobserverlogic)
    - [LogsHandler.AlarmsCreationObserver](#logs-handleralarmscreationobserver)
4. [Usage Instructions](#usage-instructions)
5. [Logging](#logging)

## Prerequisites

- **FTOptix HMI Project**: Ensure the project has some alarms added to the project (the system system will automatically create the **Retained Alarms** and **Localized Alarms** nodes to comply with the OPC/UA specs).
- **Programming Environment**: This script requires a compatible FTOptix runtime environment with C# scripting capabilities.
- **Observer Pattern**: Familiarity with the observer pattern and OPC UA object structures.

## Key Features

1. **Dynamic Alarm Monitoring**: Subscribes to new alarm objects and tracks changes in their `ActiveState`, `AckedState`, and `ConfirmedState`.
2. **Event Logging**:
    - Tracks when alarms are enabled or removed.
    - Logs variable changes for key alarm states.
3. **Error Handling**: Ensures robust operation with logging for unexpected errors.


## Script Details

### **AlarmsObserverLogic**

This is the main entry point for the observer logic. It initializes and manages the observer for localized alarms.

- **Key Methods**:
  - `Start()`:
    - Assigns an affinity ID to the logic context.
    - Calls `StartObserver()` to initialize the alarm observer.
  - `Stop()`:
    - Disposes of the observer when the logic stops.
  - `StartObserver()`:
    - Locates the `LocalizedAlarms` node under the `RetainedAlarms` object.
    - Registers an `AlarmsCreationObserver` instance as a reference observer to monitor changes.

- **Properties**:
  - `alarmsCreationObserver`: Stores the event registration for the observer.
  - `affinityId`: Stores the thread affinity ID assigned to the logic context.

---

### **LogsHandler.AlarmsCreationObserver**

This class implements `IReferenceObserver` to handle changes in alarm references and states.

- **Key Methods**:
  - `OnReferenceAdded(...)`: Triggered when a new alarm object is added.  
    - Logs the addition of the alarm.
    - Subscribes to the `ActiveState`, `AckedState`, and `ConfirmedState` variables.
  - `OnReferenceRemoved(...)`: Triggered when an alarm object is removed.  
    - Logs the removal of the alarm.
  - `AlarmsCreationObserver_VariableChange(...)`: Triggered when subscribed alarm state variables change.  
    - Logs the name of the alarm and its new state value.

## Usage Instructions

1. **Add the Script to the FTOptix Project**:
    - Import this script into the NetLogic editor of your FTOptix HMI project.

2. **Configure the Retained Alarms**:
    - Ensure the project contains some alarms and some controls to trigger them. An AlarmGrid can also be used.

3. **Assign the NetLogic**:
    - The `AlarmsObserverLogic` NetLogic will automatically connect to a relevant application context (e.g., project root or a specific alarms manager node). No configuration is needed.

4. **Deploy and Test**:
    - Deploy the project to the runtime.
    - Generate alarms and verify that the events are logged appropriately.

## Logging

- Logs provide detailed feedback on observer operations:
  - **Info**:
    - Addition or removal of alarms.
    - Alarm state changes.
  - **Error**:
    - Issues locating the `LocalizedAlarms` node.
    - Errors during variable subscription.

Logs are prefixed with `LogsEventObserver` for quick identification in the runtime log system.

## Additional Notes

- **Thread Affinity**: The `affinityId` ensures the observer operates on the correct thread within the runtime.
- **Error Handling**: Extensive error logging helps identify and troubleshoot potential configuration or runtime issues.

## Cloning the repository

There are multiple ways to download this project, here is the recommended one

### Clone repository

1. Click on the green `CODE` button in the top right corner
2. Select `HTTPS` and copy the provided URL
3. Open FT Optix IDE
4. Click on `Open` and select the `Remote` tab
5. Paste the URL from step 2
6. Click `Open` button in bottom right corner to start cloning process

## Disclaimer

Rockwell Automation maintains these repositories as a convenience to you and other users. Although Rockwell Automation reserves the right at any time and for any reason to refuse access to edit or remove content from this Repository, you acknowledge and agree to accept sole responsibility and liability for any Repository content posted, transmitted, downloaded, or used by you. Rockwell Automation has no obligation to monitor or update Repository content

The examples provided are to be used as a reference for building your own application and should not be used in production as-is. It is recommended to adapt the example for the purpose, observing the highest safety standards.
