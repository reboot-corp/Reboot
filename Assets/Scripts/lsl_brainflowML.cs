using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using brainflow;
using brainflow.math;
using LSL;
public static class ArrayExtension
{

    public static double[] ToDouble(this float[] arr) =>
                                     System.Array.ConvertAll(arr, x => (double)x);

}
public class lsl_brainflowML : MonoBehaviour
{
    public float sampletime = 1f;
    private float timeLeft;
    public string StreamType = "EEG";
    public float scaleInput = 0.1f;
    StreamInfo[] streamInfos;
    StreamInlet streamInlet;
    float[] sample;
    //float[,] dataf;
    
    List<List<float>> dataf = new List<List<float>>();
    float[,] dataf_array;
    //= new List<List<float>>()

    double[,] datad;
    private int channelCount = 0;
    double lastTimeStamp;
    
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
            
            lastTimeStamp = streamInlet.pull_sample(sample, 0.0f);
           
            if (lastTimeStamp != 0.0)
            {
                Process(sample, lastTimeStamp);
                while ((lastTimeStamp = streamInlet.pull_sample(sample, 0.0f)) != 0)
                {
                    Process(sample, lastTimeStamp);
                    //dataf_array = dataf.ToArray();
                    //datad = System.Array.ConvertAll(dataf_array, x => (double)x);
                    foreach(var y in dataf)
                    {
                        foreach (var x in dataf)
                        {
                            //Debug.Log(x[0]);
                            float[] x_arr_f = x.ToArray();
                            double[] x_arr_d = x_arr_f.ToDouble();
                            //datad.Add(x_arr_d);
                        }                            
                    }

                    dataf = new List<List<float>>();



                }
            }
        }
    }
    void Process(float[] newSample, double timeStamp)
    {
        dataf.Add(new List<float>(newSample));
        var inputPosition = new Vector3(scaleInput * (newSample[0] - 0.5f), scaleInput * (newSample[1] - 0.5f), scaleInput * (newSample[2] - 0.5f));
        gameObject.transform.position = inputPosition;

    }
}
