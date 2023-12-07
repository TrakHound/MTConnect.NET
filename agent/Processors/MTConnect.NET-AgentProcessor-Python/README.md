![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect Python Agent Processor

## Nuget
<table>
    <thead>
        <tr>
            <td style="font-weight: bold;">Package Name</td>
            <td style="font-weight: bold;">Downloads</td>
            <td style="font-weight: bold;">Link</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>MTConnect.NET-AgentProcessor-Python</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-AgentProcessor-Python?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-AgentProcessor-Python">https://www.nuget.org/packages/MTConnect.NET-AgentProcessor-Python</a></td>
        </tr>
    </tbody>
</table>

## Description
This Agent Processor uses individual Python script files to transform input data

## Configuration
```yaml
- python:
    directory: processors
```

* `directory` - The directory to load and monitor for ".py" script files.

## Scripts
Scripts are implemented using IronPython and supports Python v3. Both the MTConnect Entity to process and the MTConnect Agent are both accessible from the script.

### Example 1
Change the Result for a DataItem with a Type = "EMERGENCY_STOP"
```python
def process(observation):

    if observation.DataItem.Type == 'EMERGENCY_STOP':

        result = observation.GetValue('Result')

        if result.lower() == 'TRUE'.lower():
            observation.AddValue('Result', 'ARMED')
        else:
            observation.AddValue('Result', 'TRIGGERED')

    return observation
```

### Example 2
Change the Result for a DataItem with a Type = "PATH_FEEDRATE_OVERRIDE" from a Percentage to an Integer by multiplying by 100
```python
def process(observation):

    if observation.DataItem.Type == 'PATH_FEEDRATE_OVERRIDE':

        result = float(observation.GetValue('Result'))
        observation.AddValue('Result', result * 100)

    return observation
```

### Example 3
Add a new TimeSeries observation for any time a DataItem with an ID = "L2p1Fact" changes
```python
import clr
clr.AddReference("MTConnect.NET-Common")
import MTConnect.Input

def process(observation):

    if observation.DataItem.Id == "L2p1Fact": 

        timeseries = MTConnect.Input.TimeSeriesObservationInput()
        timeseries.DataItemKey = 'L2p1Sensor'
        timeseries.SampleRate = 100

        n = 15
        samples = [0] * n

        for x in range(n):
            samples[x] = float(x)

        timeseries.Samples = samples

        observation.Agent.AddObservation(observation.DataItem.Device.Uuid, timeseries)

    return observation
```

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use.
