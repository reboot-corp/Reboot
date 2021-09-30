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


 
public static class ArrayExtension
{

    public static double[] ToDouble(this float[] arr) =>
                                     System.Array.ConvertAll(arr, x => (double)x);

}
public class lsl_brainflowML_v2 : MonoBehaviour
{
    public TextMeshProUGUI concentration_lvl_txt;
    public int samplecount = 200; //Number of samples that are recorded per brainflow ML batch
    int samples_done = 0;
    
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
    private int[] channelCount_arr = { 1 };
    double lastTimeStamp;

    private BoardShim board_shim = null;
    private int sampling_rate = 0;
    int[] eeg_channels;
    public TextMeshProUGUI concentration_lvl_txt;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();
            int board_id = (int)BoardIds.GANGLION_BOARD;
            input_params.serial_port = "COM8";
            board_shim = new BoardShim(board_id, input_params);
            board_shim.prepare_session();
            board_shim.start_stream(450000, "file://brainflow_data.csv:w");
            sampling_rate = BoardShim.get_sampling_rate(board_id);
            Debug.Log("Brainflow streaming was started");
            int nfft = DataFilter.get_nearest_power_of_two(sampling_rate);
            eeg_channels = BoardShim.get_eeg_channels(board_shim.get_board_id());
            print("sampling rate :" + sampling_rate);

        }
        catch (BrainFlowException e)
        {
            Debug.Log(e);
        }
    }
    void Update()
    {
        if (streamInlet == null)
        {
            streamInfos = LSL.LSL.resolve_stream("type", StreamType, 1, 0.0);
            if (streamInfos.Length > 0)
            {
                streamInlet = new StreamInlet(streamInfos[0]);
                channelCount = streamInlet.info().channel_count();
                channelCount_arr[0] = channelCount;
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
                    if (samples_done < samplecount)
                    {
                        samples_done++;
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
                                datad[y, x] = System.Convert.ToDouble(dataf[y][x]);
                            }
                        }
                        //Debug recording:
                        using (StreamWriter outfile = new StreamWriter("test.csv"))
                        {
                            for (int y = 0; y < samplenum; y++)
                            {
                                string content = "";
                                for (int x = 0; x < streamnum; x++)
                                {
                                    content += datad[y, x].ToString("0.00") + ",";
                                }
                                outfile.WriteLine(content);
                            }
                        }
                        dataf = new List<List<float>>();
                        
                        samples_done = 0; //Reset samples
                        //Brainflow
                        System.Tuple<double[], double[]> bands = DataFilter.get_avg_band_powers(datad, channelCount_arr, samplecount, true);
                        //print(bands.Item1.Length);
                        double[] feature_vector = bands.Item1.Concatenate(bands.Item2);
                        print(feature_vector.Length);
                        BrainFlowModelParams model_params = new BrainFlowModelParams((int)BrainFlowMetrics.CONCENTRATION, (int)BrainFlowClassifiers.REGRESSION);
                        MLModel concentration = new MLModel(model_params);
                        concentration.prepare();
                        var concentration_lvl = concentration.predict(feature_vector);
                        concentration_lvl_txt.text = ((int)(concentration_lvl * 100f)).ToString() + " %";
                        print("Concentration: " + concentration_lvl);
                        concentration.release();

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
    //void Batch()
    //{
        //Need to calculate sampling rate:
        //double sampling_rate = samplenum_new / sampletime_new; //sampling rate in hz
        //Debug.Log(sampling_rate);

        
    //}
}
