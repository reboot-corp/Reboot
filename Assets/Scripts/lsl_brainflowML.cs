using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using brainflow;
using brainflow.math;

using TMPro;

using LSL;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


 
//public static class ArrayExtension
//{

    //public static double[] ToDouble(this float[] arr) =>
                                     //System.Array.ConvertAll(arr, x => (double)x);

//}
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
                    //lsl-brainflow bridge
                    if (timeLeft > 0)
                    {
                        timeLeft -= Time.deltaTime;
                    }
                    else
                    {
                        //Process
                        int samplenum = dataf.Count;
                        int streamnum = sample.Length;
                        datad = new double[samplenum, streamnum];
                        for (int y = 0; y < samplenum; y++)
                        {
                            for (int x = 0; x < streamnum; x++)
                            {
                                //Debug.Log(x[0]);
                                //datad[y, x] = System.Convert.ToDouble(dataf[y][x]);
                            }
                        }
                        dataf = new List<List<float>>();
                        Batch(samplenum, sampletime); //Run batch on last second of recorded data (ie dataf or datad)
                        timeLeft = sampletime; //Reset timer
                        BinaryFormatter bf = new BinaryFormatter();
                        using (FileStream fs = new FileStream("test.txt", FileMode.Create))
                            bf.Serialize(fs, datad);
                        using (FileStream fs = new FileStream("test.txt", FileMode.Open))
                            datad = (double[,])bf.Deserialize(fs);
                    }
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
    void Batch(int samplenum_new, double sampletime_new)
    {
        //Need to calculate sampling rate:
        double sampling_rate = samplenum_new / sampletime_new; //sampling rate in hz
        Debug.Log(sampling_rate);

        
    }
}
