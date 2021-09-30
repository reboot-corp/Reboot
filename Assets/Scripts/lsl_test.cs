using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class lsl_test : MonoBehaviour
{
    public string StreamType = "EEG";
    public float scaleInput = 0.1f;
    StreamInfo[] streamInfos;
    StreamInlet streamInlet;
    float[] sample;
    private int channelCount = 0;

    void Update()
    {
        if (streamInlet == null)
        {
            streamInfos = LSL.LSL.resolve_stream("type", StreamType, 1, 0.0);
            if (streamInfos.Length > 0)
            {
                streamInlet = new StreamInlet(streamInfos[0]);
                channelCount = streamInlet.info().channel_count();
                streamInlet.open_stream();
            }
        }

        if (streamInlet != null)
        {
            sample = new float[channelCount];
            double lastTimeStamp = streamInlet.pull_sample(sample, 0.0f);
            if (lastTimeStamp != 0.0)
            {
                Process(sample, lastTimeStamp);
                while ((lastTimeStamp = streamInlet.pull_sample(sample, 0.0f)) != 0)
                {
                    Process(sample, lastTimeStamp);
                }
            }
        }
    }
    void Process(float[] newSample, double timeStamp)
    {
        var inputPosition = new Vector3(scaleInput * (newSample[0] - 0.5f), scaleInput * (newSample[1] - 0.5f), scaleInput * (newSample[2] - 0.5f));
        gameObject.transform.position = inputPosition;
    }
}